using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NeteaseM2DServer.Src.Service;
using System.Threading;
using NeteaseM2DServer.Src.Model;
using NeteaseM2DServer.Src.Api;
using System.Text.RegularExpressions;
using NeteaseM2DServer.Src.Util;

namespace NeteaseM2DServer.Src.UI {

    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        // StartThread SocketMetaDataCb SocketPlaybackStateCb StopThread
        #region 线程服务

        private Thread socketThread;
        private SocketService socketService;

        /// <summary>
        /// 开始线程
        /// </summary>
        /// <param name="port">端口号</param>
        private void StartThread(int port) {
            socketService = new SocketService();
            socketService.port = port;

            socketService.listenCb = (ok) => {
                // コントロールが作成されたスレッド以外のスレッドからコントロール 'buttonListen' がアクセスされました
                this.Invoke(new Action(() => {
                    Global.isListening = ok;
                    numericUpDownPort.Enabled = !ok;
                    if (ok) {
                        labelSongDuration.Text = "正在等待歌曲...";
                        buttonListen.Text = "取消监听";

                        labelSongTitle.Text = "未知歌曲";
                        labelSongArtist.Text = "未知歌手";
                        labelSongAlbum.Text = "未知专辑";
                    } else {
                        labelSongDuration.Text = "未监听...";
                        buttonListen.Text = "监听端口";
                        timerGlobal.Enabled = false;
                        MessageBox.Show("端口监听失败，可能是被占用。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }));
            };

            socketService.pingCb = (ok) => {
                Console.WriteLine("ping");
            };

            socketService.metaDataCb = SocketMetaDataCb;
            socketService.playbackStateCb = SocketPlaybackStateCb;

            socketThread = new Thread(new ThreadStart(socketService.RunThread));
            socketThread.IsBackground = true;
            socketThread.Start();
        }

        /// <summary>
        /// Socket 获取 MetaData 歌曲信息 回调
        /// </summary>
        /// <param name="obj">Metadata</param>
        private void SocketMetaDataCb(Metadata obj) {
            Console.WriteLine(obj.ToString());
            if (!Global.isListening) return;

            this.Invoke(new Action(() => {
                buttonShowLyric.Enabled = buttonOpenWeb.Enabled = true;
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = true;
                Global.currentSong = obj;

                labelSongTitle.Text = "标题：" + obj.title;
                labelSongArtist.Text = "歌手：" + obj.artist;
                labelSongAlbum.Text = "专辑：" + obj.album;
                toolTip.SetToolTip(labelSongTitle, labelSongTitle.Text.Substring(3));
                toolTip.SetToolTip(labelSongArtist, labelSongArtist.Text.Substring(3));
                toolTip.SetToolTip(labelSongAlbum, labelSongAlbum.Text.Substring(3));

                double s = Global.currentSong.duration;
                Global.durationStr = ((int)(s / 60.0)).ToString("00") + ":" + ((int)(s % 60.0)).ToString("00");

                // 搜索歌词
                Search();

                // 更新歌词
                LyricForm.getInstance().updateSongLyric();

                // 开始全局计时
                timerGlobal.Enabled = true;
            }));
        }

        /// <summary>
        /// Socket 获取 PlatbackState 播放状态 回调
        /// </summary>
        /// <param name="obj">PlaybackState</param>
        private void SocketPlaybackStateCb(PlaybackState obj) {
            Global.stateUpdateMS = CommonUtil.GetTimeStamp();

            Console.WriteLine(obj.ToString());
            if (!Global.isListening) return;

            this.Invoke(new Action(() => {
                timerGlobal.Enabled = true;
                Global.currentState = obj;
            }));
        }

        /// <summary>
        /// 结束线程
        /// </summary>
        private void StopThread() {
            if (socketService != null)
                socketService.serverListener.Stop();
            if (socketThread != null)
                socketThread.Abort();
        }

        #endregion // 线程服务

        // Search
        #region 歌曲查找

        /// <summary>
        /// 查找歌曲 Id 和 歌词
        /// </summary>
        public void Search() {
            var api = new NeteaseMusicAPI();

            Regex reg = new Regex("(.*)");
            string searchStr = reg.Replace(Global.currentSong.title, "") + " " + Global.currentSong.artist + " " + Global.currentSong.album;

            SearchResult searchResult = api.Search(searchStr);
            if (searchResult != null && searchResult.Code == 200 &&
                searchResult.Result != null && searchResult.Result.Songs != null &&
                searchResult.Result.Songs.Count > 0) {
                Global.MusicId = searchResult.Result.Songs.ElementAt(0).Id;

                // TODO 查找歌词
                LyricResult lyricResult = api.Lyric(Global.MusicId);
                if (lyricResult != null && lyricResult.Code == 200 && lyricResult.Lrc != null)
                    Global.MusicLyricPage = LyricPage.parseLrc(lyricResult.Lrc.Lyric);
                else
                    Global.MusicLyricPage = null;
            } else
                Global.MusicId = -1;
        }

        #endregion // 歌曲查找

        // MainForm_Load MainForm_FormClosed buttonListen_Click buttonExit_Click buttonShowLyric_Click buttonOpenWeb_Click
        #region 界面交互

        private void MainForm_Load(object sender, EventArgs e) {
            labelSongDuration.Text = "未监听...";
            Global.MainFormTimer = timerSong_Tick;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            StopThread();
        }

        /// <summary>
        /// 监听 / 取消监听
        /// </summary>
        private void buttonListen_Click(object sender, EventArgs e) {
            if ((sender as Button).Text == "监听端口") {
                StartThread(int.Parse(numericUpDownPort.Value.ToString()));
            } else {
                StopThread();
                labelSongDuration.Text = "未监听...";
                buttonListen.Text = "监听端口";

                numericUpDownPort.Enabled = true;
                timerGlobal.Enabled = false;
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = false;

                Global.isListening = buttonShowLyric.Enabled = buttonOpenWeb.Enabled = false;
                Global.currentSong = null;
                Global.currentState = null;
                Global.isListening = false;
                Global.MusicId = -1;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// 打开歌词
        /// </summary>
        private void buttonShowLyric_Click(object sender, EventArgs e) {
            LyricForm lyricForm = LyricForm.getInstance();
            lyricForm.Activate();
            lyricForm.Show();
        }

        /// <summary>
        /// 打开网页
        /// </summary>
        private void buttonOpenWeb_Click(object sender, EventArgs e) {
            if (Global.MusicId != -1) {
                string url = "https://music.163.com/#/song?id=" + Global.MusicId;
                System.Diagnostics.Process.Start(url);
            } else {
                DialogResult ret =
                    MessageBox.Show("未找到歌曲 \"" + Global.currentSong.title + "\"。", "错误", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (ret == DialogResult.Retry) {
                    Search();
                    buttonOpenWeb_Click(sender, e);
                }
            }
        }

        #endregion // 界面交互

        /// <summary>
        /// 时间更新
        /// </summary>
        private void timerSong_Tick() {
            long now = CommonUtil.GetTimeStamp();

            if (!Global.currentState.isPlay) return;
            string currentPos = "未知";
            if (Global.currentState != null) {
                double s = Global.currentState.currentPosSecond + (double)((now - Global.stateUpdateMS) / 1000.0);
                Global.currentPos = (int)(s * 1000);
                currentPos = ((int)(s / 60.0)).ToString("00") + ":" + ((int)(s % 60.0)).ToString("00");
            }

            labelSongDuration.Text = currentPos + " / " + (Global.durationStr == "" || Global.durationStr == null ? "未知" : Global.durationStr);
        }

        /// <summary>
        /// 全局计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerGlobal_Tick(object sender, EventArgs e) {
            if (Global.MainFormTimer != null)
                Global.MainFormTimer();
            if (Global.LyricFormTimer != null)
                Global.LyricFormTimer();
        }

    }
}