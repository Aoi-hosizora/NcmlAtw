package com.aoihosizora.neteasem2dclient;

import android.Manifest;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.ComponentName;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.annotation.UiThread;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class MainActivity extends AppCompatActivity {

    // public static String TAG = "MainActivity";

    public static int REQUEST_NETWORK_PERMISSION_CODE = 1;

    private Intent ServiceIntent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ButterKnife.bind(this);
        ServiceIntent = new Intent(this, MainService.class);

        // 服务事件
        MainService.m_MainEvent = new MainService.MainEvent() {

            @Override
            @UiThread
            public void onDisConnect() {
                showAlert(getString(R.string.alert_break_connect));
                on_ui_stop();
                on_controller_stop();
            }

            @Override
            @UiThread
            public void onNoSession() {
                showAlert(getString(R.string.alert_no_session));
                on_ui_stop();
                on_controller_stop();
            }

            @Override
            @UiThread
            public void onSessionDestroy() {
                showAlert(getString(R.string.alert_session_destroy));
                on_ui_stop();
                on_controller_stop();
            }
        };

        // 访问权限
        if (!isNotificationListenersEnabled()) {
            showAlert(getString(R.string.alert_check_per), new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {
                    Intent intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
                    intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    startActivity(intent);
                }
            });
        }
    }

    /**
     * 通知へのアクセス
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

    @BindView(R.id.id_btn_service)
    Button m_btn_service;

    @BindView(R.id.id_edt_ip)
    TextView m_edt_ip;
    @BindView(R.id.id_edt_port)
    TextView m_edt_port;

    @OnClick(R.id.id_btn_service)
    @UiThread
    void on_btn_service_clicked() {
        if (m_btn_service.getText().equals(getString(R.string.btn_start))) {
            // 开启服务
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
            // 断开服务
            disconnect();
        }
    }

    /**
     * 格式正确，连接
     */
    @UiThread
    private void connect() {

        // 判断网路权限
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
                    ClientSendUtil.ip = m_edt_ip.getText().toString();
                    ClientSendUtil.port = Integer.parseInt(m_edt_port.getText().toString());
                    if (!ClientSendUtil.ping()) {

                        // 连接不通
                        runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                progressDialog.dismiss();
                                showAlert(getString(R.string.alert_connect_failed));
                                on_ui_stop();
                            }
                        });
                        return;
                    }

                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            progressDialog.dismiss();
                            Toast.makeText(MainActivity.this, R.string.tst_connect_success, Toast.LENGTH_SHORT).show();

                            try {
                                on_controller_stop();
                            } catch (Exception ex) {
                                ex.printStackTrace();
                            }
                            startService(ServiceIntent);
                            on_ui_starting();
                        }
                    });

            }
        }).start();
    }

    /**
     * 断开连接
     */
    @UiThread
    private void disconnect() {
        on_ui_stop();
        on_controller_stop();
    }

    /**
     * 连接界面更新
     */
    @UiThread
    private void on_ui_starting() {
        m_btn_service.setText(getString(R.string.btn_stop));
        m_edt_ip.setEnabled(false);
        m_edt_port.setEnabled(false);
    }

    /**
     * 断开连接界面更新
     */
    @UiThread
    private void on_ui_stop() {
        m_btn_service.setText(getString(R.string.btn_start));
        m_edt_ip.setEnabled(true);
        m_edt_port.setEnabled(true);
    }

    /**
     * 主动注销通知，(不能写在 onDestroy)
     */
    private void on_controller_stop() {
        if (MainService.mediaController != null)
            MainService.mediaController.unregisterCallback(MainService.m_callBack);
        MainService.mediaController = null;
        MainService.m_callBack = null;

        stopService(ServiceIntent);
    }

    @Override
    protected void onDestroy() {
        try {
            on_controller_stop();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
        super.onDestroy();
    }

    private void showAlert(String msg) {
        showAlert(msg, null);
    }

    private void showAlert(String msg, DialogInterface.OnClickListener listener) {
        new AlertDialog.Builder(this)
                .setTitle(R.string.alert_title)
                .setMessage(msg)
                .setPositiveButton(R.string.alert_pos, listener)
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
