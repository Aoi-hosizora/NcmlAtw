package com.aoihosizora.ncmlatwclient

import android.app.Activity
import android.app.ActivityManager
import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.media.MediaMetadata
import android.media.session.MediaController
import android.media.session.MediaSessionManager
import android.media.session.PlaybackState
import android.service.notification.NotificationListenerService
import androidx.annotation.UiThread

class MainService : NotificationListenerService() {

    companion object {
        var mediaController: MediaController? = null
        var mediaCallback: MediaCallBack? = null
        var eventListener: EventListener? = null

        fun isRunning(a: Activity): Boolean {
            val manager = a.getSystemService(Context.ACTIVITY_SERVICE) as ActivityManager
            for (service in manager.getRunningServices(Int.MAX_VALUE)) {
                if (service.service.className == MainService::class.java.name) {
                    return true
                }
            }
            return false
        }
    }

    // ========================
    // service lifetime related
    // ========================

    override fun onCreate() {
        super.onCreate()
        eventListener?.onServiceLifetime("onCreate")

        if (mediaController != null) {
            stopSelf()
            return
        }
        if (mediaCallback == null) {
            mediaCallback = MediaCallBack()
        }

        try {
            // get MediaSessionManager
            val manager = getSystemService(Context.MEDIA_SESSION_SERVICE) as? MediaSessionManager
            if (manager == null) {
                eventListener?.onNoSession()
                stopSelf()
                return
            }
            // find ncm MediaController
            for (controller in manager.getActiveSessions(ComponentName(this, MainService::class.java))) {
                val pkgName = controller.packageName
                if (pkgName.contains("netease") && pkgName.contains("music")) {
                    mediaController = controller
                    break
                }
            }
            if (mediaController == null) {
                eventListener?.onNoSession()
                stopSelf()
                return
            }

            // register callback
            mediaController!!.registerCallback(mediaCallback!!)
        } catch (ex: Exception) {
            // Toast.makeText(this, getExceptionString(ex), Toast.LENGTH_SHORT).show()
            stopSelf()
        }

        // get current states
        val state = mediaController!!.playbackState
        val metadata = mediaController!!.metadata
        if (state != null && metadata != null) {
            mediaCallback!!.onMetadataChanged(metadata)
            mediaCallback!!.onPlaybackStateChanged(state)
        }
    }

    override fun onDestroy() {
        eventListener?.onServiceLifetime("onDestroy")
        super.onDestroy()
    }

    // override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
    //     intent?.let { it ->
    //         if (it.action == "UPDATE") {
    //             mediaController?.let { it1 ->
    //                 val state = it1.playbackState
    //                 val metadata = it1.metadata
    //                 if (state != null && metadata != null) {
    //                     val isPlaying = state.state == PlaybackState.STATE_PLAYING
    //                     val position = state.position.toDouble() / 1000.0
    //                     val title = metadata.getString(MediaMetadata.METADATA_KEY_TITLE)
    //                     val artist = metadata.getString(MediaMetadata.METADATA_KEY_ARTIST)
    //                     val album = metadata.getString(MediaMetadata.METADATA_KEY_ALBUM)
    //                     val duration = metadata.getLong(MediaMetadata.METADATA_KEY_DURATION).toDouble() / 1000.0
    //                     Thread {
    //                         PlaybackStateDto(isPlaying, position).toJSON()?.let { SocketUtils.send(it.toString()) }
    //                         MetadataDto(title, artist, album, duration).toJSON()?.let { SocketUtils.send(it.toString()) }
    //                     }.start()
    //                 }
    //             }
    //         }
    //     }
    //     return START_STICKY
    // }

    // ======================
    // media callback related
    // ======================

    inner class MediaCallBack : MediaController.Callback() {
        override fun onPlaybackStateChanged(state: PlaybackState?) {
            super.onPlaybackStateChanged(state)
            if (state == null) {
                return
            }

            // information
            val isPlaying = state.state == PlaybackState.STATE_PLAYING
            val position = state.position.toDouble() / 1000.0
            eventListener?.onPlaybackStateChanged(isPlaying, position)
        }

        override fun onMetadataChanged(metadata: MediaMetadata?) {
            super.onMetadataChanged(metadata)
            if (metadata == null) {
                return
            }

            // information
            val title = metadata.getString(MediaMetadata.METADATA_KEY_TITLE)
            val artist = metadata.getString(MediaMetadata.METADATA_KEY_ARTIST)
            val album = metadata.getString(MediaMetadata.METADATA_KEY_ALBUM)
            val duration = metadata.getLong(MediaMetadata.METADATA_KEY_DURATION).toDouble() / 1000.0
            eventListener?.onMetadataChanged(title, artist, album, duration)
        }

        override fun onSessionDestroyed() {
            super.onSessionDestroyed()
            eventListener?.onSessionDestroyed()
            stopSelf()
        }
    }

    interface EventListener {
        /**
         * No ncm session callback.
         */
        @UiThread
        fun onNoSession()

        /**
         * Session destroyed callback.
         */
        @UiThread
        fun onSessionDestroyed()

        /**
         * Media playback state changed callback.
         */
        @UiThread
        fun onPlaybackStateChanged(isPlaying: Boolean, position: Double)

        /**
         * Media metadata changed callback.
         */
        @UiThread
        fun onMetadataChanged(title: String, artist: String, album: String, duration: Double)

        /**
         * Service created or destroyed callback.
         */
        @UiThread
        fun onServiceLifetime(text: String)
    }

    // private fun getExceptionString(ex: Exception): String? {
    //     val sw = StringWriter()
    //     val pw = PrintWriter(sw)
    //     ex.printStackTrace(pw)
    //     pw.flush()
    //     return sw.toString()
    // }
}
