package com.aoihosizora.neteasem2dserver;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class MainActivity extends AppCompatActivity {

    public static String TAG = "MainActivity";
    public static int REQUEST_PERMISSION_CODE = 1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        ButterKnife.bind(this);

        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.BIND_NOTIFICATION_LISTENER_SERVICE) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[] {
                    Manifest.permission.BIND_NOTIFICATION_LISTENER_SERVICE
            }, REQUEST_PERMISSION_CODE);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == REQUEST_PERMISSION_CODE) {
            if (grantResults[0] != PackageManager.PERMISSION_GRANTED)
                Toast.makeText(this, "Grant permission failed", Toast.LENGTH_LONG).show();
        }
    }

    @BindView(R.id.id_btn_server)
    Button m_btn_server;

    @OnClick(R.id.id_btn_server)
    void on_btn_server_clicked(View view) {
        Intent intent = new Intent(this, MusicNotificationService.class);
        if (m_btn_server.getText().equals(getString(R.string.btn_start))) {
            m_btn_server.setText(getString(R.string.btn_stop));
            startService(intent);
        } else {
            m_btn_server.setText(getString(R.string.btn_start));
            stopService(intent);
        }
    }
}
