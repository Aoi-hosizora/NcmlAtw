package com.aoihosizora.ncmlatwclient

import android.Manifest
import android.annotation.SuppressLint
import android.app.ProgressDialog
import android.content.ComponentName
import android.content.Context
import android.content.DialogInterface
import android.content.Intent
import android.content.pm.PackageManager
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.provider.Settings
import android.text.TextUtils
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.Toast
import androidx.annotation.UiThread
import androidx.annotation.WorkerThread
import androidx.appcompat.app.AlertDialog
import androidx.core.app.ActivityCompat
import com.jwsd.libzxing.OnQRCodeScanCallback
import com.jwsd.libzxing.QRCodeManager
import kotlinx.android.synthetic.main.activity_main.*
import java.text.SimpleDateFormat
import java.util.*

class MainActivity : AppCompatActivity() {

    companion object {
        private const val QRCODE_MAGIC = "NCMLATW-"
        private const val REQUEST_CAMERA_PERMISSION_CODE = 1
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
        adtIP = ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, getSavedSpData("ip"))
        adtPort = ArrayAdapter<String>(this, android.R.layout.simple_list_item_1, getSavedSpData("port"))
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
        MainService.eventCallback = EventCallback()

        edt_ip.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }
        edt_port.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }

        btn_qrcode.setOnClickListener { scanQrcode() }
        btn_manual.setOnClickListener { updateManually() }
        btn_clearLog.setOnClickListener { tv_log.text = "" }
        btn_service.setOnClickListener {
            if (btn_service.text == "启动服务") {
                startService()
            } else {
                stopService()
            }
        }
    }

    @UiThread
    private fun startService() {
        // check address
        val ip = edt_ip.text.toString()
        val portStr = edt_port.text.toString()
        if (!SendUtils.checkIPString(ip)) {
            showAlertDialog("启动服务", "IP 地址格式有误。")
            return
        }
        if (!SendUtils.checkPortString(portStr)) {
            showAlertDialog("启动服务", "无效的端口。")
            return
        }
        val port = Integer.parseInt(portStr)

        // show progress dlg
        var isCanceled = false
        @Suppress("DEPRECATION") val dlg = ProgressDialog(this)
        @Suppress("DEPRECATION") dlg.setMessage("正在连接...")
        dlg.setCancelable(true)
        dlg.setOnCancelListener { isCanceled = true }
        dlg.show()

        Thread {
            // connect
            SendUtils.ip = ip
            SendUtils.port = port
            val ok = SendUtils.ping()
            if (isCanceled) {
                return@Thread
            }

            // process
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
        // process
        processStopService()

        // disconnect
        Thread {
            val dto = DestroyedDto(true)
            val json = dto.toJSON() ?: return@Thread
            MainService.eventCallback?.onSend(json.toString(), checkResult = false)
        }.start()
    }

    override fun onDestroy() {
        processStopService()
        super.onDestroy()
    }

    /**
     * Process start service, includes: start service, update ui, save data to sp.
     */
    @UiThread
    private fun processStartService(ip: String, port: String) {
        // start service
        if (MainService.isRunning(this)) {
            stopService(Intent(this, MainService::class.java))
        }
        MainService.mediaCallback?.let { MainService.mediaController?.unregisterCallback(it) }
        MainService.mediaController = null
        MainService.mediaCallback = null
        if (!MainService.isRunning(this)) {
            startService(Intent(this, MainService::class.java))
        }

        // update ui
        btn_service.text = "停止服务"
        edt_ip.isEnabled = false
        edt_port.isEnabled = false
        btn_qrcode.isEnabled = false
        btn_manual.isEnabled = true

        // save sp and update adapter
        if (saveDataToSp("ip", ip)) {
            adtIP.add(ip)
            adtIP.notifyDataSetChanged()
        }
        if (saveDataToSp("port", port)) {
            adtPort.add(port)
            adtPort.notifyDataSetChanged()
        }
    }

    /**
     * Process stop service, includes: stop service, update ui.
     */
    @UiThread
    private fun processStopService() {
        // stop service
        if (MainService.isRunning(this)) {
            stopService(Intent(this, MainService::class.java))
            MainService.mediaCallback?.let { MainService.mediaController?.unregisterCallback(it) }
            MainService.mediaController = null
            MainService.mediaCallback = null
        }

        // update ui
        btn_service.text = "启动服务"
        edt_ip.isEnabled = true
        edt_port.isEnabled = true
        btn_qrcode.isEnabled = true
        btn_manual.isEnabled = false
    }

    inner class EventCallback : MainService.EventCallback {

        @UiThread
        override fun onNoSession() {
            showAlertDialog("启动服务", "没有打开网易云音乐 app。")
            processStopService()
        }

        @UiThread
        override fun onSessionDestroyed() {
            showAlertDialog("服务", "网易云音乐 app 已经关闭。")
            processStopService()
        }

        @SuppressLint("SetTextI18n")
        @WorkerThread
        override fun onSend(text: String, checkResult: Boolean) {
            runOnUiThread {
                appendLog(text)
            }

            if (!SendUtils.send(text) && checkResult) {
                // disconnected
                runOnUiThread {
                    showAlertDialog("服务", "连接已断开。")
                    processStopService()
                }
            }
        }

        @UiThread
        override fun onLog(text: String) {
            appendLog(text)
        }
    }

    @UiThread
    private fun updateManually() {
        MainService.mediaController?.let {
            val state = it.playbackState
            val metadata = it.metadata
            if (state != null && metadata != null) {
                MainService.mediaCallback?.onMetadataChanged(metadata)
                MainService.mediaCallback?.onPlaybackStateChanged(state)
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
                    if (!SendUtils.checkIPString(ip) || !SendUtils.checkPortString(portStr)) {
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

    /**
     * Append log with datetime, and add to the bottom of TextView.
     */
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

    private fun getSavedSpData(name: String): Array<String> {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        val out = arrayListOf<String>()
        for (entry in sp.all.entries) {
            out.add(entry.value.toString())
        }
        return out.toArray(arrayOf<String>())
    }

    private fun saveDataToSp(name: String, value: String): Boolean {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        if (!getSavedSpData(name).contains(value)) {
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
