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

        private static ParameterizedThreadStart ServiceThreadStart = new ParameterizedThreadStart(SocketService.RunThread);
        private static Thread ServiceThread = new Thread(ServiceThreadStart);

        private void MainForm_Load(object sender, EventArgs e) {
            ServiceThread.Start(1212);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            Environment.Exit(0);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            
        }
    }
}
