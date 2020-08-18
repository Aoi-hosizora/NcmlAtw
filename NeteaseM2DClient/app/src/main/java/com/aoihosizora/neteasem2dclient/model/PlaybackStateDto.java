package com.aoihosizora.neteasem2dclient.model;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;

public class PlaybackStateDto implements Serializable {

    private boolean isPlay;
    private double currentPosSecond;

    public PlaybackStateDto(boolean isPlay, double currentPosSecond) {
        this.isPlay = isPlay;
        this.currentPosSecond = currentPosSecond;
    }

    public JSONObject toJson() {
        JSONObject obj = new JSONObject();
        try {
            obj.put("isPlay", isPlay);
            obj.put("currentPosSecond", currentPosSecond);
            return obj;
        } catch (JSONException ex) {
            ex.printStackTrace();
            return null;
        }
    }
}