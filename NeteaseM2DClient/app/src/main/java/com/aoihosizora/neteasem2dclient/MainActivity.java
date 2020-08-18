package com.aoihosizora.neteasem2dclient;

import android.Manifest;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.ComponentName;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.media.MediaMetadata;
import android.media.session.PlaybackState;
import android.os.Bundle;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.annotation.UiThread;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.text.TextUtils;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.Toast;

import com.jwsd.libzxing.OnQRCodeScanCallback;
import com.jwsd.libzxing.QRCodeManager;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Locale;
import java.util.Map;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class MainActivity extends AppCompatActivity {

    public static final int REQUEST_NETWORK_PERMISSION_CODE = 1;
    public static final int REQUEST_CAMERA_PERMISSION_CODE = 2;

    public static final String QRCODE_MAGIC = "NETEASE_M2D://";

    private SharedPreferences ipSP, portSP;

    private Intent ServiceIntent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ButterKnife.bind(this);
        ServiceIntent = new Intent(this, MainService.class);

        // 服务事件
        MainService.mainEvent = new MainEvent() {

            @Override
            @UiThread
            public void onDisconnect() {
                showAlert(getString(R.string.alert_break_connect), null);
                on_ui_stop();
                on_controller_stop();
            }

            @Override
            @UiThread
            public void onNoSession() {
                showAlert(getString(R.string.alert_no_session), null);
                on_ui_stop();
                on_controller_stop();
            }

            @Override
            @UiThread
            public void onSessionDestroy() {
                showAlert(getString(R.string.alert_session_destroy), null);
                on_ui_stop();
                on_controller_stop();
            }
        };

        // 访问权限
        if (!isNotificationListenersEnabled()) {
            showAlert(getString(R.string.alert_check_per), (d, i) -> {
                Intent intent = new Intent("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
                intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(intent);
            });
        }

        ipSP = getSharedPreferences("ip", Context.MODE_PRIVATE);
        portSP = getSharedPreferences("port", Context.MODE_PRIVATE);

        // 自动完成输入框
        initAutoCompleteEdit();
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

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        switch (requestCode) {
            case REQUEST_NETWORK_PERMISSION_CODE:
                if (grantResults[0] != PackageManager.PERMISSION_GRANTED)
                    Toast.makeText(this, R.string.tst_auth_failed, Toast.LENGTH_LONG).show();
                break;
            case REQUEST_CAMERA_PERMISSION_CODE:
                if (grantResults[0] != PackageManager.PERMISSION_GRANTED)
                    Toast.makeText(this, R.string.tst_auth_failed, Toast.LENGTH_LONG).show();
                else
                    on_btn_qrCode_clicked();
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    @BindView(R.id.id_btn_service)
    Button m_btn_service;
    @BindView(R.id.id_btn_qrCode)
    Button m_btn_qrCode;
    @BindView(R.id.id_btn_update)
    Button m_btn_update;

    @BindView(R.id.id_edt_ip)
    AutoCompleteTextView m_edt_ip;
    @BindView(R.id.id_edt_port)
    AutoCompleteTextView m_edt_port;

    /**
     * 自动补全文本框 弹出
     */
    @OnClick({R.id.id_edt_ip, R.id.id_edt_port})
    void on_edt_auto_completed_clicked(View view) {
        ((AutoCompleteTextView) view).showDropDown();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        QRCodeManager.getInstance().with(this).onActivityResult(requestCode, resultCode, data);
    }

    /**
     * 扫描二维码
     */
    @OnClick(R.id.id_btn_qrCode)
    void on_btn_qrCode_clicked() {

        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{
                Manifest.permission.CAMERA,
                Manifest.permission.WRITE_EXTERNAL_STORAGE
            }, REQUEST_CAMERA_PERMISSION_CODE);
            return;
        }

        QRCodeManager.getInstance()
            .with(this)
            .scanningQRCode(new OnQRCodeScanCallback() {
                @Override
                public void onCompleted(String result) {
                    try {
                        // 无 MAGIC
                        if (!result.startsWith(QRCODE_MAGIC))
                            throw new Exception();
                        result = result.substring(QRCODE_MAGIC.length());
                        // 无端口
                        if (!result.contains(":"))
                            throw new Exception();

                        // 分割
                        String[] sp = result.split(":");
                        m_edt_ip.setText(sp[0]);
                        m_edt_port.setText(sp[1]);
                    } catch (Exception ex) {
                        ex.printStackTrace();
                        Toast.makeText(MainActivity.this, getString(R.string.tst_qr_error), Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onError(Throwable errorMsg) {
                    Toast.makeText(MainActivity.this, getString(R.string.tst_qr_failed), Toast.LENGTH_SHORT).show();
                }

                @Override
                public void onCancel() {
                }
            });
    }

    /**
     * 开始结束服务
     */
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
                showAlert(getString(R.string.alert_ip_format), null);
            else if (!port_str.matches(port_re))
                showAlert(getString(R.string.alert_port_format), null);
            else
                connect();
        } else {
            // 断开服务
            disconnect();
            new Thread(() -> {
                if (!SendService.sendMsg("{\"isDestroyed\": \"true\"}")) {
                    if (MainService.mainEvent != null)
                        MainService.mainEvent.onDisconnect();
                }
            }).start();
        }
    }

    /**
     * 手动更新
     */
    @OnClick(R.id.id_btn_update)
    void on_btn_update_clicked() {
        if (MainService.mediaController != null && MainService.mediaCallBack != null) {
            PlaybackState state = MainService.mediaController.getPlaybackState();
            MediaMetadata metadata = MainService.mediaController.getMetadata();

            if (state != null && metadata != null && state.getState() == PlaybackState.STATE_PLAYING) {
                MainService.mediaCallBack.onMetadataChanged(MainService.mediaController.getMetadata());
                MainService.mediaCallBack.onPlaybackStateChanged(MainService.mediaController.getPlaybackState());
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /**
     * 格式正确，连接
     */
    @UiThread
    private void connect() {
        // 判断网路权限
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{
                Manifest.permission.INTERNET
            }, REQUEST_NETWORK_PERMISSION_CODE);
        }

        final boolean[] isCanceled = new boolean[]{false};
        final ProgressDialog progressDialog = new ProgressDialog(this);
        progressDialog.setMessage(getString(R.string.progress_linking));
        progressDialog.setCancelable(true);
        progressDialog.setOnCancelListener((d) -> isCanceled[0] = true);
        progressDialog.show();

        new Thread(() -> {
            SendService.ip = m_edt_ip.getText().toString();
            SendService.port = Integer.parseInt(m_edt_port.getText().toString());

            if (!SendService.ping()) {

                if (isCanceled[0]) return;

                // 连接不通
                runOnUiThread(() -> {
                    progressDialog.dismiss();
                    showAlert(getString(R.string.alert_connect_failed), null);
                    on_ui_stop();
                });
                return;
            }

            if (isCanceled[0]) return;

            runOnUiThread(() -> {
                progressDialog.dismiss();
                Toast.makeText(MainActivity.this, R.string.tst_connect_success, Toast.LENGTH_SHORT).show();

                // 保存
                saveIP(SendService.ip);
                savePort(SendService.port);
                initAutoCompleteEdit();

                // 开始服务
                on_controller_stop();
                startService(ServiceIntent);
                on_ui_starting();
            });
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
        m_btn_qrCode.setEnabled(false);
        m_btn_update.setEnabled(true);
    }

    /**
     * 断开连接界面更新
     */
    @UiThread
    private void on_ui_stop() {
        m_btn_service.setText(getString(R.string.btn_start));
        m_edt_ip.setEnabled(true);
        m_edt_port.setEnabled(true);
        m_btn_qrCode.setEnabled(true);
        m_btn_update.setEnabled(false);
    }

    /**
     * 主动注销通知，(不能写在 onDestroy)
     */
    private void on_controller_stop() {
        if (MainService.mediaController != null)
            MainService.mediaController.unregisterCallback(MainService.mediaCallBack);

        MainService.mediaController = null;
        MainService.mediaCallBack = null;

        stopService(ServiceIntent);
    }

    @Override
    protected void onDestroy() {
        on_controller_stop();
        super.onDestroy();
    }

    private void showAlert(String msg, DialogInterface.OnClickListener listener) {
        new AlertDialog.Builder(this)
            .setTitle(R.string.alert_title)
            .setMessage(msg)
            .setPositiveButton(R.string.alert_pos, listener)
            .show();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /**
     * 获得当前时间
     *
     * @return yyyy-MM-dd-HH-mm-ss
     */
    private String getCurrentTime() {
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd-HH-mm-ss", Locale.CHINA);
        Date date = new Date(System.currentTimeMillis());
        return simpleDateFormat.format(date);
    }

    /**
     * 保存 IP 至 SharedPreferences
     */
    private void saveIP(String ip) {
        boolean isSaved = false;
        for (Map.Entry<String, ?> entry : ipSP.getAll().entrySet()) {
            if (entry.getValue().equals(ip)) {
                isSaved = true;
                break;
            }
        }
        if (!isSaved) {
            SharedPreferences.Editor ipSPEdit = ipSP.edit();
            ipSPEdit.putString(getCurrentTime(), ip);
            ipSPEdit.apply();
        }
    }

    /**
     * 保存 Port 至 SharedPreferences
     */
    private void savePort(int port) {
        boolean isSaved = false;
        for (Map.Entry<String, ?> entry : portSP.getAll().entrySet()) {
            try {
                if (Integer.parseInt(entry.getValue().toString()) == port) {
                    isSaved = true;
                    break;
                }
            } catch (Exception ex) {
                ex.printStackTrace();
            }
        }
        if (!isSaved) {
            SharedPreferences.Editor ipSPEdit = portSP.edit();
            ipSPEdit.putInt(getCurrentTime(), port);
            ipSPEdit.apply();
        }
    }

    /**
     * 获取存储的 IP
     */
    private String[] getSavedIP() {
        ArrayList<String> ret = new ArrayList<>();
        for (Map.Entry<String, ?> entry : ipSP.getAll().entrySet()) {
            ret.add(entry.getValue().toString());
        }
        return ret.toArray(new String[0]);
    }

    /**
     * 获取存储的 Port
     */
    private String[] getSavedPort() {
        ArrayList<String> ret = new ArrayList<>();
        for (Map.Entry<String, ?> entry : portSP.getAll().entrySet()) {
            ret.add(entry.getValue().toString());
        }
        return ret.toArray(new String[0]);
    }

    /**
     * 初始化自动完成框
     */
    private void initAutoCompleteEdit() {
        ArrayAdapter ipAdapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, getSavedIP());
        ArrayAdapter portAdapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, getSavedPort());
        m_edt_ip.setAdapter(ipAdapter);
        m_edt_port.setAdapter(portAdapter);
        ipAdapter.notifyDataSetChanged();
        portAdapter.notifyDataSetChanged();
    }
}
