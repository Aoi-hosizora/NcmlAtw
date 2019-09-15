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
        /// <param name="port"></param>
        private void StartThread(int port) {
            socketService = new SocketService();
            socketService.port = port;
            socketService.metaDataCb = (obj) => {
                Console.WriteLine(obj.ToString());
            };
            socketService.playbackStateCb = (obj) => {
                Console.WriteLine(obj.ToString());
            };
            socketService.pingCb = (ok) => {
                if (ok) Console.WriteLine("ping");
            };
            socketThread = new Thread(new ThreadStart(socketService.RunThread));
            socketThread.Start();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            StartThread(1212);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (socketService != null)
                socketService.serverListener.Stop();
            if (socketThread != null)
                socketThread.Abort();
        }
    }
}
