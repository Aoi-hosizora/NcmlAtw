package com.aoihosizora.neteasem2dclient;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.media.MediaMetadata;
import android.media.session.MediaController;
import android.media.session.MediaSessionManager;
import android.media.session.PlaybackState;
import android.os.IBinder;
import android.service.notification.NotificationListenerService;
import android.support.annotation.Nullable;
import android.support.annotation.UiThread;
import android.widget.Toast;

import org.json.JSONObject;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.List;

public class MainService extends NotificationListenerService {

    private MediaController mediaController = null;
    private CallBack m_callBack;

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

        m_callBack = new CallBack();

        try {
            MediaSessionManager msMgr = (MediaSessionManager) getSystemService(Context.MEDIA_SESSION_SERVICE);
            List<MediaController> controllers = msMgr.getActiveSessions(new ComponentName(this, MainService.class));

            for (MediaController mediaController : controllers)
                if (mediaController.getPackageName().contains("netease") && mediaController.getPackageName().contains("music"))
                    this.mediaController = mediaController;

            if (mediaController != null) {
                mediaController.registerCallback(m_callBack);
            }
        }
        catch (Exception ex) {
            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        // Toast.makeText(this, "onDestroy", Toast.LENGTH_SHORT).show();
        try {
            mediaController.unregisterCallback(m_callBack);
        }
        catch (Exception ex) {
            Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public IBinder onBind(Intent intent) {
        return super.onBind(intent);
    }

    class CallBack extends MediaController.Callback {

        @Override
        public void onPlaybackStateChanged(@Nullable PlaybackState state) {
            super.onPlaybackStateChanged(state);
            if (state == null) return;

            /*
                state: 3, stateChangeTimeMs: 2256786083, currentPosMs: 80807 <- Play
                state: 2, stateChangeTimeMs: 2256797365, currentPosMs: 85887 <- Stop
             */

            final Model.PlaybackState m_playbackState = new Model.PlaybackState(
                    state.getState() == PlaybackState.STATE_PLAYING,
                    (double) state.getPosition() / 1000.0
            );
            final JSONObject obj = m_playbackState.toJson();

            // if (obj != null)
            //     Toast.makeText(MainService.this, obj.toString(), Toast.LENGTH_SHORT).show();

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
        public void onMetadataChanged(@Nullable MediaMetadata metadata) {
            super.onMetadataChanged(metadata);
            if (metadata == null) return;

            String title = metadata.getString(MediaMetadata.METADATA_KEY_TITLE);
            String artist = metadata.getString(MediaMetadata.METADATA_KEY_ARTIST);
            String album = metadata.getString(MediaMetadata.METADATA_KEY_ALBUM);
            long duration = metadata.getLong(MediaMetadata.METADATA_KEY_DURATION);

            final Model.Metadata m_metadata = new Model.Metadata(title, artist, album, (double) duration / 1000.0);
            final JSONObject obj = m_metadata.toJson();

            // if (obj != null)
            //     Toast.makeText(MainService.this, obj.toString(), Toast.LENGTH_SHORT).show();

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
