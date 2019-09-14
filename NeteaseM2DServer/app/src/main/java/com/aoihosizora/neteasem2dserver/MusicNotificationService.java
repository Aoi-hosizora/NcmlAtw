package com.aoihosizora.neteasem2dserver;

import android.media.AudioManager;
import android.media.MediaMetadataRetriever;
import android.media.RemoteController;
import android.service.notification.NotificationListenerService;
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
            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onClientChange(boolean clearing) { }

    @Override
    public void onClientTransportControlUpdate(int transportControlFlags) { }

    @Override
    public void onClientPlaybackStateUpdate(int state) { }

    @Override
    public void onClientPlaybackStateUpdate(int state, long stateChangeTimeMs, long currentPosMs, float speed) {
        /*
        state: 3, stateChangeTimeMs: 2256786083, currentPosMs: 80807 <- Play
        state: 2, stateChangeTimeMs: 2256797365, currentPosMs: 85887 <- Stop
         */
        String str = "state: " + state + "\ncurrentPosMs: " + currentPosMs / 1000.0;
        Toast.makeText(this, "onClientPlaybackStateUpdate: \n" + str, Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onClientMetadataUpdate(RemoteController.MetadataEditor metadataEditor) {
        try {
            String title = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_TITLE, "null");
            String artist = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ARTIST, "null");
            String album = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ALBUM, "null");
            long duration = metadataEditor.getLong(MediaMetadataRetriever.METADATA_KEY_DURATION, -1) / 1000;
            Toast.makeText(this, "title: " + title + "\nartist: " + artist + "\nalbum: " + album + "\nduration: " + duration, Toast.LENGTH_SHORT).show();
        }
        catch (Exception ex) {

            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    private String getExceptionStr(Exception ex) {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        ex.printStackTrace(pw);
        pw.flush();
        return sw.toString();
    }
}
