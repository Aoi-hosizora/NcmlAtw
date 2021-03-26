package com.aoihosizora.ncmlatwclient

import android.service.notification.NotificationListenerService
import android.widget.Toast

class MainService : NotificationListenerService() {

    override fun onCreate() {
        super.onCreate()
        Toast.makeText(this, "onCreate", Toast.LENGTH_SHORT).show()
    }

    override fun onDestroy() {
        Toast.makeText(this, "onDestroy", Toast.LENGTH_SHORT).show()
        super.onDestroy()
    }
}
