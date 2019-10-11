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
            this.Opacity = 0;
        }

        // LyricForm_Load LyricForm_FormClosing
        #region 加载与退出

        private void LyricForm_Load(object sender, EventArgs e) {

            contextMenuStrip.Renderer = new NativeRenderer(NativeRenderer.ToolbarTheme.MediaToolbar);

            this.BackColor = Properties.Settings.Default.LyricBackColor;
            labelLyric.ForeColor = Properties.Settings.Default.LyricForeColor;

            menuItemAdjust.Checked = !Properties.Settings.Default.isAdjustLyric;
            menuItemAdjust_Click(menuItemAdjust, new EventArgs());

            menuItemLock.Checked = !Properties.Settings.Default.isLock;
            menuItemLock_Click(menuItemLock, new EventArgs());

            labelLyric.Font = Properties.Settings.Default.LyricFont;

            // 窗口位置
            menuItemPosition_Click(menuItemPosition, e);
            if (Properties.Settings.Default.LyricTop != -1)
                this.Top = Properties.Settings.Default.LyricTop;
            if (Properties.Settings.Default.LyricLeft != -1)
                this.Left = Properties.Settings.Default.LyricLeft;
            if (!Properties.Settings.Default.LyricSize.Equals(new Size(-1, -1)))
                this.Size = Properties.Settings.Default.LyricSize;

            // 订阅移动窗口事件
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Object_MouseUp);
            foreach (Control o in this.Controls) {
                o.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Object_MouseDown);
                o.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Object_MouseMove);
                o.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Object_MouseUp);
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
        public void LyricForm_FormClosing(object sender, FormClosingEventArgs e) {
            Properties.Settings.Default.LyricTop = this.Top;
            Properties.Settings.Default.LyricLeft = this.Left;
            Properties.Settings.Default.LyricSize = this.Size;
            Properties.Settings.Default.Save();

            e.Cancel = this.Opacity != 0;
            if (timerShow.Enabled) timerShow.Enabled = false;
            timerHide.Enabled = true;
            Global.LyricFormTimer = null;
        }

        /// <summary>
        /// 窗口高度
        /// </summary>
        private void labelLyric_FontChanged(object sender, EventArgs e) {
            // 48 -> 100
            // this.Height = (int)((100.0 * labelLyric.Font.Size) / 48.0);
        }

        #endregion // 加载与退出

        // Object_MouseDown Object_MouseMove timerShow_Tick timerHide_Tick
        #region 窗口设置

        private Point MouseDownMousePosition;
        private Point MouseDownWinPosition;
        private Size MouseDownWinSize;

        private const int BorderSizing = 5;
        private MouseBorder MouseDownBorder;
        private bool isBorderSizing;

        private enum MouseBorder {
            N, E, S, W,
            NE, NW, SE, SW,
            None
        }

        private void Object_MouseDown(object sender, MouseEventArgs e) {
            MouseDownMousePosition = Cursor.Position;
            MouseDownWinPosition = new Point(this.Left, this.Top);
            MouseDownWinSize = this.Size;

            MouseDownBorder = MouseBorder.None;
            isBorderSizing = false;
        }

        private void Object_MouseUp(object sender, MouseEventArgs e) {
            isBorderSizing = false;
            (sender as Control).Cursor = System.Windows.Forms.Cursors.Default;
        }

        /// <summary>
        /// 按钮右键移动，其他左键移动和调整大小
        /// </summary>
        private void Object_MouseMove(object sender, MouseEventArgs e) {
            bool rn = e.Button == MouseButtons.Left;
            if (sender.GetType().Equals(typeof(Button)))
                rn = e.Button == MouseButtons.Right;

            if (!isBorderSizing && !sender.GetType().Equals(typeof(Button))) {
                bool N = Cursor.Position.Y >= this.Top - BorderSizing && Cursor.Position.Y <= this.Top + BorderSizing;
                bool S = Cursor.Position.Y >= this.Top + this.Height - BorderSizing  && Cursor.Position.Y <= this.Top + this.Height + BorderSizing;
                bool E = Cursor.Position.X >= this.Left - BorderSizing && Cursor.Position.X <= this.Left + BorderSizing;
                bool W = Cursor.Position.X >= this.Left + this.Width - BorderSizing && Cursor.Position.X <= this.Left + this.Width + BorderSizing;

                bool InX = Cursor.Position.X >= this.Left && Cursor.Position.X <= this.Left + this.Width;
                bool InY = Cursor.Position.Y >= this.Top && Cursor.Position.Y <= this.Top + this.Height;

                bool isBorderN = InX && N;
                bool isBorderS = InX && S;
                bool isBorderE = InY && E;
                bool isBorderW = InY && W;

                bool isBorderNE = N && E;
                bool isBorderNW = N && W;
                bool isBorderSE = S && E;
                bool isBorderSW = S && W;
                bool IsNotBorder = InX && InY && !N && !S && !E && !W;

                if (isBorderNE) MouseDownBorder = MouseBorder.NE;
                else if (isBorderNW) MouseDownBorder = MouseBorder.NW;
                else if (isBorderSE) MouseDownBorder = MouseBorder.SE;
                else if (isBorderSW) MouseDownBorder = MouseBorder.SW;
                else if (isBorderN) MouseDownBorder = MouseBorder.N;
                else if (isBorderS) MouseDownBorder = MouseBorder.S;
                else if (isBorderE) MouseDownBorder = MouseBorder.E;
                else if (isBorderW) MouseDownBorder = MouseBorder.W;
                else if (IsNotBorder) MouseDownBorder = MouseBorder.None;

                if (MouseDownBorder == MouseBorder.NE || MouseDownBorder == MouseBorder.SW)
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeNWSE;
                else if (MouseDownBorder == MouseBorder.NW || MouseDownBorder == MouseBorder.SE)
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeNESW;
                else if (MouseDownBorder == MouseBorder.N || MouseDownBorder == MouseBorder.S)
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeNS;
                else if (MouseDownBorder == MouseBorder.E || MouseDownBorder == MouseBorder.W)
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeWE;
                else
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.Default;
            }

            if (rn) {

                int newTop = MouseDownWinPosition.Y + Cursor.Position.Y - MouseDownMousePosition.Y;
                int newLeft = MouseDownWinPosition.X + Cursor.Position.X - MouseDownMousePosition.X;

                int newH_N = MouseDownWinSize.Height + MouseDownMousePosition.Y - Cursor.Position.Y;
                int newH_S = MouseDownWinSize.Height - MouseDownMousePosition.Y + Cursor.Position.Y;
                int newW_W = MouseDownWinSize.Width - MouseDownMousePosition.X + Cursor.Position.X;
                int newW_E = MouseDownWinSize.Width + MouseDownMousePosition.X - Cursor.Position.X;

                if (MouseDownBorder == MouseBorder.None) {
                    this.Top = newTop;
                    this.Left = newLeft;
                    (sender as Control).Cursor = System.Windows.Forms.Cursors.SizeAll;
                } else if (!sender.GetType().Equals(typeof(Button))) {

                    isBorderSizing = MouseDownBorder != MouseBorder.None;

                    int MinH = MinimumSize.Height, MaxH = MaximumSize.Height;
                    int MinW = MinimumSize.Width, MaxW = MaximumSize.Width;

                    if (MouseDownBorder == MouseBorder.NE) {
                        if ((MinH == 0 || newH_N > MinH) && (MaxH == 0 || newH_N < MaxH)) {
                            this.Top = newTop;
                            this.Height = newH_N;
                        }
                        if ((MinW == 0 || newW_E > MinW) && (MaxW == 0 || newW_E < MaxW)) {
                            this.Left = newLeft;
                            this.Width = newW_E;
                        }
                    } else if (MouseDownBorder == MouseBorder.SW) {
                        this.Height = newH_S;
                        this.Width = newW_W;
                    } else if (MouseDownBorder == MouseBorder.NW) {
                        if ((MinH == 0 || newH_N > MinH) && (MaxH == 0 || newH_N < MaxH)) {
                            this.Top = newTop;
                            this.Height = newH_N;
                        }
                        this.Width = newW_W;
                    } else if (MouseDownBorder == MouseBorder.SE) {
                        if ((MinW == 0 || newW_E > MinW) && (MaxW == 0 || newW_E < MaxW)) {
                            this.Left = newLeft;
                            this.Width = newW_E;
                        }
                        this.Height = newH_S;
                    } else if (MouseDownBorder == MouseBorder.N) {
                        if ((MinH == 0 || newH_N > MinH) && (MaxH == 0 || newH_N < MaxH)) {
                            this.Top = newTop;
                            this.Height = newH_N;
                        }
                    } else if (MouseDownBorder == MouseBorder.S) {
                        this.Height = newH_S;
                    } else if (MouseDownBorder == MouseBorder.E) {
                        if ((MinW == 0 || newW_E > MinW) && (MaxW == 0 || newW_E < MaxW)) {
                            this.Left = newLeft;
                            this.Width = newW_E;
                        }
                    } else if (MouseDownBorder == MouseBorder.W) {
                        this.Width = newW_W;
                    }
                }
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
        /// 快 0.5 秒
        /// </summary>
        private void buttonFaster_Click(object sender, EventArgs e) {
            Global.stateUpdateMS -= 500;
        }

        /// <summary>
        /// 慢 0.5 秒
        /// </summary>
        private void buttonSlower_Click(object sender, EventArgs e) {
            Global.stateUpdateMS += 500;
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
        /// 调整快慢
        /// </summary>
        private void menuItemAdjust_Click(object sender, EventArgs e) {
            menuItemAdjust.Checked = !menuItemAdjust.Checked;
            buttonFaster.Visible = buttonSlower.Visible = menuItemAdjust.Checked;
            Properties.Settings.Default.isAdjustLyric = menuItemAdjust.Checked;
        }

        /// <summary>
        /// 菜单，锁定
        /// </summary>
        private void menuItemLock_Click(object sender, EventArgs e) {
            menuItemLock.Checked = !menuItemLock.Checked;
            CommonUtil.setWindowCrossOver(this, this.Opacity, menuItemLock.Checked);
            Properties.Settings.Default.isLock = menuItemLock.Checked;
        }

        /// <summary>
        /// 菜单，原位置
        /// </summary>
        private void menuItemPosition_Click(object sender, EventArgs e) {
            Computer My = new Computer();
            this.Left = (My.Screen.Bounds.Width - this.Width) / 2;
            this.Top = My.Screen.WorkingArea.Height - this.Height;
        }

        /// <summary>
        /// 菜单，原大小
        /// </summary>
        private void menuItemSize_Click(object sender, EventArgs e) {
            Computer My = new Computer();
            int left = this.Left;
            int width = this.Width;
            this.Width = (int)(My.Screen.WorkingArea.Width * 0.8);
            this.Height = 100;
            this.Left = left + width - this.Width;

            Font newFont = new Font(labelLyric.Font.Name, 48F, labelLyric.Font.Style, System.Drawing.GraphicsUnit.Point);
            labelLyric.Font = newFont;
            Properties.Settings.Default.LyricFont = labelLyric.Font;
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
        /// 菜单，字体修改
        /// </summary>
        private void menuItemFont_Click(object sender, EventArgs e) {
            fontDialog.Font = labelLyric.Font;
            fontDialog.ShowDialog();
            Console.WriteLine(fontDialog.Font.Name);
            Properties.Settings.Default.LyricFont = labelLyric.Font = fontDialog.Font;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 菜单，修改文字颜色
        /// </summary>
        private void menuItemForeColor_Click(object sender, EventArgs e) {
            colorDialog.Color = Properties.Settings.Default.LyricForeColor;
            colorDialog.ShowDialog();
            labelLyric.ForeColor = colorDialog.Color;
            Properties.Settings.Default.LyricForeColor = labelLyric.ForeColor;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// 菜单，修改背景颜色
        /// </summary>
        private void menuForeBackColor_Click(object sender, EventArgs e) {
            colorDialog.Color = Properties.Settings.Default.LyricBackColor;
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
        /// 当前歌词行， timerLyric_Tick 用
        /// </summary>
        public int currentLineIdx = -1;

        /// <summary>
        /// 更新当前歌曲歌词
        /// </summary>
        public void updateSongLyric(bool isSearching) {
            currentLineIdx = -1;
            if (isSearching) {
                changeText("正在搜索歌詞...");
                return;
            }
            if (Global.MusicId == -1) {
                changeText("未找到歌曲");
            } else {
                if (Global.MusicLyricState == Global.LyricState.NotFound)
                    changeText("未找到歌詞");
                else if (Global.MusicLyricState == Global.LyricState.PureMusic)
                    changeText("純音樂 請欣賞");
            }
        }

        /// <summary>
        /// 主计时器，歌词和窗口
        /// </summary>
        private void timerLyric_Tick() {

            // 窗口
            if (menuItemLock.Checked)
                CommonUtil.setWindowCrossOver(this, this.Opacity, !(
                    CommonUtil.ControlInRange(this, buttonOption) ||
                    (buttonFaster.Visible && CommonUtil.ControlInRange(this, buttonFaster)) ||
                    (buttonSlower.Visible && CommonUtil.ControlInRange(this, buttonSlower))));
            else
                CommonUtil.setWindowCrossOver(this, this.Opacity, false);

            // 歌词显示
            if (Global.MusicLyricPage == null) return;

            // label1.Text = currentLineIdx.ToString();

            if (currentLineIdx == -1) {
                // 正文前
                if (Global.currentSong.title != labelLyric.Text)
                    changeText(Global.currentSong.title);
                if (Global.currentPos >= Global.MusicLyricPage.Lines.ElementAt(0).timeDuration)
                    currentLineIdx++;
            } else {

                // 正文，已经超过
                if (currentLineIdx > Global.MusicLyricPage.Lines.Count - 1) {
                    currentLineIdx--;
                    return;
                }

                // 正文，不超过
                LyricLine currLyric = Global.MusicLyricPage.Lines.ElementAt(currentLineIdx);
                if (currentLineIdx == Global.MusicLyricPage.Lines.Count - 1) {
                    // 最后一行
                    if (Global.currentPos <= currLyric.timeDuration) {
                        // 时间过快，上一行
                        currentLineIdx--;
                    } else {
                        if (currLyric.Lyric != labelLyric.Text) 
                            // 最后一行刚好
                            changeText(currLyric.Lyric);
                    }
                } else {
                    // 非最后一行
                    LyricLine nextLyric = Global.MusicLyricPage.Lines.ElementAt(currentLineIdx + 1);
                    if (Global.currentPos >= currLyric.timeDuration && Global.currentPos <= nextLyric.timeDuration) {
                        if (currLyric.Lyric != labelLyric.Text) // 到达行内
                            changeText(currLyric.Lyric);
                    } else if (Global.currentPos >= nextLyric.timeDuration) {
                        currentLineIdx++; // 下一行
                    } else if (Global.currentPos <= currLyric.timeDuration) {
                        currentLineIdx--; // 上一行
                    }
                }
            }
        }
    }
}
