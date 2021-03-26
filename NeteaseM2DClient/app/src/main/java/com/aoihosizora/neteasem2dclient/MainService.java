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

import com.aoihosizora.neteasem2dclient.model.DestroyDto;
import com.aoihosizora.neteasem2dclient.model.MetadataDto;
import com.aoihosizora.neteasem2dclient.model.PlaybackStateDto;

import org.json.JSONObject;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.List;

public class MainService extends NotificationListenerService {

    /**
     * 外部事件，由 MainActivity 初始化
     */
    public static MainEvent mainEvent = null;

    /**
     * 媒体控制器
     */
    public static MediaController mediaController = null;

    /**
     * 媒体回调
     */
    public static MediaCallBack mediaCallBack = null;

    @Override
    public void onCreate() {
        super.onCreate();
        if (mediaController != null) {
            return;
        }
        if (mediaCallBack == null) {
            mediaCallBack = new MediaCallBack();
        }

        try {
            // 获得 MediaSessionManager
            MediaSessionManager manager = (MediaSessionManager) getSystemService(Context.MEDIA_SESSION_SERVICE);
            if (manager == null) {
                if (mainEvent != null) {
                    mainEvent.onNoSession();
                }
                return;
            }

            // 检查当前服务是否是网易云
            List<MediaController> controllers = manager.getActiveSessions(new ComponentName(this, MainService.class));
            for (MediaController controller : controllers) {
                String packageName = controller.getPackageName();
                if (packageName.contains("netease") && packageName.contains("music")) {
                    mediaController = controller;
                    break;
                }
            }
            if (mediaController == null) {
                if (mainEvent != null) {
                    mainEvent.onNoSession();
                }
                return;
            }

            // 注册回调
            mediaController.registerCallback(mediaCallBack);

            // 返回状态
            PlaybackState state = mediaController.getPlaybackState();
            MediaMetadata metadata = mediaController.getMetadata();
            if (state != null && metadata != null) {
                if (state.getState() == PlaybackState.STATE_PLAYING) {
                    mediaCallBack.onMetadataChanged(mediaController.getMetadata());
                    mediaCallBack.onPlaybackStateChanged(mediaController.getPlaybackState());
                }
            }
        } catch (Exception ex) {
            Toast.makeText(this, getExceptionString(ex), Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return super.onBind(intent);
    }

    /**
     * 重写媒体回调
     */
    class MediaCallBack extends MediaController.Callback {

        /**
         * 播放状态
         */
        @Override
        public void onPlaybackStateChanged(@Nullable PlaybackState state) {
            super.onPlaybackStateChanged(state);
            // Toast.makeText(MainService.this, "onPlaybackStateChanged", Toast.LENGTH_SHORT).show();
            if (state == null) {
                return;
            }

            // 信息
            boolean isPlaying = state.getState() == PlaybackState.STATE_PLAYING;
            double musicPosition = (double) state.getPosition() / 1000.0;

            // 序列化
            PlaybackStateDto dto = new PlaybackStateDto(isPlaying, musicPosition);
            JSONObject json = dto.toJson();
            if (json == null) {
                return;
            }
            Toast.makeText(MainService.this, json.toString(), Toast.LENGTH_SHORT).show();

            // 发送
            new Thread(() -> {
                boolean ok = SendUtils.send(json.toString());
                if (!ok && mainEvent != null) {
                    mainEvent.onDisconnect();
                }
            }).start();
        }

        /**
         * 媒体信息
         */
        @Override
        public void onMetadataChanged(@Nullable MediaMetadata metadata) {
            super.onMetadataChanged(metadata);
            // Toast.makeText(MainService.this, "onMetadataChanged", Toast.LENGTH_SHORT).show();
            if (metadata == null) {
                return;
            }

            // 信息
            String title = metadata.getString(MediaMetadata.METADATA_KEY_TITLE);
            String artist = metadata.getString(MediaMetadata.METADATA_KEY_ARTIST);
            String album = metadata.getString(MediaMetadata.METADATA_KEY_ALBUM);
            double duration = (double) metadata.getLong(MediaMetadata.METADATA_KEY_DURATION) / 1000.0;

            // 序列化
            MetadataDto dto = new MetadataDto(title, artist, album, duration);
            JSONObject json = dto.toJson();
            if (json == null) {
                return;
            }
            Toast.makeText(MainService.this, json.toString(), Toast.LENGTH_SHORT).show();

            // 发送
            new Thread(() -> {
                boolean ok = SendUtils.send(json.toString());
                if (!ok && mainEvent != null) {
                    mainEvent.onDisconnect();
                }
            }).start();
        }

        /**
         * 退出
         */
        @Override
        public void onSessionDestroyed() {
            super.onSessionDestroyed();
            Toast.makeText(MainService.this, "onSessionDestroyed", Toast.LENGTH_SHORT).show();
            if (mainEvent != null) {
                mainEvent.onSessionDestroy();
            }

            // 序列化
            DestroyDto dto = new DestroyDto(true);
            JSONObject json = dto.toJson();
            if (json == null) {
                return;
            }
            Toast.makeText(MainService.this, json.toString(), Toast.LENGTH_SHORT).show();

            // 发送
            new Thread(() -> {
                boolean ok = SendUtils.send(json.toString());
                if (!ok && mainEvent != null) {
                    mainEvent.onDisconnect();
                }
            }).start();
        }
    }

    /**
     * Debug 用
     */
    @UiThread
    private String getExceptionString(Exception ex) {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        ex.printStackTrace(pw);
        pw.flush();
        return sw.toString();
    }
}

/**
 * 外部事件，没有使用广播
 */
interface MainEvent {

    /**
     * Socket 断开连接
     */
    @UiThread
    void onDisconnect();

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
