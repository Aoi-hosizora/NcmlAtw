using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NcmlAtwServer {

    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            cmsText.Renderer = new NativeRenderer();
            cmsAdjustOffset.Renderer = new NativeRenderer();

            var interfaces = Utils.GetNetworkInterfaces();
            cbbInterface.Items.AddRange(interfaces.ToArray());
            var defaultInterface = Properties.Settings.Default.NetworkInterface;
            if (interfaces.Contains(defaultInterface)) {
                cbbInterface.SelectedItem = defaultInterface;
            } else if (cbbInterface.Items.Count > 0) {
                cbbInterface.SelectedIndex = 0;
            }
            numPort.Value = Properties.Settings.Default.Port;

            lblDuration.Text = "未监听...";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            var ok = MessageBox.Show("确定退出程序？", "退出", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (ok != DialogResult.Yes) {
                e.Cancel = true;
                return;
            }

            e.Cancel = false;
            Properties.Settings.Default.Top = Top;
            Properties.Settings.Default.Left = Left;
            Properties.Settings.Default.Save();
        }

        private void BtnExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void CbbInterface_SelectedIndexChanged(object sender, EventArgs e) {
            var currentInterface = (string) cbbInterface.SelectedItem;
            edtIP.Text = Utils.GetNetworkInterfaceIPv4(currentInterface);
        }

        private void BtnQrcode_Click(object sender, EventArgs e) {
            var bmp = Utils.GenerateAddressQrcode(edtIP.Text, (int) numPort.Value);
            Utils.ShowBitmapForm(bmp, "二维码", this);
        }

        private void BtnVisitLink_Click(object sender, EventArgs e) {
            var text = lblLink.Text;
            if (!text.StartsWith("http://") && !text.StartsWith("https://")) {
                if (lblDuration.Text.Contains("正在")) {
                    MessageBox.Show("暂无歌曲，请等待。", "打开链接", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else {
                    MessageBox.Show("未找到链接。", "打开链接", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else {
                Process.Start(text);
            }
        }

        private void BtnListen_Click(object sender, EventArgs e) {
            var doListen = btnListen.Text == "开始监听";
            if (doListen) {
                btnListen.Text = "停止监听";
                cbbInterface.Enabled = false;
                numPort.ReadOnly = true;
                btnQrcode.Enabled = true;
                btnAdjustOffset.Enabled = true;
                lblDuration.Text = "正在等待歌曲...";
                lblTitle.Text = "未知标题";
                lblAlbum.Text = "未知专辑";
                lblArtist.Text = "未知歌手";
                lblLink.Text = "未知链接";
            } else {
                btnListen.Text = "开始监听";
                cbbInterface.Enabled = true;
                numPort.ReadOnly = false;
                btnQrcode.Enabled = false;
                btnAdjustOffset.Enabled = false;
                lblDuration.Text = "未监听...";
            }

            if (doListen) {
                Properties.Settings.Default.NetworkInterface = (string) cbbInterface.SelectedItem;
                Properties.Settings.Default.Port = (int) numPort.Value;
                Properties.Settings.Default.Save();
            } else {

            }
        }

        private void BtnAdjustOffset_Click(object sender, EventArgs e) {
            cmsAdjustOffset.Show(btnAdjustOffset, new Point(0, btnAdjustOffset.Height));
        }

        private void MiCopy_Click(object sender, EventArgs e) {
            if (cmsText.SourceControl is Label c) {
                Clipboard.SetText(c.Text);
            }
        }

        private void MiAdjustOffset_Click(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem mi) {
                Global.Offset += (double) mi.Tag;
            }
        }
    }
}
