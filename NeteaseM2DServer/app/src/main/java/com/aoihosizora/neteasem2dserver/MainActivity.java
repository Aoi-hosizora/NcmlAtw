package com.aoihosizora.neteasem2dserver;

import android.Manifest;
import android.content.ComponentName;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class MainActivity extends AppCompatActivity {

    public static String TAG = "MainActivity";

    private Intent ServiceIntent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ButterKnife.bind(this);
        ServiceIntent = new Intent(this, MusicNotificationService.class);

        PackageManager pm = getPackageManager();
        pm.setComponentEnabledSetting(new ComponentName(this, MusicNotificationService.class),
                PackageManager.COMPONENT_ENABLED_STATE_DISABLED, PackageManager.DONT_KILL_APP);
        pm.setComponentEnabledSetting(new ComponentName(this, MusicNotificationService.class),
                PackageManager.COMPONENT_ENABLED_STATE_ENABLED, PackageManager.DONT_KILL_APP);

        if (!isNotificationListenersEnabled()) {
            Intent intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            startActivity(intent);
        }
    }

    /**
     * 通知へのアクセス
     * @return
     */
    public boolean isNotificationListenersEnabled() {
        String pkgName = getPackageName();
        final String flat = Settings.Secure.getString(getContentResolver(), "enabled_notification_listeners");
        if (!TextUtils.isEmpty(flat)) {
            final String[] names = flat.split(":");
            for (String name : names) {
                final ComponentName cn = ComponentName.unflattenFromString(name);
                if (cn != null && TextUtils.equals(pkgName, cn.getPackageName()))
                    return true;
            }
        }
        return false;
    }

    @BindView(R.id.id_btn_server)
    Button m_btn_server;

    @OnClick(R.id.id_btn_server)
    void on_btn_server_clicked(View view) {
        if (m_btn_server.getText().equals(getString(R.string.btn_start))) {
            m_btn_server.setText(getString(R.string.btn_stop));
            try {
                stopService(ServiceIntent);
            }
            catch (Exception ex) { }
            startService(ServiceIntent);
        } else {
            m_btn_server.setText(getString(R.string.btn_start));
            stopService(ServiceIntent);
        }
    }

    @Override
    protected void onDestroy() {
        stopService(ServiceIntent);
        super.onDestroy();
    }
}
