package com.aoihosizora.neteasem2dclient.model;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.Serializable;

public class DestroyDto implements Serializable {

    private boolean isDestroyed;

    public DestroyDto(boolean isDestroyed) {
        this.isDestroyed = isDestroyed;
    }

    public JSONObject toJson() {
        JSONObject obj = new JSONObject();
        try {
            obj.put("isDestroyed", isDestroyed);
            return obj;
        } catch (JSONException ex) {
            ex.printStackTrace();
            return null;
        }
    }
}
