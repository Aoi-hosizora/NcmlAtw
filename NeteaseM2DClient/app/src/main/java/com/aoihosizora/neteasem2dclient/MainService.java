package com.aoihosizora.neteasem2dclient;

import android.content.Intent;
import android.media.AudioManager;
import android.media.MediaMetadataEditor;
import android.media.MediaMetadataRetriever;
import android.media.RemoteController;
import android.os.IBinder;
import android.service.notification.NotificationListenerService;
import android.support.annotation.UiThread;
import android.widget.Toast;

import org.json.JSONObject;

import java.io.PrintWriter;
import java.io.StringWriter;

public class MainService extends NotificationListenerService implements RemoteController.OnClientUpdateListener {

    // public static String TAG = "MainService";
    private static RemoteController remoteController = null;

    /**
     * 记录是否暂停服务 (待改)
     */
    private static boolean isStop = false;
    public AudioManager am;

    /**
     * 外部事件，没用广播
     */
    interface MainEvent {
        @UiThread
        void onDisConnect();
    }

    public static MainEvent m_MainEvent;

    @Override
    public void onCreate() {
        super.onCreate();
        am = ((AudioManager) getSystemService(AUDIO_SERVICE));
        isStop = false;
        try {
            if (remoteController == null) {
                remoteController = new RemoteController(this, this);
                am.registerRemoteController(remoteController);
                remoteController.setSynchronizationMode(RemoteController.POSITION_SYNCHRONIZATION_CHECK);
            }
        }
        catch (Exception ex) {
            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        // am.unregisterRemoteController(remoteController);
        isStop = true;
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onClientChange(boolean clearing) { }

    @Override
    public void onClientTransportControlUpdate(int transportControlFlags) { }

    @Override
    public void onClientPlaybackStateUpdate(int state) { }

    @Override
    @UiThread
    public void onClientPlaybackStateUpdate(int state, long stateChangeTimeMs, long currentPosMs, float speed) {
        /*
        state: 3, stateChangeTimeMs: 2256786083, currentPosMs: 80807 <- Play
        state: 2, stateChangeTimeMs: 2256797365, currentPosMs: 85887 <- Stop
         */
        if (isStop) return;
        final Model.PlaybackState playbackState = new Model.PlaybackState(state == 3, (double) currentPosMs / 1000.0);
        final JSONObject obj = playbackState.toJson();

        new Thread(new Runnable() {
            @Override
            public void run() {
                if (obj != null && !ClientSendUtil.sendMsg(obj.toString())) {
                    if (m_MainEvent != null)
                        m_MainEvent.onDisConnect();
                }
            }
        }).start();
    }

    @Override
    @UiThread
    public void onClientMetadataUpdate(RemoteController.MetadataEditor metadataEditor) {
        if (isStop) return;
        try {
            String title = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_TITLE, "null");
            String artist = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ARTIST, "null");
            String album = metadataEditor.getString(MediaMetadataRetriever.METADATA_KEY_ALBUM, "null");
            long duration = metadataEditor.getLong(MediaMetadataRetriever.METADATA_KEY_DURATION, -1);

            final Model.Metadata metadata = new Model.Metadata(title, artist, album, (double) duration / 1000.0);
            final JSONObject obj = metadata.toJson();

            new Thread(new Runnable() {
                @Override
                public void run() {
                    if (obj != null && !ClientSendUtil.sendMsg(obj.toString())) {
                        if (m_MainEvent != null)
                            m_MainEvent.onDisConnect();
                    }
                }
            }).start();
        }
        catch (Exception ex) {
            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @UiThread
    private String getExceptionStr(Exception ex) {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        ex.printStackTrace(pw);
        pw.flush();
        return sw.toString();
    }
}
