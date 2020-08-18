package com.aoihosizora.neteasem2dclient.model;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;

public class MetadataDto implements Serializable {

    private String title;
    private String artist;
    private String album;
    private double duration;

    public MetadataDto(String title, String artist, String album, double duration) {
        this.title = title;
        this.artist = artist;
        this.album = album;
        this.duration = duration;
    }

    public JSONObject toJson() {
        JSONObject obj = new JSONObject();
        try {
            obj.put("title", title);
            obj.put("artist", artist);
            obj.put("album", album);
            obj.put("duration", duration);
            return obj;
        } catch (JSONException ex) {
            ex.printStackTrace();
            return null;
        }
    }
}