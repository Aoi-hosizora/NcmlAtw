package com.aoihosizora.ncmlatwclient

import android.app.ProgressDialog
import android.content.ComponentName
import android.content.Context
import android.content.DialogInterface
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.provider.Settings
import android.text.TextUtils
import android.util.Patterns
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.Toast
import androidx.annotation.UiThread
import androidx.annotation.WorkerThread
import androidx.appcompat.app.AlertDialog
import kotlinx.android.synthetic.main.activity_main.*
import java.text.SimpleDateFormat
import java.util.*

class MainActivity : AppCompatActivity() {

    private lateinit var adtIP: ArrayAdapter<String>
    private lateinit var adtPort: ArrayAdapter<String>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        initUI()
        initPermission()
        initListener()
    }

    private fun initUI() {
        adtIP = ArrayAdapter(this, android.R.layout.simple_list_item_1, getSavedSpData("ip"))
        adtPort = ArrayAdapter(this, android.R.layout.simple_list_item_1, getSavedSpData("port"))
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

    private fun initListener() {
        edt_ip.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }
        edt_port.setOnClickListener { v -> (v as AutoCompleteTextView).showDropDown() }

        btn_service.setOnClickListener {
            if (btn_service.text == "启动服务") {
                startService()
            } else {
                stopService()
            }
        }

        MainService.eventCallback = EventCallback()
    }

    private fun startService() {
        // check address
        val ip = edt_ip.text.toString()
        val portStr = edt_port.text.toString()
        val port: Int
        if (!Patterns.IP_ADDRESS.matcher(ip).matches()) {
            showAlertDialog("启动服务", "IP 地址格式有误。")
            return
        }
        try {
            port = Integer.parseInt(portStr)
        } catch (ex: Exception) {
            showAlertDialog("启动服务", "端口格式有误。")
            return
        }
        if (port < 1 || port > 65535) {
            showAlertDialog("启动服务", "无效端口。")
            return
        }

        // show progress dlg
        var isCanceled = false
        @Suppress("DEPRECATION") val dlg = ProgressDialog(this)
        @Suppress("DEPRECATION") dlg.setMessage("正在连接中...")
        dlg.setCancelable(true)
        dlg.setOnCancelListener { isCanceled = true }
        dlg.show()

        Thread {
            // connect
            Thread.sleep(500)

            // canceled
            if (isCanceled) {
                return@Thread
            }

            // process
            runOnUiThread {
                dlg.dismiss()
                processStartService(ip, portStr)
            }
        }.start()
    }

    private fun stopService() {
        Thread {
            // process
            runOnUiThread {
                processStopService()
            }

            // disconnect
            val dto = DestroyedDto(true)
            val json = dto.toJSON() ?: return@Thread
            Toast.makeText(this, json.toString(), Toast.LENGTH_SHORT).show()
            MainService.eventCallback?.onSend(json.toString(), false)
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
        val intent = Intent(this, MainService::class.java)
        if (MainService.isRunning(this)) {
            stopService(intent)
            MainService.mediaCallback?.let { MainService.mediaController?.unregisterCallback(it) }
            MainService.mediaController = null
            MainService.mediaCallback = null
        }
        if (!MainService.isRunning(this)) {
            startService(intent)
        }

        // update ui
        btn_service.text = "停止服务"
        edt_ip.isEnabled = false
        edt_port.isEnabled = false

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

        @WorkerThread
        override fun onSend(text: String, checkResult: Boolean) {
            // TODO
        }
    }

    private fun showAlertDialog(title: String, message: String, cancelable: Boolean = true, okCallback: ((DialogInterface, Int) -> Unit)? = null) {
        AlertDialog.Builder(this)
            .setTitle(title)
            .setMessage(message)
            .setCancelable(cancelable)
            .setPositiveButton("确定", okCallback)
            .show()
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
            val token = fmt.format(Date(System.currentTimeMillis()))
            val edt = sp.edit()
            edt.putString(token, value)
            edt.apply()
            return true
        }
        return false
    }
}
