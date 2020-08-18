package com.aoihosizora.neteasem2dclient;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.media.MediaMetadata;
import android.media.session.MediaController;
import android.media.session.MediaSessionManager;
import android.media.session.PlaybackState;
import android.os.Bundle;
import android.os.IBinder;
import android.service.notification.NotificationListenerService;
import android.support.annotation.Nullable;
import android.support.annotation.UiThread;
import android.widget.Toast;

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
     * 媒体控制器，防止被 onDestroy 注销
     */
    public static MediaController mediaController = null;

    /**
     * 媒体回调
     */
    public static MediaCallBack mediaCallBack = null;

    @Override
    public void onCreate() {
        super.onCreate();

        // 初始化回调
        if (mediaCallBack == null) {
            mediaCallBack = new MediaCallBack();
        }

        // 初始化控制器
        if (mediaController != null) {
            return;
        }

        try {
            // 获得 MediaSessionManager，检查是否有音乐服务
            MediaSessionManager manager = (MediaSessionManager) getSystemService(Context.MEDIA_SESSION_SERVICE);
            if (manager == null) {
                if (mainEvent != null) {
                    mainEvent.onNoSession();
                }
                return;
            }

            // 检查是否是网易云
            List<MediaController> controllers = manager.getActiveSessions(new ComponentName(this, MainService.class));
            for (MediaController controller : controllers) {
                String packageName = controller.getPackageName();
                // if (packageName.contains("netease") && packageName.contains("music")) {
                if ((packageName.contains("netease") && packageName.contains("music")) || packageName.contains("xiami")) {
                    Toast.makeText(this, packageName, Toast.LENGTH_SHORT).show();
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

            // 一连接上就返回状态
            // TODO 网易云搞事情
            Bundle bundle = mediaController.getExtras();
            long flags = mediaController.getFlags();
            PlaybackState state = mediaController.getPlaybackState();
            MediaMetadata metadata = mediaController.getMetadata();

            Toast.makeText(this, bundle == null ? "bundle == null" : "bundle != null", Toast.LENGTH_SHORT).show();
            Toast.makeText(this, String.valueOf(flags), Toast.LENGTH_SHORT).show();

            Toast.makeText(this, mediaController.getPackageName(), Toast.LENGTH_SHORT).show();
            Toast.makeText(this, state == null ? "state == null" : "state != null", Toast.LENGTH_SHORT).show();

            if (state != null && metadata != null) {
                Toast.makeText(this, metadata.getText(MediaMetadata.METADATA_KEY_TITLE), Toast.LENGTH_SHORT).show();
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
        // 会自动 destroy，不能把注销 MediaController 放在这里
        super.onDestroy();
        // Toast.makeText(this, "onDestroy", Toast.LENGTH_SHORT).show();
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
            Toast.makeText(MainService.this, "onPlaybackStateChanged1", Toast.LENGTH_SHORT).show();
            if (state == null) {
                return;
            }
            Toast.makeText(MainService.this, "onPlaybackStateChanged2", Toast.LENGTH_SHORT).show();

            // 信息
            boolean isPlaying = state.getState() == PlaybackState.STATE_PLAYING;
            double musicPosition = (double) state.getPosition() / 1000.0;

            // dto
            PlaybackStateDto dto = new PlaybackStateDto(isPlaying, musicPosition);
            JSONObject json = dto.toJson();
            if (json == null) {
                return;
            }
            Toast.makeText(MainService.this, json.toString(), Toast.LENGTH_SHORT).show();

            // 发送
            new Thread(() -> {
                if (!SendService.sendMsg(json.toString()) && mainEvent != null) {
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
            Toast.makeText(MainService.this, "onMetadataChanged1", Toast.LENGTH_SHORT).show();
            if (metadata == null) {
                return;
            }
            Toast.makeText(MainService.this, "onMetadataChanged2", Toast.LENGTH_SHORT).show();

            // 信息
            String title = metadata.getString(MediaMetadata.METADATA_KEY_TITLE);
            String artist = metadata.getString(MediaMetadata.METADATA_KEY_ARTIST);
            String album = metadata.getString(MediaMetadata.METADATA_KEY_ALBUM);
            double duration = (double) metadata.getLong(MediaMetadata.METADATA_KEY_DURATION) / 1000.0;

            // dto
            MetadataDto dto = new MetadataDto(title, artist, album, duration);
            JSONObject json = dto.toJson();
            if (json == null) {
                return;
            }
            Toast.makeText(MainService.this, json.toString(), Toast.LENGTH_SHORT).show();

            // 发送
            new Thread(() -> {
                if (!SendService.sendMsg(json.toString()) && mainEvent != null) {
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
            Toast.makeText(MainService.this, "onSessionDestroyed1", Toast.LENGTH_SHORT).show();
            if (mainEvent != null) {
                mainEvent.onSessionDestroy();
            }
            Toast.makeText(MainService.this, "onSessionDestroyed2", Toast.LENGTH_SHORT).show();

            new Thread(() -> {
                if (!SendService.sendMsg("{\"isDestroyed\": \"true\"}") && mainEvent != null) {
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
