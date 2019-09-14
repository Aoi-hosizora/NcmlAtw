package com.aoihosizora.neteasem2dserver;

import android.app.Notification;
import android.app.PendingIntent;
import android.content.Context;
import android.graphics.Bitmap;
import android.media.AudioManager;
import android.media.MediaMetadataRetriever;
import android.media.RemoteController;
import android.os.Bundle;
import android.service.notification.NotificationListenerService;
import android.service.notification.StatusBarNotification;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.RemoteViews;
import android.widget.TextView;
import android.widget.Toast;

import java.io.PrintWriter;
import java.io.StringWriter;

public class MusicNotificationService extends NotificationListenerService implements RemoteController.OnClientUpdateListener {

    public static String TAG = "MusicNotificationService";

    @Override
    public void onCreate() {
        super.onCreate();

        try {
            RemoteController remoteController = new RemoteController(this, this);
            ((AudioManager) getSystemService(AUDIO_SERVICE)).registerRemoteController(remoteController);
            remoteController.setArtworkConfiguration(100, 100);
            remoteController.setSynchronizationMode(RemoteController.POSITION_SYNCHRONIZATION_CHECK);
        }
        catch (Exception ex) {
            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            ex.printStackTrace(pw);
            pw.flush();
            Toast.makeText(this, sw.toString().substring(sw.toString().length() - 500), Toast.LENGTH_SHORT).show();
        }
    }

    // @Override
    // public void onNotificationPosted(StatusBarNotification sbn) {
    //     if (sbn.getPackageName() != null &&
    //             sbn.getPackageName().contains("netease") && sbn.getPackageName().contains("music")) {

    //         Notification notification = sbn.getNotification();
    //         if (notification == null) return;

    //         try {
    //             RemoteViews rv = notification.contentView;
    //             // Toast.makeText(this, rv == null ? "rv == null" : "rv != null", Toast.LENGTH_SHORT).show();
    //             LayoutInflater inflater = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);

    //             Toast.makeText(this, "rv.getLayoutId(): " + rv.getLayoutId(), Toast.LENGTH_SHORT).show();
    //             ViewGroup localView = (ViewGroup) inflater.inflate(rv.getLayoutId(), null);
    //             Toast.makeText(this, "1: " + (localView == null ? "localView == null" : "localView != null"), Toast.LENGTH_SHORT).show();
    //             // rv.reapply(getApplicationContext(), localView);
    //             // Toast.makeText(this, "2: " + (localView == null ? "localView == null" : "localView != null"), Toast.LENGTH_SHORT).show();

    //             Bundle extras = notification.extras;
    //             // extra: hw_type = type_music
    //             Toast.makeText(this, extras == null ? "extras == null" : "extras != null", Toast.LENGTH_SHORT).show();
    //             String str = "";
    //             for (String key : extras.keySet()) {
    //                 str += key + ": " + extras.getString(key) + "\n";
    //             }

    //             Toast.makeText(this, str, Toast.LENGTH_SHORT).show();

    //         } catch (Exception ex) {
    //             StringWriter sw = new StringWriter();
    //             PrintWriter pw = new PrintWriter(sw);
    //             ex.printStackTrace(pw);
    //             pw.flush();
    //             Toast.makeText(this, sw.toString().substring(sw.toString().length() - 500), Toast.LENGTH_SHORT).show();
    //         }
    //     }
    // }

    // @Override
    // public void onNotificationRemoved(StatusBarNotification sbn) {
    //     if (sbn.getPackageName() != null &&
    //             sbn.getPackageName().contains("netease") && sbn.getPackageName().contains("music")) {
    //         Toast.makeText(this, "onNotificationRemoved", Toast.LENGTH_SHORT).show();
    //     }
    // }

    @Override
    public void onListenerConnected() {
        Toast.makeText(this, "onListenerConnected", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onListenerDisconnected() {
        Toast.makeText(this, "onListenerDisconnected", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onClientChange(boolean clearing) { }

    @Override
    public void onClientPlaybackStateUpdate(int state) { }

    @Override
    public void onClientPlaybackStateUpdate(int state, long stateChangeTimeMs, long currentPosMs, float speed) { }

    @Override
    public void onClientTransportControlUpdate(int transportControlFlags) { }

    @Override

    public void onClientMetadataUpdate(RemoteController.MetadataEditor metadataEditor) {
        try {
            String title = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_TITLE, "null");
            String artist = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ARTIST, "null");
            String album = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ALBUM, "null");
            Long duration = metadataEditor.getLong(MediaMetadataRetriever.METADATA_KEY_DURATION, -1);
            Toast.makeText(this, title + "\n" + artist + "\n" + album + "\n" + "\n" + duration, Toast.LENGTH_SHORT).show();
            // Toast.makeText(this, title, Toast.LENGTH_SHORT).show();
        }
        catch (Exception ex) {
            StringWriter sw = new StringWriter();
            PrintWriter pw = new PrintWriter(sw);
            ex.printStackTrace(pw);
            pw.flush();
            Toast.makeText(this, sw.toString().substring(sw.toString().length() - 500), Toast.LENGTH_SHORT).show();
        }

    }
}
