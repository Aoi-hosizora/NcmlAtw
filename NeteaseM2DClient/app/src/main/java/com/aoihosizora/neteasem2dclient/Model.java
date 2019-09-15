package com.aoihosizora.neteasem2dclient;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;

class Model {

    static class PlaybackState implements Serializable {

        boolean isPlay;
        double currentPosSecond;

        PlaybackState(boolean isPlay, double currentPosSecond) {
            this.isPlay = isPlay;
            this.currentPosSecond = currentPosSecond;
        }

        JSONObject toJson() {
            JSONObject obj = new JSONObject();
            try {
                obj.put("isPlay", isPlay);
                obj.put("currentPosSecond", currentPosSecond);
                return obj;
            }
            catch (JSONException ex) {
                ex.printStackTrace();
                return null;
            }
        }
    }

    static class Metadata implements Serializable {

        String title;
        String artist;
        String album;
        double duration;

        Metadata(String title, String artist, String album, double duration) {
            this.title = title;
            this.artist = artist;
            this.album = album;
            this.duration = duration;
        }

        JSONObject toJson() {
            JSONObject obj = new JSONObject();
            try {
                obj.put("title", title);
                obj.put("artist", artist);
                obj.put("album", album);
                obj.put("duration", duration);
                return obj;
            }
            catch (JSONException ex) {
                ex.printStackTrace();
                return null;
            }
        }
    }
}
