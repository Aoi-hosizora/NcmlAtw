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

namespace NeteaseM2DServer.Src.UI
{
    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private Thread socketThread;
        private SocketService socketService;

        /// <summary>
        /// 开始线程
        /// </summary>
        /// <param name="port">端口号</param>
        private void StartThread(int port) {
            socketService = new SocketService();
            socketService.port = port;

            socketService.listenCb = (ok) =>
            {
                // コントロールが作成されたスレッド以外のスレッドからコントロール 'buttonListen' がアクセスされました
                 this.Invoke(new Action(() => {
                     Global.isListening = buttonShowLyric.Enabled = ok;
                     numericUpDownPort.Enabled = !ok;
                     if (ok) {
                         labelSongDuration.Text = "正在等待歌曲...";
                         buttonListen.Text = "取消监听";

                         labelSongTitle.Text = "未知歌曲";
                         labelSongArtist.Text = "未知歌手";
                         labelSongAlbum.Text = "未知专辑";
                     }
                     else {
                         labelSongDuration.Text = "未监听...";
                         buttonListen.Text = "监听端口";
                         timerSong.Enabled = false;
                         MessageBox.Show("端口监听失败，可能是被占用。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     }
                 }));
            };

            socketService.pingCb = (ok) => { };

            socketService.metaDataCb = SocketMetaDataCb;
            socketService.playbackStateCb = SocketPlaybackStateCb;

            socketThread = new Thread(new ThreadStart(socketService.RunThread));
            socketThread.IsBackground = true;
            socketThread.Start();
        }

        /// <summary>
        /// Socket 获取 MetaData 回调
        /// </summary>
        /// <param name="obj"></param>
        private void SocketMetaDataCb(Metadata obj) {
            Console.WriteLine(obj.ToString());
            if (!Global.isListening) return;

            this.Invoke(new Action(() => {
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = true;
                Global.currentSong = obj;

                labelSongTitle.Text = "标题：" + obj.title;
                labelSongArtist.Text = "歌手：" + obj.artist;
                labelSongAlbum.Text = "专辑：" + obj.album;
                toolTip.SetToolTip(labelSongTitle, labelSongTitle.Text.Substring(3));
                toolTip.SetToolTip(labelSongArtist, labelSongArtist.Text.Substring(3));
                toolTip.SetToolTip(labelSongAlbum, labelSongAlbum.Text.Substring(3));

                timerSong.Enabled = true;
            }));
        }

        /// <summary>
        /// Socket 获取 PlatbackState 回调
        /// </summary>
        /// <param name="obj"></param>
        private void SocketPlaybackStateCb(PlaybackState obj) {
            Global.stateUpdateMS = GetTimeStamp();

            Console.WriteLine(obj.ToString());
            if (!Global.isListening) return;

            this.Invoke(new Action(() => {
                timerSong.Enabled = true;
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

        private void MainForm_Load(object sender, EventArgs e) {
            labelSongDuration.Text = "未监听...";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            StopThread();
        }

        private void buttonListen_Click(object sender, EventArgs e) {
            if ((sender as Button).Text == "监听端口") {
                StartThread(int.Parse(numericUpDownPort.Value.ToString()));
            }
            else {
                StopThread();
                labelSongDuration.Text = "未监听...";
                buttonListen.Text = "监听端口";
                Global.isListening = buttonShowLyric.Enabled = false;
                numericUpDownPort.Enabled = true;
                timerSong.Enabled = false;
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = false;
            }
            
        }

        private void buttonExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void timerSong_Tick(object sender, EventArgs e) {
            long now = GetTimeStamp();

            if (!Global.currentState.isPlay) return;
            string currentPos = "未知", duration = "未知";
            if (Global.currentState != null) {
                double s = Global.currentState.currentPosSecond + (double) ((now - Global.stateUpdateMS) / 1000.0);
                currentPos = ((int)(s / 60.0)).ToString("00") + ":" + ((int)(s % 60.0)).ToString("00");
            }
            if (Global.currentSong != null) {
                double s = Global.currentSong.duration;
                duration = ((int)(s / 60.0)).ToString("00") + ":" + ((int)(s % 60.0)).ToString("00");
            }

            labelSongDuration.Text = currentPos + " / " + duration;
        }

        private long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Int64.Parse(Convert.ToInt64(ts.TotalMilliseconds).ToString());
        }
    }
}
