package com.aoihosizora.ncmlatwclient

import android.Manifest
import android.app.*
import android.content.ComponentName
import android.content.Context
import android.content.DialogInterface
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.provider.Settings
import android.text.TextUtils
import android.view.WindowManager
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.Toast
import androidx.annotation.UiThread
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.app.NotificationCompat
import com.jwsd.libzxing.OnQRCodeScanCallback
import com.jwsd.libzxing.QRCodeManager
import kotlinx.android.synthetic.main.activity_main.*
import java.text.SimpleDateFormat
import java.util.*

class MainActivity : AppCompatActivity() {

    companion object {
        private const val QRCODE_MAGIC = "NCMLATW-"
        private const val REQUEST_CAMERA_PERMISSION_CODE = 1
        private var SHOWING_NO_SESSION_DIALOG = false
        private var SHOWING_DISCONNECT_DIALOG = false
    }

    private lateinit var adtIP: ArrayAdapter<String>
    private lateinit var adtPort: ArrayAdapter<String>
    private var lastBackPressedTime: Date? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        initUI()
        initPermission()
        initListener()
    }

    private fun initUI() {
        adtIP = ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, getDataFromSP("ip"))
        adtPort = ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, getDataFromSP("port"))
        edt_ip.setAdapter(adtIP)
        edt_port.setAdapter(adtPort)
        adtIP.notifyDataSetChanged()
        adtPort.notifyDataSetChanged()
    }

    private fun initPermission() {
        var granted = false
        val flat = Settings.Secure.getString(contentResolver, "enabled_notification_listeners")
        if (!TextUtils.isEmpty(flat)) {
            for (name in flat.split(":")) {
                val cn = ComponentName.unflattenFromString(name)
                if (cn != null && TextUtils.equals(packageName, cn.packageName)) {
                    granted = true
                    break
                }
            }
        }

        if (!granted) {
            showAlertDialog("授权", "该应用需要授权通知访问，请授权。", false) { _, _ ->
                // 通知へのアクセス
                val intent = Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS")
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
                startActivity(intent)
            }
        }
    }

    override fun onResume() {
        super.onResume()
        window.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON)
    }

    override fun onPause() {
        window.clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON)
        super.onPause()
    }

    override fun onDestroy() {
        processStopService()
        super.onDestroy()
    }

    override fun onBackPressed() {
        val current = Calendar.getInstance().time
        if (lastBackPressedTime == null || current.time - lastBackPressedTime!!.time > 2000) {
            Toast.makeText(this, "再按一次退出应用", Toast.LENGTH_SHORT).show()
            lastBackPressedTime = current
        } else {
            super.onBackPressed()
        }
    }

    private fun initListener() {
        MainService.eventListener = EventListener()

        edt_ip.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }
        edt_port.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }

        btn_qrcode.setOnClickListener { scanQrcode() }
        btn_manual.setOnClickListener { updateManually() }
        btn_clearLog.setOnClickListener { clearLog() }
        btn_service.setOnClickListener { if (btn_service.text == "启动服务") startService() else stopService() }
    }

    // ===============
    // service related
    // ===============

    @UiThread
    private fun startService() {
        // check address
        val ip = edt_ip.text.toString()
        val portStr = edt_port.text.toString()
        if (!SocketUtils.checkIPString(ip)) {
            showAlertDialog("启动服务", "IP 地址格式有误。")
            return
        }
        if (!SocketUtils.checkPortString(portStr)) {
            showAlertDialog("启动服务", "无效的端口。")
            return
        }
        val port = Integer.parseInt(portStr)

        // show progress dlg
        var isCanceled = false
        val dlg = ProgressDialog(this)
        dlg.setMessage("正在连接...")
        dlg.setCancelable(true)
        dlg.setOnCancelListener { isCanceled = true }
        dlg.show()

        Thread {
            // start connect
            SocketUtils.storeAddress(ip, port)
            val ok = SocketUtils.ping()
            if (isCanceled) {
                return@Thread
            }

            // start service
            runOnUiThread {
                dlg.dismiss()
                if (!ok) {
                    showAlertDialog("启动服务", "无法连接到给定地址。")
                } else {
                    processStartService(ip, portStr)
                }
            }
        }.start()
    }

    @UiThread
    private fun stopService() {
        // stop service
        processStopService()

        // send destroyed
        val dto = DestroyedDto(true)
        dto.toJSON()?.let { sendToServer(it.toString(), failAlert = false) }
    }

    /**
     * Process start service, includes: start service, update ui, save data to sp.
     */
    @UiThread
    private fun processStartService(ip: String, port: String) {
        // start service
        if (MainService.isRunning(this)) {
            appendLog("Before start service, stopping service...")
            MainService.mediaCallback?.let { MainService.mediaController?.unregisterCallback(it) }
            MainService.mediaController = null
            MainService.mediaCallback = null
            stopService(Intent(this, MainService::class.java))
        }
        if (!MainService.isRunning(this)) {
            appendLog("Starting service...")
            startService(Intent(this, MainService::class.java))
        }

        // save sp and update adapter
        if (saveDataToSP("ip", ip)) {
            adtIP.add(ip)
            adtIP.notifyDataSetChanged()
        }
        if (saveDataToSP("port", port)) {
            adtPort.add(port)
            adtPort.notifyDataSetChanged()
        }

        // update ui
        btn_service.text = "停止服务"
        edt_ip.isEnabled = false
        edt_port.isEnabled = false
        btn_qrcode.isEnabled = false
        btn_manual.isEnabled = true

        // setup notification
        val nm = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
        val channel = NotificationChannel("ncmlatw", "服务状态", NotificationManager.IMPORTANCE_DEFAULT)
        nm.createNotificationChannel(channel)
        val contentIntent = PendingIntent.getActivity(this, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT)
        val actionIntent = PendingIntent.getService(this, 0, Intent(this, NotificationIntentService::class.java), PendingIntent.FLAG_UPDATE_CURRENT)
        val notification = NotificationCompat.Builder(this, "ncmlatw")
            .setAutoCancel(false)
            .setOngoing(true)
            .setContentTitle("服务已启动")
            .addAction(R.mipmap.ic_launcher, "手动更新", actionIntent)
            .setContentText("正在与 PC 端保持着连接")
            .setWhen(System.currentTimeMillis())
            .setSmallIcon(R.mipmap.ic_launcher)
            .setContentIntent(contentIntent)
            .build()
        nm.notify(1, notification)
    }

    /**
     * Process stop service, includes: stop service, update ui.
     */
    @UiThread
    private fun processStopService() {
        // stop service
        if (MainService.isRunning(this)) {
            appendLog("Stopping service...")
            MainService.mediaCallback?.let { MainService.mediaController?.unregisterCallback(it) }
            MainService.mediaController = null
            MainService.mediaCallback = null
            stopService(Intent(this, MainService::class.java))
        }

        // update ui
        btn_service.text = "启动服务"
        edt_ip.isEnabled = true
        edt_port.isEnabled = true
        btn_qrcode.isEnabled = true
        btn_manual.isEnabled = false

        // remove notification
        val nm = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager
        nm.cancel(1)
    }

    // =====================
    // event respond related
    // =====================

    inner class EventListener : MainService.EventListener {

        @UiThread
        override fun onNoSession() {
            showAlertDialog("启动服务", "没有打开网易云音乐 app。")
            processStopService()
        }

        @UiThread
        override fun onSessionDestroyed() {
            if (!SHOWING_NO_SESSION_DIALOG) {
                SHOWING_NO_SESSION_DIALOG = true
                showAlertDialog("服务", "网易云音乐 app 已经关闭。", false) { _, _ -> SHOWING_NO_SESSION_DIALOG = false }
            }
            processStopService()

            val dto = DestroyedDto(true)
            dto.toJSON()?.let { sendToServer(it.toString()) }
        }

        @UiThread
        override fun onPlaybackStateChanged(isPlaying: Boolean, position: Double) {
            val dto = PlaybackStateDto(isPlaying, position)
            dto.toJSON()?.let { sendToServer(it.toString()) }
        }

        @UiThread
        override fun onMetadataChanged(title: String, artist: String, album: String, duration: Double) {
            val dto = MetadataDto(title, artist, album, duration)
            dto.toJSON()?.let { sendToServer(it.toString()) }
        }

        @UiThread
        override fun onServiceLifetime(text: String) {
            appendLog("Service: $text")
        }
    }

    /**
     * Append text to log, send text to server with retrying, show alert dialog when failed and switch on.
     */
    @UiThread
    private fun sendToServer(text: String, failAlert: Boolean = true) {
        appendLog(text)

        Thread {
            for (i in 0..2) { // retry twice
                if (SocketUtils.send(text)) {
                    return@Thread
                }
            }
            if (failAlert) {
                runOnUiThread {
                    if (!SHOWING_DISCONNECT_DIALOG) {
                        SHOWING_DISCONNECT_DIALOG = true
                        showAlertDialog("服务", "与 PC 端的连接已断开。", false) { _, _ -> SHOWING_DISCONNECT_DIALOG = false }
                    }
                    processStopService()
                }
            }
        }.start()
    }

    // ================
    // other actions...
    // ================

    @UiThread
    private fun updateManually() {
        MainService.mediaController?.let { it1 ->
            MainService.mediaCallback?.let { it2 ->
                val state = it1.playbackState
                val metadata = it1.metadata
                if (state != null && metadata != null) {
                    it2.onMetadataChanged(metadata)
                    it2.onPlaybackStateChanged(state)
                }
            }
        }
    }

    class NotificationIntentService : IntentService("NcmlAtwIntentService") {
        override fun onHandleIntent(intent: Intent?) {
            MainService.mediaController?.let { it1 ->
                MainService.mediaCallback?.let { it2 ->
                    val state = it1.playbackState
                    val metadata = it1.metadata
                    if (state != null && metadata != null) {
                        it2.onMetadataChanged(metadata)
                        it2.onPlaybackStateChanged(state)
                    }
                }
            }
        }
    }

    @UiThread
    private fun scanQrcode() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.CAMERA), REQUEST_CAMERA_PERMISSION_CODE)
            return
        }

        QRCodeManager.getInstance()
            .with(this)
            .scanningQRCode(object : OnQRCodeScanCallback {
                override fun onCompleted(result: String?) {
                    if (result == null) {
                        return
                    }
                    if (!result.startsWith(QRCODE_MAGIC)) {
                        showAlertDialog("扫描二维码", "不支持的二维码。")
                        return
                    }
                    val addrSp = result.substring(QRCODE_MAGIC.length).split(":")
                    if (addrSp.size != 2) {
                        showAlertDialog("扫描二维码", "不支持的二维码。")
                        return
                    }

                    val ip = addrSp[0]
                    val portStr = addrSp[1]
                    if (!SocketUtils.checkIPString(ip) || !SocketUtils.checkPortString(portStr)) {
                        showAlertDialog("扫描二维码", "二维码包含格式错误的 IP 地址和端口。")
                        return
                    }

                    edt_ip.setText(ip)
                    edt_port.setText(portStr)
                }

                override fun onError(error: Throwable?) {
                    showAlertDialog("扫描二维码", "扫描二维码失败。")
                }

                override fun onCancel() {}
            })
    }

    // =========
    // others...
    // =========

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        QRCodeManager.getInstance().with(this).onActivityResult(requestCode, resultCode, data)
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        when (requestCode) {
            REQUEST_CAMERA_PERMISSION_CODE -> {
                if (grantResults[0] != PackageManager.PERMISSION_GRANTED) {
                    showAlertDialog("授权", "相机授权失败。")
                } else {
                    scanQrcode()
                }
            }
        }
    }

    @UiThread
    private fun showAlertDialog(title: String, message: String, cancelable: Boolean = true, okCallback: ((DialogInterface, Int) -> Unit)? = null) {
        AlertDialog.Builder(this)
            .setTitle(title)
            .setMessage(message)
            .setCancelable(cancelable)
            .setPositiveButton("确定", okCallback)
            .show()
    }

    @UiThread
    private fun clearLog() {
        tv_log.text = ""
    }

    @UiThread
    private fun appendLog(text: String) {
        val fmt = SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.CHINA)
        val now = fmt.format(Calendar.getInstance().time)
        tv_log.append("\n[$now] $text")
        val scrollAmount = tv_log.layout.getLineTop(tv_log.lineCount) - tv_log.height
        if (scrollAmount > 0) {
            tv_log.scrollTo(0, scrollAmount)
        } else {
            tv_log.scrollTo(0, 0)
        }
    }

    private fun getDataFromSP(name: String): Array<String> {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        val out = arrayListOf<String>()
        for (entry in sp.all.entries) {
            out.add(entry.value.toString())
        }
        return out.toArray(arrayOf<String>())
    }

    private fun saveDataToSP(name: String, value: String): Boolean {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        if (!getDataFromSP(name).contains(value)) {
            val fmt = SimpleDateFormat("yyyyMMddHHmmss", Locale.CHINA)
            val token = fmt.format(Calendar.getInstance().time)
            val edt = sp.edit()
            edt.putString(token, value)
            edt.apply()
            return true
        }
        return false
    }
}
