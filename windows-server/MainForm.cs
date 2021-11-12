using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NcmlAtwServer {

    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private static MainForm _instance;

        public static MainForm Instance {
            get {
                if (_instance == null) {
                    _instance = new MainForm();
                }
                return _instance;
            }
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
            ckbAutoDisconnect.Checked = Properties.Settings.Default.AutoDisconnect;

            lblDuration.Text = "未监听...";
            _globalTimer = new Timer(20);
            _globalTimer.Elapsed += (s, e2) => Invoke(new Action(() => GlobalTimer_Elapsed(s, e2)));
        }

        private bool _hasCheckedExit = false;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (Global.IsListening) {
                var ok1 = MessageBox.Show("是否结束监听？", "退出", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (ok1 != DialogResult.Yes) {
                    e.Cancel = true;
                    return;
                }
                StopService();
            }

            if (!_hasCheckedExit) {
                var ok2 = MessageBox.Show("确定退出程序？", "退出", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (ok2 != DialogResult.Yes) {
                    e.Cancel = true;
                    return;
                }
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Save();
            }
            _hasCheckedExit = true;

            if (LyricForm.Instance.Opacity != 0) {
                LyricForm.Instance.Close();
                e.Cancel = true;
                new Thread(() => {
                    while (true) {
                        if (LyricForm.Instance.Opacity <= 0) {
                            Invoke(new Action(() => Close())); // 下一次 Close 直接退出
                            break;
                        }
                    }
                }).Start();
            }
        }

        // =============
        // event handler
        // =============

        private void BtnExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void CbbInterface_SelectedIndexChanged(object sender, EventArgs e) {
            var currentInterface = (string) cbbInterface.SelectedItem;
            edtIP.Text = Utils.GetNetworkInterfaceIPv4(currentInterface);
        }

        private void BtnQrcode_Click(object sender, EventArgs e) {
            var ip = edtIP.Text;
            if (ip == "unknown") {
                MessageBox.Show("未知的 IP 地址，无法生成二维码，请检查对应的网络接口。", "二维码", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var bmp = Utils.GenerateAddressQrcode(ip, (int) numPort.Value);
            Utils.ShowBitmapForm(bmp, "二维码", this);
        }

        private void BtnVisitLink_Click(object sender, EventArgs e) {
            if (Global.CurrentMetadata == null) {
                MessageBox.Show("暂无歌曲，请等待。", "打开链接", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (Global.CurrentMusicId == -1) {
                MessageBox.Show("未找到链接。", "打开链接", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                var link = Utils.GetMusicLink(Global.CurrentMusicId);
                Process.Start(link);
            }
        }

        private void BtnAdjustOffset_Click(object sender, EventArgs e) {
            miOffsetText.Text = $"当前时间差 : {Global.Offset} 秒";
            cmsAdjustOffset.Show(btnAdjustOffset, new Point(0, btnAdjustOffset.Height));
        }

        private void MiAdjustOffset_Click(object sender, EventArgs e) {
            if (sender is ToolStripMenuItem mi) {
                if (mi.Tag is double d) {
                    Global.Offset += d;
                }
            }
        }

        private void MiResetOffset_Click(object sender, EventArgs e) {
            Global.Offset = 0;
        }

        private void MiCopy_Click(object sender, EventArgs e) {
            if (cmsText.SourceControl is Label c) {
                Clipboard.SetText(c.Text);
            }
        }

        private void CkbAutoDisconnect_CheckedChanged(object sender, EventArgs e) {
            Properties.Settings.Default.AutoDisconnect = (sender as CheckBox).Checked;
            Properties.Settings.Default.Save();
        }

        private void CkbPauseIgnore_CheckedChanged(object sender, EventArgs e) {
            Global.Pausing = (sender as CheckBox).Checked;
        }

        private void BtnShowLyric_Click(object sender, EventArgs e) {
            var form = LyricForm.Instance;
            form.Activate();
            form.Show();
        }

        private void BtnListen_Click(object sender, EventArgs e) {
            if (btnListen.Text == "开始监听") {
                StopService();
                StartService();
            } else {
                StopService();
            }
        }

        // ===============
        // service related
        // ===============

        /// <summary>执行 socket 服务线程</summary>
        private Thread _socketThread;
        private SocketService _socketService;
        /// <summary>用于搜索歌曲和歌词</summary>
        private Thread _searchThread;
        /// <summary>全局计时器：歌曲状态显示、歌曲时间更新、歌词显示</summary>
        private Timer _globalTimer;

        private void StartService() {
            _socketService = new SocketService {
                listenCallback = (ok) => Invoke(new Action(() => {
                    if (!ok) {
                        MessageBox.Show("监听失败，可能因为端口被占用。", "监听", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StopService();
                        return;
                    }

                    Properties.Settings.Default.NetworkInterface = (string) cbbInterface.SelectedItem;
                    Properties.Settings.Default.Port = (int) numPort.Value;
                    Properties.Settings.Default.Save();

                    Global.IsListening = true;
                    Global.Pausing = false;
                    Global.CurrentMetadata = null;
                    Global.CurrentState = null;
                    Global.CurrentPosition = 0;
                    Global.CurrentMusicId = -1;

                    // <<<
                    btnListen.Text = "结束监听";
                    cbbInterface.Enabled = false; // 网络监听
                    numPort.ReadOnly = true;
                    btnQrcode.Enabled = true;
                    lblDuration.Text = "正在等待歌曲..."; // 当前歌曲
                    btnAdjustOffset.Enabled = false;
                    lblTitle.Text = "未知标题";
                    lblAlbum.Text = "未知专辑";
                    lblArtist.Text = "未知歌手";
                    lblLink.Text = "未知链接";
                    lblTitle.Visible = lblTitleHint.Visible = false;
                    lblArtist.Visible = lblArtistHint.Visible = false;
                    lblAlbum.Visible = lblAlbumHint.Visible = false;
                    lblLink.Visible = lblLinkHint.Visible = false;
                    ckbPauseIgnore.Enabled = true; // 下方按钮
                    ckbPauseIgnore.Checked = false;
                    btnShowLyric.Enabled = false;
                    btnVisitLink.Enabled = false;
                })),
                pingCallback = () => Invoke(new Action(() => {
                    Console.WriteLine("pong");
                    Utils.CloseBitmapForm();
                })),
                metadataCallback = (o) => Invoke(new Action(() => SocketMetadataCallback(o))),
                playbackStateCallback = (o) => Invoke(new Action(() => SocketPlaybackStateCallback(o))),
                sessionDestroyedCallback = () => Invoke(new Action(() => {
                    if (ckbAutoDisconnect.Checked) {
                        StopService();
                        MessageBox.Show("安卓端的连接已经断开。", "监听", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                })),
            };

            int port = (int) numPort.Value;
            _socketThread = new Thread(() => _socketService.StartAndRunService(port)) {
                IsBackground = true
            };
            _socketThread.Start();
        }

        private void StopService() {
            _searchThread?.Abort();
            _searchThread = null;
            _globalTimer.Stop();
            _socketService?.StopService();
            _socketService = null;
            _socketThread?.Abort();
            _socketThread = null;
            if (LyricForm.Instance.Opacity != 0) {
                LyricForm.Instance.Close();
            }

            // <<<
            btnListen.Text = "开始监听";
            cbbInterface.Enabled = true; // 网络监听
            numPort.ReadOnly = false;
            btnQrcode.Enabled = false;
            lblDuration.Text = "未监听..."; // 当前歌曲
            btnAdjustOffset.Enabled = false;
            lblTitle.Visible = lblTitleHint.Visible = false;
            lblArtist.Visible = lblArtistHint.Visible = false;
            lblAlbum.Visible = lblAlbumHint.Visible = false;
            lblLink.Visible = lblLinkHint.Visible = false;
            ckbPauseIgnore.Enabled = false; // 下方按钮
            ckbPauseIgnore.Checked = false;
            btnShowLyric.Enabled = false;
            btnVisitLink.Enabled = false;

            Global.IsListening = false;
            Global.Pausing = false;
            Global.CurrentMetadata = null;
            Global.CurrentState = null;
            Global.CurrentPosition = 0;
            Global.CurrentMusicId = -1;
        }

        private void SocketMetadataCallback(Metadata obj) {
            if (!Global.IsListening || Global.Pausing) {
                return;
            }
            if (Global.CurrentMetadata?.Equals(obj) == true) {
                return;
            }
            Global.CurrentMetadata = obj;
            Global.CurrentPosition = 0;
            Global.CurrentMusicId = -1; // 等待搜索歌曲
            Global.CurrentLyric = null;
            Global.CurrentLyricState = LyricState.NotFound;

            // <<<
            btnAdjustOffset.Enabled = true;
            lblTitle.Text = obj.Title;
            lblArtist.Text = obj.Artist;
            lblAlbum.Text = obj.Album;
            lblLink.Text = "未知链接";
            lblTitle.Visible = lblTitleHint.Visible = true;
            lblArtist.Visible = lblArtistHint.Visible = true;
            lblAlbum.Visible = lblAlbumHint.Visible = true;
            lblLink.Visible = lblLinkHint.Visible = true;
            tipText.SetToolTip(lblTitle, lblTitle.Text);
            tipText.SetToolTip(lblArtist, lblArtist.Text);
            tipText.SetToolTip(lblAlbum, lblAlbum.Text);
            tipText.SetToolTip(lblLink, lblLink.Text);
            btnShowLyric.Enabled = true;
            btnVisitLink.Enabled = true;

            _globalTimer.Start();
            _searchThread?.Abort();
            _searchThread = new Thread(() => {
                // search music
                Invoke(new Action(() => LyricForm.Instance.UpdateMusicLyric(searching: true)));
                var song = Utils.SearchMusicFromNcma(obj);
                if (song == null) {
                    Invoke(new Action(() => LyricForm.Instance.UpdateMusicLyric(searching: false)));
                    return;
                }
                Global.CurrentMusicId = song.Id;
                Invoke(new Action(() => {
                    lblLink.Text = Utils.GetMusicLink(song.Id);
                    tipText.SetToolTip(lblLink, lblLink.Text);
                }));

                // search music lyric
                Invoke(new Action(() => LyricForm.Instance.UpdateMusicLyric(searching: true)));
                var (lyricPage, lyricState) = Utils.SearchLyricFromNcma(song.Id);
                Console.WriteLine($"Lyric lines: {lyricPage?.Lines?.Count ?? 0}, Lyric state: {lyricState}");
                Global.CurrentLyricState = lyricState;
                if (lyricState == LyricState.Found) {
                    Global.CurrentLyric = lyricPage;
                }
                Invoke(new Action(() => LyricForm.Instance.UpdateMusicLyric(searching: false)));
            });
            _searchThread.Start();
        }

        private void SocketPlaybackStateCallback(PlaybackState obj) {
            if (!Global.IsListening || Global.Pausing) {
                return;
            }

            Global.CurrentState = obj;
            Global.CurrentPosition = obj.CurrentPosition;
            Global.LastUpdateTimestamp = Utils.GetCurrentTimestamp(); // ms

            // <<<
            btnAdjustOffset.Enabled = true;
            lblTitle.Visible = lblTitleHint.Visible = true;
            lblArtist.Visible = lblArtistHint.Visible = true;
            lblAlbum.Visible = lblAlbumHint.Visible = true;
            lblLink.Visible = lblLinkHint.Visible = true;
            btnShowLyric.Enabled = true;
            btnVisitLink.Enabled = true;

            _globalTimer.Start();
        }

        private void UpdateShownDuration(double current, double total) {
            int curSec = (int) (current / 60.0);
            int curMs = (int) (current % 60.0);
            int totSec = (int) (total / 60.0);
            int totMs = (int) (total % 60.0);
            lblDuration.Text = string.Format("当前进度 : {0:00}:{1:00} / {2:00}:{3:00}", curSec, curMs, totSec, totMs);
        }

        private void GlobalTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if (LyricForm.Instance.TimerEnabled) {
                try {
                    LyricForm.Instance.GlobalTimer_Elapsed(sender, e);
                } catch { }
            }
            if (!Global.IsListening || Global.Pausing) {
                return;
            }

            if (Global.CurrentMetadata == null) {
                lblDuration.Text = "正在等待歌曲...";
            } else if (Global.CurrentState == null) { // 无当前时间
                UpdateShownDuration(current: 0, total: Global.CurrentMetadata.Duration);
            } else if (!Global.CurrentState.IsPlaying) { // 暂停中
                UpdateShownDuration(current: Global.CurrentState.CurrentPosition + Global.Offset, total: Global.CurrentMetadata.Duration);
            } else { // 实时更新时间
                long now = Utils.GetCurrentTimestamp(); // ms
                Global.CurrentPosition = Global.CurrentState.CurrentPosition + (double) ((now - Global.LastUpdateTimestamp) / 1000.0) + Global.Offset;
                if (Global.CurrentPosition <= Global.CurrentMetadata.Duration) {
                    UpdateShownDuration(current: Global.CurrentPosition, total: Global.CurrentMetadata.Duration);
                }
            }
        }
    }
}
