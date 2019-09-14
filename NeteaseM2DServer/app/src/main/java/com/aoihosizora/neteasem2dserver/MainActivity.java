package com.aoihosizora.neteasem2dserver;

import android.Manifest;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.ComponentName;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.annotation.UiThread;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.net.Socket;

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

        // PackageManager pm = getPackageManager();
        // pm.setComponentEnabledSetting(new ComponentName(this, MusicNotificationService.class),
        //         PackageManager.COMPONENT_ENABLED_STATE_DISABLED, PackageManager.DONT_KILL_APP);
        // pm.setComponentEnabledSetting(new ComponentName(this, MusicNotificationService.class),
        //         PackageManager.COMPONENT_ENABLED_STATE_ENABLED, PackageManager.DONT_KILL_APP);

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

    @BindView(R.id.id_edt_ip)
    TextView m_edt_ip;
    @BindView(R.id.id_edt_port)
    TextView m_edt_port;


    @OnClick(R.id.id_btn_server)
    @UiThread
    void on_btn_server_clicked(View view) {
        if (m_btn_server.getText().equals(getString(R.string.btn_start))) {
            final String ip_str = m_edt_ip.getText().toString();
            final String port_str = m_edt_port.getText().toString();

            String ip_re = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            String port_re = "([0-9]|[1-9]\\d{1,3}|[1-5]\\d{4}|6[0-4]\\d{4}|65[0-4]\\d{2}|655[0-2]\\d|6553[0-5])";

            if (!ip_str.matches(ip_re))
                showAlert(getString(R.string.alert_ip_format));
            else if (!port_str.matches(port_re))
                showAlert(getString(R.string.alert_port_format));
            else
                connect();
        }
        else {
            new Thread(new Runnable() {
                @Override
                public void run() {
                    disconnect();
                }
            }).start();
        }
    }

    public static int REQUEST_NETWORK_PERMISSION_CODE = 1;

    /**
     * 格式正确，连接
     */
    @UiThread
    private void connect() {

        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[] {
                    Manifest.permission.INTERNET
            }, REQUEST_NETWORK_PERMISSION_CODE);
        }

        final ProgressDialog progressDialog = new ProgressDialog(this);
        progressDialog.setMessage(getString(R.string.progress_linking));
        progressDialog.setCancelable(false);
        progressDialog.show();

        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    ClientSendUtil.socket = new Socket(m_edt_ip.getText().toString(), Integer.parseInt(m_edt_port.getText().toString()));

                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            progressDialog.dismiss();
                            Toast.makeText(MainActivity.this, R.string.tst_connect_success, Toast.LENGTH_SHORT).show();
                            m_btn_server.setText(getString(R.string.btn_stop));
                            m_edt_ip.setEnabled(false);
                            m_edt_port.setEnabled(false);

                            try {
                                stopService(ServiceIntent);
                            } catch (Exception ex) { }
                            startService(ServiceIntent);
                        }
                    });

                } catch (Exception ex) {
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            progressDialog.dismiss();
                            showAlert(getString(R.string.alert_connect_failed));
                            m_btn_server.setText(getString(R.string.btn_start));
                            m_edt_ip.setEnabled(true);
                            m_edt_port.setEnabled(true);
                        }
                    });
                    ex.printStackTrace();
                }
            }
        }).start();
    }

    /**
     * 断开连接
     */
    @UiThread
    private void disconnect() {

        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            m_btn_server.setText(getString(R.string.btn_start));
                            stopService(ServiceIntent);
                        }
                    });
                    ClientSendUtil.socket.close();
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            m_btn_server.setText(getString(R.string.btn_start));
                            m_edt_ip.setEnabled(true);
                            m_edt_port.setEnabled(true);
                        }
                    });
                }
                catch (Exception ex) { }
            }
        }).start();
    }

    @Override
    protected void onDestroy() {
        stopService(ServiceIntent);
        super.onDestroy();
    }

    private void showAlert(String msg) {
        new AlertDialog.Builder(this)
                .setTitle(R.string.alert_title)
                .setMessage(msg)
                .setPositiveButton(R.string.alert_pos, null)
                .show();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == REQUEST_NETWORK_PERMISSION_CODE) {
            if (grantResults[0] != PackageManager.PERMISSION_GRANTED)
                Toast.makeText(this, R.string.tst_auth_failed, Toast.LENGTH_LONG).show();
        }
    }
}
