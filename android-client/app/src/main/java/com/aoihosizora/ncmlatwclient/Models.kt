package com.aoihosizora.ncmlatwclient

import java.io.Serializable
import org.json.JSONException
import org.json.JSONObject

data class PlaybackStateDto(
    val isPlaying: Boolean,
    val currentPosition: Double // second
) : Serializable {
    fun toJSON(): JSONObject? {
        val obj = JSONObject()
        try {
            obj.put("isPlaying", isPlaying)
            obj.put("currentPosition", currentPosition)
        } catch (ex: JSONException) {
            return null
        }
        return obj
    }
}

data class MetadataDto(
    val title: String,
    val artist: String,
    val album: String,
    val duration: Double // second
) : Serializable {
    fun toJSON(): JSONObject? {
        val obj = JSONObject()
        try {
            obj.put("title", title)
            obj.put("artist", artist)
            obj.put("album", album)
            obj.put("duration", duration)
        } catch (ex: JSONException) {
            return null
        }
        return obj
    }
}

data class DestroyedDto(
    val isDestroyed: Boolean // always true
) : Serializable {
    fun toJSON(): JSONObject? {
        val obj = JSONObject()
        try {
            obj.put("isDestroyed", isDestroyed)
        } catch (ex: JSONException) {
            return null
        }
        return obj
    }
}
