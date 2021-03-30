using System;
using System.Drawing;
using System.Windows.Forms;

namespace NcmlAtwServer {

    public partial class LyricForm : Form {

        public LyricForm() {
            InitializeComponent();
        }

        private static LyricForm _instance;

        public static LyricForm Instance {
            get {
                if (_instance == null) {
                    _instance = new LyricForm();
                }
                return _instance;
            }
        }

        private void LyricForm_Load(object sender, EventArgs e) {
            var top = Properties.Settings.Default.LyricTop;
            var left = Properties.Settings.Default.LyricLeft;
            var size = Properties.Settings.Default.LyricSize;
            if (!size.Equals(new Size(-1, -1))) {
                Size = size;
            } else {
                Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 100, 120);
            }
            if (top != -1) {
                Top = top;
            } else {
                Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
            }
            if (left != -1) {
                Left = left;
            } else {
                Left = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
            }

            cmsOption.Renderer = new NativeRenderer();
            Utils.SetWindowCrossOver(this, Opacity, Properties.Settings.Default.LyricLock);
            MouseDown += Object_MouseDown;
            MouseMove += Object_MouseMove;
            MouseUp += Object_MouseUp;
            foreach (var o in Controls) {
                if (o is Control ctrl) {
                    ctrl.MouseDown += Object_MouseDown;
                    ctrl.MouseMove += Object_MouseMove;
                    ctrl.MouseUp += Object_MouseUp;
                }
            }

            tmrShow.Start();
        }

        private void LyricForm_FormClosed(object sender, FormClosedEventArgs e) {
            _instance = null;
            Properties.Settings.Default.LyricTop = Top;
            Properties.Settings.Default.LyricLeft = Left;
            Properties.Settings.Default.LyricSize = Size;
            Properties.Settings.Default.Save();
        }

        private void LyricForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = Opacity != 0;
            tmrShow.Stop();
            tmrHide.Start();
        }

        private void TmrShow_Tick(object sender, EventArgs e) {
            Utils.SetWindowCrossOver(this, 0, false);
            Opacity += 0.05;
            if (Opacity > Properties.Settings.Default.LyricOpacity) {
                Opacity = Properties.Settings.Default.LyricOpacity;
                Utils.SetWindowCrossOver(this, Opacity, Properties.Settings.Default.LyricLock);
                tmrShow.Stop();
            }
        }

        private void TmrHide_Tick(object sender, EventArgs e) {
            Utils.SetWindowCrossOver(this, 0, false);
            Opacity -= 0.05;
            if (Opacity <= 0) {
                Opacity = 0;
                Close();
                tmrHide.Stop();
            }
        }

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

        private void Object_MouseMove(object sender, MouseEventArgs e) {
            isBorderSizing = false;
            (sender as Control).Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void Object_MouseUp(object sender, MouseEventArgs e) {
            bool rn = e.Button == MouseButtons.Left;
            if (sender.GetType().Equals(typeof(Button)))
                rn = e.Button == MouseButtons.Right;

            if (!isBorderSizing && !sender.GetType().Equals(typeof(Button))) {
                bool N = Cursor.Position.Y >= this.Top - BorderSizing && Cursor.Position.Y <= this.Top + BorderSizing;
                bool S = Cursor.Position.Y >= this.Top + this.Height - BorderSizing && Cursor.Position.Y <= this.Top + this.Height + BorderSizing;
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

        private void MiExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void MiShowMainWindow_Click(object sender, EventArgs e) {
            MainForm.Instance.Activate();
        }

        private void MiRestoreWindow_Click(object sender, EventArgs e) {
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width - 100, 120);
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height;
            Left = (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;
        }

        private void MiLockWindow_Click(object sender, EventArgs e) {
            miLockWindow.Checked = !miLockWindow.Checked;
            Properties.Settings.Default.LyricLock = miLockWindow.Checked;
            Properties.Settings.Default.Save();
            Utils.SetWindowCrossOver(this, Opacity, Properties.Settings.Default.LyricLock);
        }

        private void BtnOption_Click(object sender, EventArgs e) {

        }
    }
}
