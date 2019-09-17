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

namespace NeteaseM2DServer.Src.UI {

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

        // LyricForm_Load LyricForm_FormClosing
        #region 加载与退出

        private void LyricForm_Load(object sender, EventArgs e) {
            this.Opacity = 0;
            this.BackColor = Properties.Settings.Default.LyricBackColor;
            labelLyric.ForeColor = Properties.Settings.Default.LyricForeColor;

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
                if (Properties.Settings.Default.Opacity == (double)i / 10.0)
                    menuItemOpacitySubItem.Checked = true;
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

        #endregion // 加载与退出

        // Object_MouseDown Object_MouseMove timerShow_Tick timerHide_Tick
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
                (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeAll;
            } else {
                (sender as Control).Cursor = System.Windows.Forms.Cursors.Default;
            }
        }

        /// <summary>
        /// 窗口显示 Timer
        /// </summary>
        private void timerShow_Tick(object sender, EventArgs e) {
            bool rn;
            if (this.Opacity < Properties.Settings.Default.Opacity) {
                this.Opacity += 0.05;
                rn = this.Opacity >= Properties.Settings.Default.Opacity;
            } else {
                this.Opacity -= 0.05;
                rn = this.Opacity <= Properties.Settings.Default.Opacity;
            }

            if (rn) {
                this.Opacity = Properties.Settings.Default.Opacity;
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

        // buttonOption_Click menuItemXXX_Click
        #region 弹出菜单

        /// <summary>
        /// 弹出菜单
        /// </summary>
        private void buttonOption_Click(object sender, EventArgs e) {
            Computer My = new Computer();
            Point senderPnt = PointToScreen((sender as Button).Location);

            if (My.Screen.WorkingArea.Height - this.Top - (sender as Button).Top - (sender as Button).Height < contextMenuStrip.Height)
                contextMenuStrip.Show(
                    senderPnt.X,
                    senderPnt.Y - contextMenuStrip.Height
                );
            else
                contextMenuStrip.Show(
                    senderPnt.X,
                    senderPnt.Y + (sender as Button).Height
                );
        }

        /// <summary>
        /// 菜单，所有歌词
        /// </summary>
        private void menuItemAllLyric_Click(object sender, EventArgs e) {

            if (Global.MusicLyricPage == null) {
                MessageBox.Show("歌曲：\"" + Global.currentSong.artist + " - " + Global.currentSong.title + "\" 找不到歌词", 
                    "所有歌词", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form form = new Form();
            form.Text = "所有歌词 - " + Global.currentSong.title;
            form.Name = "LyricDialog";
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Size = new System.Drawing.Size(750, 450);

            TextBox textBox = new TextBox();
            textBox.Name = "textBox";
            textBox.Text = Global.MusicLyricPage.ToString();
            textBox.Multiline = true;
            textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBox.WordWrap = false;
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            textBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBox.Select(0, 0);

            form.Controls.Add(textBox);
            textBox.Dock = DockStyle.Fill;

            form.Show();
        }

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

            foreach (ToolStripMenuItem item in menuItemOpacity.DropDownItems)
                item.Checked = false;
            (sender as ToolStripMenuItem).Checked = true;

            Properties.Settings.Default.Opacity = (double)(sender as ToolStripMenuItem).Tag;
            Properties.Settings.Default.Save();
            timerShow.Enabled = true;
        }

        /// <summary>
        /// 菜单，修改文字颜色
        /// </summary>
        private void menuItemForeColor_Click(object sender, EventArgs e) {
            colorDialog.Color = labelLyric.ForeColor;
            colorDialog.ShowDialog();
            labelLyric.ForeColor = colorDialog.Color;
            Properties.Settings.Default.LyricForeColor = labelLyric.ForeColor;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 菜单，修改背景颜色
        /// </summary>
        private void menuForeBackColor_Click(object sender, EventArgs e) {
            colorDialog.Color = this.BackColor;
            colorDialog.ShowDialog();
            this.BackColor = colorDialog.Color;
            Properties.Settings.Default.LyricBackColor = labelLyric.BackColor;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 菜单，恢复颜色
        /// </summary>
        private void menuItemRestoreColor_Click(object sender, EventArgs e) {
            labelLyric.ForeColor = Color.White;
            this.BackColor = Color.Black;
            Properties.Settings.Default.LyricForeColor = labelLyric.ForeColor;
            Properties.Settings.Default.LyricBackColor = labelLyric.BackColor;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 显示主界面
        /// </summary>
        private void menuItemShowMain_Click(object sender, EventArgs e) {
            MainForm.GetInstance().Show();
            MainForm.GetInstance().Activate();
        }

        #endregion // 弹出菜单

        // timerLabelText_Tick labelLyric_TextChanged
        #region 歌词动画

        /// <summary>
        /// 后歌词内容，timerLabelText_Tick 用
        /// </summary>
        public string currentToLyricContext = "";

        /// <summary>
        /// 变换前部
        /// </summary>
        bool isFront = true;

        /// <summary>
        /// 文字颜色变换速度
        /// </summary>
        int colorRate = 20;

        /// <summary>
        /// 歌词文字变化动画
        /// </summary>
        private void timerLabelText_Tick(object sender, EventArgs e) {
            int R = labelLyric.ForeColor.R;
            int G = labelLyric.ForeColor.G;
            int B = labelLyric.ForeColor.B;
            Color dest = isFront ? this.BackColor : Properties.Settings.Default.LyricForeColor;
            R = (R == dest.R) ? R : ((R > dest.R) ? (R - colorRate <= 0 ? 0 : R - colorRate) : (R + colorRate >= 255 ? 255 : R + colorRate));
            G = (G == dest.G) ? G : ((G > dest.G) ? (G - colorRate <= 0 ? 0 : G - colorRate) : (G + colorRate >= 255 ? 255 : G + colorRate));
            B = (B == dest.B) ? B : ((B > dest.B) ? (B - colorRate <= 0 ? 0 : B - colorRate) : (B + colorRate >= 255 ? 255 : B + colorRate));
            labelLyric.ForeColor = Color.FromArgb(R, G, B);

            if (Math.Abs(R - dest.R) < colorRate && Math.Abs(G - dest.G) < colorRate && Math.Abs(B - dest.B) < colorRate) {
                if (isFront) {
                    isFront = false;
                   labelLyric.Text = currentToLyricContext;
                } else {
                    timerLabelText.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 文字变化
        /// </summary>
        /// <param name="to">变化为的文字</param>
        private void changeText(string to) {
            currentToLyricContext = to;
            isFront = true;
            timerLabelText.Enabled = true;
        }

        #endregion // 歌词动画

        /// <summary>
        /// 当前歌曲Id，updateSongLyric 用
        /// </summary>
        public long currentSongId = -1;

        /// <summary>
        /// 当前歌词行， timerLyric_Tick 用
        /// </summary>
        public int currentLineIdx = -1;

        /// <summary>
        /// 更新当前歌曲歌词
        /// </summary>
        public void updateSongLyric(bool isSearching) {
            if (isSearching) {
                changeText("正在搜索歌词");
                return;
            }
            if (Global.MusicId == -1 || Global.MusicLyricPage == null)
                changeText("未找到歌词");
            else
                currentSongId = Global.MusicId;
        }

        /// <summary>
        /// 主计时器，歌词和窗口
        /// </summary>
        private void timerLyric_Tick() {

            // 窗口
            if (menuItemLock.Checked)
                CommonUtil.setWindowCrossOver(this, this.Opacity, !CommonUtil.ControlInRange(this, buttonOption));

            // 歌词显示
            if (Global.MusicLyricPage == null) return;

            if (currentLineIdx == -1) {
                // 正文前
                if (Global.currentSong.title != labelLyric.Text)
                    changeText(Global.currentSong.title);
                if (Global.currentPos >= Global.MusicLyricPage.Lines.ElementAt(0).timeDuration)
                    currentLineIdx++;
            } else {

                // 已经超过
                if (currentLineIdx > Global.MusicLyricPage.Lines.Count - 1) {
                    currentLineIdx--;
                    return;
                }

                // 不超过，获取正文
                LyricLine currLyric = Global.MusicLyricPage.Lines.ElementAt(currentLineIdx);
                if (currentLineIdx == Global.MusicLyricPage.Lines.Count - 1) {
                    // 最后一行
                    if (currLyric.Lyric != labelLyric.Text)
                        changeText(currLyric.Lyric);
                } else {
                    // 非最后一行
                    LyricLine nextLyric = Global.MusicLyricPage.Lines.ElementAt(currentLineIdx + 1);
                    if (Global.currentPos >= currLyric.timeDuration && Global.currentPos <= nextLyric.timeDuration) {
                        // 到达行内
                        if (currLyric.Lyric != labelLyric.Text)
                            changeText(currLyric.Lyric);
                    } else if (Global.currentPos >= nextLyric.timeDuration) {
                        // 下一行
                        currentLineIdx++;
                    } else if (Global.currentPos <= currLyric.timeDuration) {
                        // 时间过快，上一行
                        currentLineIdx--;
                    }
                }
            }
        }
    }
}

