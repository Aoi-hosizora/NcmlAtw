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

    /**
     * 外部事件，没用广播
     */
    interface MainEvent {

        /**
         * Socket 断开连接
         */
        @UiThread
        void onDisConnect();

        /**
         * 没有网易云会话
         */
        @UiThread
        void onNoSession();

        /**
         * 网易云会话关闭
         */
        @UiThread
        void onSessionDestroy();
    }

    public static MainEvent m_MainEvent;

    /**
     * 全局 媒体控制器，防止被 onDestroy 注销
     */
    public static MediaController mediaController = null;
    public static CallBack m_callBack = null;

    @Override
    public void onCreate() {
        super.onCreate();

        if (m_callBack == null)
            m_callBack = new CallBack();

        if (mediaController == null) {
            try {
                MediaSessionManager msMgr = (MediaSessionManager) getSystemService(Context.MEDIA_SESSION_SERVICE);
                List<MediaController> controllers = msMgr.getActiveSessions(new ComponentName(this, MainService.class));

                for (MediaController mc : controllers)
                    if (mc.getPackageName().contains("netease") && mc.getPackageName().contains("music"))
                        mediaController = mc;

                if (mediaController != null) {
                    mediaController.registerCallback(m_callBack);
                }
                else {
                    if (m_MainEvent != null)
                        m_MainEvent.onNoSession();
                }
            }
            catch (Exception ex) {
                Toast.makeText(this, getExceptionStr(ex), Toast.LENGTH_SHORT).show();
            }
        }
    }

    @Override
    public void onDestroy() {
        // 会自动 destroy，不能把注销 MediaController 放在这里
        super.onDestroy();
        // Toast.makeText(this, "onDestroy", Toast.LENGTH_SHORT).show();
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

            int musicState = state.getState();
            long pos = state.getPosition();

            final Model.PlaybackState m_playbackState = new Model.PlaybackState(
                    musicState == PlaybackState.STATE_PLAYING,
                    (double) pos / 1000.0
            );
            final JSONObject obj = m_playbackState.toJson();

            if (obj == null) return;

            // Toast.makeText(MainService.this, obj.toString(), Toast.LENGTH_SHORT).show();

            new Thread(new Runnable() {
                @Override
                public void run() {
                    if (!ClientSendUtil.sendMsg(obj.toString())) {
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

            if (obj == null) return;

            // Toast.makeText(MainService.this, obj.toString(), Toast.LENGTH_SHORT).show();

            new Thread(new Runnable() {
                @Override
                public void run() {
                    if (!ClientSendUtil.sendMsg(obj.toString())) {
                        if (m_MainEvent != null)
                            m_MainEvent.onDisConnect();
                    }
                }
            }).start();
        }

        @Override
        public void onSessionDestroyed() {
            super.onSessionDestroyed();
            // Toast.makeText(MainService.this, "onSessionDestroyed", Toast.LENGTH_SHORT).show();
            if (m_MainEvent != null)
                m_MainEvent.onSessionDestroy();
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
