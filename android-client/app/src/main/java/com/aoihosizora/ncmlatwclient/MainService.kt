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
import androidx.annotation.WorkerThread
import java.io.PrintWriter
import java.io.StringWriter

class MainService : NotificationListenerService() {

    companion object {
        var mediaController: MediaController? = null
        var mediaCallback: MediaCallBack? = null
        var eventCallback: EventCallback? = null

        @Suppress("DEPRECATION")
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

    override fun onCreate() {
        super.onCreate()
        // Toast.makeText(this, "onCreate", Toast.LENGTH_SHORT).show()
        eventCallback?.onLog("onCreate")

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
                eventCallback?.onNoSession()
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
                eventCallback?.onNoSession()
                stopSelf()
                return
            }

            // register callback and get states
            mediaController!!.registerCallback(mediaCallback!!)
            val state = mediaController!!.playbackState
            val metadata = mediaController!!.metadata
            if (state != null && metadata != null) {
                mediaCallback!!.onMetadataChanged(metadata)
                mediaCallback!!.onPlaybackStateChanged(state)
            }
        } catch (ex: Exception) {
            // Toast.makeText(this, getExceptionString(ex), Toast.LENGTH_SHORT).show()
            stopSelf()
        }
    }

    override fun onDestroy() {
        // Toast.makeText(this, "onDestroy", Toast.LENGTH_SHORT).show()
        eventCallback?.onLog("onDestroy")
        super.onDestroy()
    }

    inner class MediaCallBack : MediaController.Callback() {
        override fun onPlaybackStateChanged(state: PlaybackState?) {
            super.onPlaybackStateChanged(state)
            if (state == null) {
                return
            }

            // information
            val isPlaying = state.state == PlaybackState.STATE_PLAYING
            val currPosition = state.position.toDouble() / 1000.0

            // marshal
            val dto = PlaybackStateDto(isPlaying, currPosition)
            val json = dto.toJSON() ?: return

            // send
            Thread {
                eventCallback?.onSend(json.toString())
            }.start()
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

            // marshal
            val dto = MetadataDto(title, artist, album, duration)
            val json = dto.toJSON() ?: return

            // send
            Thread {
                eventCallback?.onSend(json.toString())
            }.start()
        }

        override fun onSessionDestroyed() {
            super.onSessionDestroyed()
            eventCallback?.onSessionDestroyed()
            stopSelf()

            // marshal
            val dto = DestroyedDto(true)
            val json = dto.toJSON() ?: return

            // send
            Thread {
                eventCallback?.onSend(json.toString())
            }.start()
        }
    }

    interface EventCallback {
        /**
         * no ncm session callback
         */
        @UiThread
        fun onNoSession()

        /**
         * session destroyed callback
         */
        @UiThread
        fun onSessionDestroyed()

        /**
         * send text callback
         */
        @WorkerThread
        fun onSend(text: String, checkResult: Boolean = true)

        /**
         * do log callback
         */
        @UiThread
        fun onLog(text: String)
    }

    private fun getExceptionString(ex: Exception): String? {
        val sw = StringWriter()
        val pw = PrintWriter(sw)
        ex.printStackTrace(pw)
        pw.flush()
        return sw.toString()
    }
}
