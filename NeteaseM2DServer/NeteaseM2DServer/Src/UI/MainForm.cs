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

            socketService.listenCb = (ok) => {
                 this.Invoke(new Action(() => {
                    Global.isListening = ok;
                    // コントロールが作成されたスレッド以外のスレッドからコントロール 'buttonListen' がアクセスされました
                    buttonListen.Enabled = ok;
                    if (ok)
                        labelSongDuration.Text = "正在等待歌曲...";
                 }));
            };

            socketService.pingCb = (ok) => {
                // this.Invoke(new Action(() => {
                //     Global.isListening = buttonListen.Enabled = ok;
                //     labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = ok;
                //     labelSongTitle.Text = "未知歌曲";
                //     labelSongArtist.Text = "未知歌手";
                //     labelSongAlbum.Text = "未知专辑";
                // }));
            };

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

            this.Invoke(new Action(() => {
                Global.isListening = buttonListen.Enabled = true;
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = true;

                Global.currentSong = obj;
                labelSongTitle.Text = obj.title;
                labelSongArtist.Text = obj.artist;
                labelSongAlbum.Text = obj.album;

                labelSongDuration.Text = "(" + 
                    (Global.currentState == null ? -1 : Global.currentState.currentPosSecond) + " / " +
                    obj.duration + ")";
            }));
        }

        /// <summary>
        /// Socket 获取 PlatbackState 回调
        /// </summary>
        /// <param name="obj"></param>
        private void SocketPlaybackStateCb(PlaybackState obj) {
            Console.WriteLine(obj.ToString());

            this.Invoke(new Action(() => {
                Global.isListening = buttonListen.Enabled = true;
                labelSongTitle.Visible = labelSongArtist.Visible = labelSongAlbum.Visible = true;

                Global.currentState = obj;
                labelSongDuration.Text = "(" +
                    obj.currentPosSecond + " / " +
                    (Global.currentSong == null ? -1 : Global.currentSong.duration) + ")";
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonListen_Click(object sender, EventArgs e) {
            StartThread(int.Parse(numericUpDownPort.Value.ToString()));
        }
    }
}
