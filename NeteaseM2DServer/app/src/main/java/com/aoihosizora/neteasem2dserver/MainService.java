package com.aoihosizora.neteasem2dserver;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.media.AudioManager;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;

public class MainService extends Service {

    private static final String TAG = "MainService";

    private AudioManager m_Am;
    public static MyOnAudioFocusChangeListener m_Listener;

    @Override
    public void onCreate() {
        m_Am = (AudioManager) getApplicationContext().getSystemService(Context.AUDIO_SERVICE);
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onStart(Intent intent, int startid) {
        Toast.makeText(this, "My Service Start", Toast.LENGTH_LONG).show();
        Log.i(TAG, "onStart");

        // Request audio focus for playback
        int result = m_Am.requestAudioFocus(m_Listener, AudioManager.STREAM_MUSIC, AudioManager.AUDIOFOCUS_GAIN);

        if (result == AudioManager.AUDIOFOCUS_REQUEST_GRANTED) {
            Toast.makeText(this, "requestAudioFocus successfully.", Toast.LENGTH_SHORT).show();
        }
        else {
            Toast.makeText(this, "requestAudioFocus failed.", Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    public void onDestroy() {
        Toast.makeText(this, "My Service Stop", Toast.LENGTH_LONG).show();

        m_Am.abandonAudioFocus(m_Listener);
    }

    private class MyOnAudioFocusChangeListener implements AudioManager.OnAudioFocusChangeListener {

        @Override
        public void onAudioFocusChange(int focusChange) {
            // -1 Play
            // 1 Stop

            Toast.makeText(MainService.this, "focusChange=" + focusChange, Toast.LENGTH_SHORT).show();
        }
    }
}
