package com.aoihosizora.neteasem2dserver;

import android.service.notification.NotificationListenerService;
import android.service.notification.StatusBarNotification;
import android.util.Log;
import android.widget.Toast;

public class MusicNotificationService extends NotificationListenerService {

    public static String TAG = "MusicNotificationService";

    @Override
    public void onNotificationPosted(StatusBarNotification sbn) {
        Toast.makeText(this, "onNotificationPosted", Toast.LENGTH_SHORT).show();
        // Toast.makeText(this, sbn.getPackageName(), Toast.LENGTH_SHORT).show();
        // Toast.makeText(this, sbn.getNotification().toString(), Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onListenerConnected() {
        Toast.makeText(this, "onListenerConnected", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onNotificationRemoved(StatusBarNotification sbn) {
        Toast.makeText(this, "onNotificationRemoved", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onListenerDisconnected() {
        Toast.makeText(this, "onListenerDisconnected", Toast.LENGTH_SHORT).show();
    }
}
