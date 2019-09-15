using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.Runtime.InteropServices;
using NeteaseM2DServer.Src.Model;

namespace NeteaseM2DServer.Src.UI
{
    public partial class LyricForm : Form {

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0x2;

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        // bAlpha: 0 .. 255
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        /// <summary>
        /// 设置窗口穿透
        /// </summary>
        /// <param name="opacity">this.Opacity</param>
        /// <param name="isCross">是否穿透</param>
        private void setWindowCrossOver(double opacity, bool isCross = true) {
            uint intExTemp = GetWindowLong(this.Handle, GWL_EXSTYLE);
            uint oldGWLEx;
            if (isCross)
                oldGWLEx = SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            else
                oldGWLEx = SetWindowLong(this.Handle, GWL_EXSTYLE, WS_EX_LAYERED);

            SetLayeredWindowAttributes(this.Handle, 0, (int)(opacity * 255), LWA_ALPHA);
        }

        private static LyricForm Instance;
        public static LyricForm getInstance() {
            if (Instance == null) {
                Instance = new LyricForm();
            }
            return Instance;
        }

        private LyricForm() {
            InitializeComponent();
        }

        private Point MouseDownMousePosition;
        private Point MouseDownWinPosition;

        private void Object_MouseDown(object sender, MouseEventArgs e) {
            MouseDownMousePosition = Cursor.Position;
            MouseDownWinPosition = new Point(this.Left, this.Top);
        }

        private void Object_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) { 
                this.Top = MouseDownWinPosition.Y + Cursor.Position.Y - MouseDownMousePosition.Y;
                this.Left = MouseDownWinPosition.X + Cursor.Position.X - MouseDownMousePosition.X;
            }
        }

        private void LyricForm_Load(object sender, EventArgs e) {
            this.Opacity = 0;
            Computer My = new Computer();
            this.Width = (int)(My.Screen.WorkingArea.Width * 0.8);
            this.Left = (My.Screen.Bounds.Width - this.Width) / 2;
            this.Top = My.Screen.WorkingArea.Height - this.Height * 1;

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
            foreach (Control o in this.Controls) {
                if (o.GetType().Equals(typeof(Button))) continue;
                o.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
                o.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
            }
            timerShow.Enabled = true;
        }

        /// <summary>
        /// 窗口显示 Timer
        /// </summary>
        private void timerShow_Tick(object sender, EventArgs e) {
            this.Opacity += 0.05;
            if (this.Opacity >= Global.LyricWinOpacity) {
                this.Opacity = Global.LyricWinOpacity;
                timerShow.Enabled = false;
                setWindowCrossOver(this.Opacity);
                timerLyric.Enabled = true;
            }
        }

        /// <summary>
        /// 窗口退出 Timer
        /// </summary>
        private void timerHide_Tick(object sender, EventArgs e) {
            this.Opacity -= 0.05;
            if (this.Opacity <= 0) {
                this.Opacity = 0;
                timerHide.Enabled = false;
                Instance = null;
                this.Close();
            }
        }

        /// <summary>
        /// 检查退出条件
        /// </summary>
        private void LyricForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = this.Opacity != 0;
            if (timerShow.Enabled) timerShow.Enabled = false;
            if (timerLyric.Enabled) timerLyric.Enabled = false;
            timerHide.Enabled = true;
        }

        /// <summary>
        /// 退出，测试用
        /// </summary>
        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// 主计时器，歌词和窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerLyric_Tick(object sender, EventArgs e)
        {
            if (Cursor.Position.X > this.Left + button1.Left &&
                Cursor.Position.X < this.Left + button1.Left + button1.Width &&
                Cursor.Position.Y > this.Top + button1.Top &&
                Cursor.Position.Y < this.Top + button1.Top + button1.Height) {
                setWindowCrossOver(this.Opacity, false);
            }
            else {
                setWindowCrossOver(this.Opacity, true);
            }
        }
    } 
}
