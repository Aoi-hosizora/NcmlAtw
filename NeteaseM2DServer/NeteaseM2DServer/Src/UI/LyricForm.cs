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
using NeteaseM2DServer.Src.Util;

namespace NeteaseM2DServer.Src.UI
{
    public partial class LyricForm : Form {

        private static LyricForm Instance;
        public static LyricForm getInstance() {
            if (Instance == null)
                Instance = new LyricForm();
            return Instance;
        }

        private LyricForm() {
            InitializeComponent();
        }

        private void LyricForm_Load(object sender, EventArgs e) {
            this.Opacity = 0;

            // 窗口位置
            menuItemPosition_Click(menuItemPosition, e);

            // 订阅移动窗口事件
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
            foreach (Control o in this.Controls) {
                o.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
                o.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
            }

            // 透明度菜单
            for (int i = 1; i <= 10; i++) {
                var menuItemOpacitySubItem = new ToolStripMenuItem();
                menuItemOpacitySubItem.Name = "menuItemOpacitySubItem" + i + "0";
                menuItemOpacitySubItem.Tag = ((double)i) / 10.0;
                menuItemOpacitySubItem.Text = (i != 10) ? "&" + i + "0%" : "1&00%";
                menuItemOpacitySubItem.Click += new System.EventHandler(this.menuItemOpacitySubItem_Click);
                menuItemOpacity.DropDownItems.Add(menuItemOpacitySubItem);
            }

            // 显示窗口 & 歌词
            timerShow.Enabled = true;
            Global.LyricFormTimer = timerLyric_Tick;
        }

        /// <summary>
        /// 检查退出条件
        /// </summary>
        private void LyricForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = this.Opacity != 0;
            if (timerShow.Enabled) timerShow.Enabled = false;
            // if (timerLyric.Enabled) timerLyric.Enabled = false;
            timerHide.Enabled = true;
            Global.LyricFormTimer = null;
        }

        #region 窗口设置

        private Point MouseDownMousePosition;
        private Point MouseDownWinPosition;

        /// <summary>
        /// 按下，记录位置用于移动
        /// </summary>
        private void Object_MouseDown(object sender, MouseEventArgs e) {
            MouseDownMousePosition = Cursor.Position;
            MouseDownWinPosition = new Point(this.Left, this.Top);
        }

        /// <summary>
        /// 按钮右键移动，其他左键移动
        /// </summary>
        private void Object_MouseMove(object sender, MouseEventArgs e) {
            bool rn = e.Button == MouseButtons.Left;
            if (sender.GetType().Equals(typeof(Button))) 
                rn = e.Button == MouseButtons.Right;
            if (rn) { 
                this.Top = MouseDownWinPosition.Y + Cursor.Position.Y - MouseDownMousePosition.Y;
                this.Left = MouseDownWinPosition.X + Cursor.Position.X - MouseDownMousePosition.X;
            }
        }

        /// <summary>
        /// 窗口显示 Timer
        /// </summary>
        private void timerShow_Tick(object sender, EventArgs e) {
            bool rn;
            if (this.Opacity < Global.LyricWinOpacity) {
                this.Opacity += 0.05;
                rn = this.Opacity >= Global.LyricWinOpacity;
            }
            else {
                this.Opacity -= 0.05;
                rn = this.Opacity <= Global.LyricWinOpacity;
            }

            if (rn) {
                this.Opacity = Global.LyricWinOpacity;
                timerShow.Enabled = false;

                CommonUtil.setWindowCrossOver(this, this.Opacity, true);
                // timerLyric.Enabled = true;
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

        #endregion // 窗口设置

        #region 弹出菜单

        /// <summary>
        /// 菜单，关闭歌词
        /// </summary>
        private void menuItemExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        /// <summary>
        /// 菜单，锁定
        /// </summary>
        private void menuItemLock_Click(object sender, EventArgs e) {
            menuItemLock.Checked = !menuItemLock.Checked;
            CommonUtil.setWindowCrossOver(this, this.Opacity, menuItemLock.Checked);
        }

        /// <summary>
        /// 菜单，原位置
        /// </summary>
        private void menuItemPosition_Click(object sender, EventArgs e) {
            Computer My = new Computer();
            this.Width = (int)(My.Screen.WorkingArea.Width * 0.8);
            this.Left = (My.Screen.Bounds.Width - this.Width) / 2;
            this.Top = My.Screen.WorkingArea.Height - this.Height * 1;
        }

        /// <summary>
        /// 菜单，透明度修改
        /// </summary>
        private void menuItemOpacitySubItem_Click(object sender, EventArgs e) {
            Global.LyricWinOpacity = (double)(sender as ToolStripMenuItem).Tag;
            timerShow.Enabled = true;
        }

        /// <summary>
        /// 菜单，修改文字颜色
        /// </summary>
        private void menuItemForeColor_Click(object sender, EventArgs e) {
            colorDialog.Color = labelLyric.ForeColor;
            colorDialog.ShowDialog();
            labelLyric.ForeColor = colorDialog.Color;
        }

        /// <summary>
        /// 菜单，修改背景颜色
        /// </summary>
        private void menuForeBackColor_Click(object sender, EventArgs e) {
            colorDialog.Color = this.BackColor;
            colorDialog.ShowDialog();
            this.BackColor = colorDialog.Color;
        }

        /// <summary>
        /// 菜单，恢复颜色
        /// </summary>
        private void menuItemRestoreColor_Click(object sender, EventArgs e) {
            labelLyric.ForeColor = Color.White;
            this.BackColor = Color.Black;
        }

        #endregion // 弹出菜单

        /// <summary>
        /// 弹出菜单
        /// </summary>
        private void buttonOption_Click(object sender, EventArgs e) {
            contextMenuStrip.Show(
                this.Left + (sender as Button).Left,
                this.Top + (sender as Button).Top - contextMenuStrip.Height
            );
        }

        private long currentSongId = -1;

        /// <summary>
        /// 主计时器，歌词和窗口
        /// </summary>
        private void timerLyric_Tick() {
            // interval == 10

            // 窗口
            if (menuItemLock.Checked)
                CommonUtil.setWindowCrossOver(this, this.Opacity, !CommonUtil.ControlInRange(this, buttonOption));

            // 歌词转换
            if (currentSongId != Global.MusicId) {
                currentSongId = Global.MusicId;
                if (Global.MusicLrc != "")
                    Global.MusicLyricPage = LyricPage.parseLrc(Global.MusicLrc);
                else
                    Global.MusicLyricPage = null;
            }

            // 歌词显示
            // TODO
            if (Global.MusicLrc == "") labelLyric.Text = "未找到歌词";
            else if (Global.MusicLyricPage == null) labelLyric.Text = "歌词处理中";
            else {
                LyricLine currLine = CommonUtil.GetCurrentLine(Global.currentPos, Global.MusicLyricPage);
                // Console.WriteLine("currentPos: " + Global.currentPos + " timeDuration: " + currLine.timeDuration + " lyric: " + currLine.ToString());
                if (currLine.Lyric != labelLyric.Text)
                    labelLyric.Text = currLine.Lyric;
            }
        }
    } 
}
