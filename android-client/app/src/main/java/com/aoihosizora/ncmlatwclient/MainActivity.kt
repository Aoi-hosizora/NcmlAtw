package com.aoihosizora.ncmlatwclient

import android.Manifest
import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.provider.Settings
import android.text.TextUtils
import android.util.Patterns
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.Toast
import androidx.core.app.ActivityCompat
import kotlinx.android.synthetic.main.activity_main.*
import java.text.SimpleDateFormat
import java.util.*

class MainActivity : AppCompatActivity() {

    companion object {
        const val REQUEST_NETWORK_PERMISSION_CODE = 1
    }

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
            val intent = Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS")
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            startActivity(intent)
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
    }

    private fun startService() {
        // check address
        val ip = edt_ip.text.toString()
        val portStr = edt_port.text.toString()
        val port: Int
        if (!Patterns.IP_ADDRESS.matcher(ip).matches()) {
            Toast.makeText(this, "IP 地址格式有误", Toast.LENGTH_SHORT).show()
            return
        }
        try {
            port = Integer.parseInt(portStr)
        } catch (ex: Exception) {
            Toast.makeText(this, "端口格式有误", Toast.LENGTH_SHORT).show()
            return
        }
        if (port < 1 || port > 65535) {
            Toast.makeText(this, "无效端口", Toast.LENGTH_SHORT).show()
            return
        }

        // check network permission
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, arrayOf(Manifest.permission.INTERNET), REQUEST_NETWORK_PERMISSION_CODE)
            return
        }

        // connect
        Toast.makeText(this, "connected", Toast.LENGTH_SHORT).show()

        // start service
        // startService(Intent(this, MainService::class.java))

        // update ui
        btn_service.text = "停止服务"
        edt_ip.isEnabled = false
        edt_port.isEnabled = false

        // save sp and update adapter
        saveDataToSp("ip", ip)
        saveDataToSp("port", portStr)
        adtIP.add(ip)
        adtPort.add(portStr)
        adtIP.notifyDataSetChanged()
        adtPort.notifyDataSetChanged()
    }

    private fun stopService() {
        // disconnect
        Toast.makeText(this, "disconnected", Toast.LENGTH_SHORT).show()

        // stop service
        // stopService(Intent(this, MainService::class.java))

        // update ui
        btn_service.text = "启动服务"
        edt_ip.isEnabled = true
        edt_port.isEnabled = true
    }

    private fun getSavedSpData(name: String): Array<String> {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        val out = arrayListOf<String>()
        for (entry in sp.all.entries) {
            out.add(entry.value.toString())
        }
        return out.toArray(arrayOf<String>())
    }

    private fun saveDataToSp(name: String, value: String) {
        val sp = getSharedPreferences(name, Context.MODE_PRIVATE)
        if (!getSavedSpData(name).contains(value)) {
            val fmt = SimpleDateFormat("yyyyMMddHHmmss", Locale.CHINA)
            val token = fmt.format(Date(System.currentTimeMillis()))
            val edt = sp.edit()
            edt.putString(token, value)
            edt.apply()
        }
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
        when (requestCode) {
            REQUEST_NETWORK_PERMISSION_CODE -> {
                if (grantResults[0] != PackageManager.PERMISSION_GRANTED) {
                    Toast.makeText(this, "网络访问授权失败", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
