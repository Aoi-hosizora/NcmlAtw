using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace NcmlAtwServer {

    public partial class LyricForm : Form {

        public LyricForm() {
            InitializeComponent();
        }

        private static LyricForm _instance;

        public bool IsShown { get; set; }

        public static LyricForm Instance {
            get {
                if (_instance == null) {
                    _instance = new LyricForm();
                }
                return _instance;
            }
        }

        private void LyricForm_Load(object sender, EventArgs e) {
            var size = Properties.Settings.Default.LyricSize;
            var top = Properties.Settings.Default.LyricTop;
            var left = Properties.Settings.Default.LyricLeft;
            Size = size.Width != -1 && size.Height != -1 ? size : new Size(Screen.PrimaryScreen.WorkingArea.Width - 100, 120);
            Top = top != -1 ? top : Screen.PrimaryScreen.WorkingArea.Height - Height;
            Left = left != -1 ? left : (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2;

            cmsOption.Renderer = new NativeRenderer();
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

            miLockWindow.Checked = Properties.Settings.Default.LyricLock;
            miAdjustOffset.Checked = Properties.Settings.Default.LyricAdjustOffset;
            for (int i = 1; i <= 10; i++) {
                var item = new ToolStripMenuItem {
                    Tag = i / 10.0,
                    Text = (i != 10) ? $"&{i}0%" : "1&00%"
                };
                item.Click += MiOpacity_Click;
                miOpacity.DropDownItems.Add(item);
                if (Properties.Settings.Default.LyricOpacity == i / 10.0) {
                    item.Checked = true;
                }
            }

            Utils.SetWindowCrossOver(this, Opacity, false);
            tmrShow.Start();
        }

        private void LyricForm_FormClosing(object sender, FormClosingEventArgs e) {
            IsShown = false;
            e.Cancel = Opacity != 0;
            tmrShow.Stop();
            tmrHide.Start();
        }

        private void LyricForm_FormClosed(object sender, FormClosedEventArgs e) {
            _instance = null;
            Properties.Settings.Default.LyricTop = Top;
            Properties.Settings.Default.LyricLeft = Left;
            Properties.Settings.Default.LyricSize = Size;
            Properties.Settings.Default.Save();
        }

        private void TmrShow_Tick(object sender, EventArgs e) {
            bool finished;
            if (Opacity < Properties.Settings.Default.LyricOpacity) {
                Opacity += 0.05;
                finished = Opacity >= Properties.Settings.Default.LyricOpacity;
            } else {
                Opacity -= 0.05;
                finished = Opacity <= Properties.Settings.Default.LyricOpacity;
            }

            if (finished) {
                IsShown = true;
                Opacity = Properties.Settings.Default.LyricOpacity;
                Utils.SetWindowCrossOver(this, Opacity, Properties.Settings.Default.LyricLock);
                tmrShow.Stop();
            }
        }

        private void TmrHide_Tick(object sender, EventArgs e) {
            Utils.SetWindowCrossOver(this, Opacity, false);
            Opacity -= 0.05;
            if (Opacity <= 0) {
                Opacity = 0;
                Close();
                tmrHide.Stop();
            }
        }

        private Point _mouseDownCursorPosition;
        private Point _mouseDownWindowPosition;
        private Size _mouseDownWindowSize;
        private MouseBorder _mouseDownBorder;
        private const int BORDER_SIZE = 5;
        private enum MouseBorder {
            N, S, W, E, None, Unset
        }

        private void Object_MouseDown(object sender, MouseEventArgs e) {
            _mouseDownCursorPosition = Cursor.Position;
            _mouseDownWindowPosition = new Point(Left, Top);
            _mouseDownWindowSize = Size;
            _mouseDownBorder = MouseBorder.Unset;
            (sender as Control).Cursor = Cursors.Default;
        }

        private void Object_MouseUp(object sender, MouseEventArgs e) {
            _mouseDownBorder = MouseBorder.Unset;
            (sender as Control).Cursor = Cursors.Default;
        }

        private void Object_MouseMove(object sender, MouseEventArgs e) {
            var ctrl = sender as Control;
            bool isBtn = sender is Button;
            bool isCollectMouseBtn = e.Button == MouseButtons.Left;
            if (isBtn) {
                isCollectMouseBtn = e.Button == MouseButtons.Right;
            }

            // cursor
            if (e.Button == MouseButtons.None || _mouseDownBorder == MouseBorder.Unset) {
                if (isBtn) {
                    _mouseDownBorder = MouseBorder.None;
                    ctrl.Cursor = Cursors.Default;
                } else {
                    var p = Cursor.Position;
                    bool inX = p.X >= Left && p.X <= Left + Width;
                    bool inY = p.Y >= Top && p.Y <= Top + Height;
                    bool onN = inX && p.Y >= Top - BORDER_SIZE && p.Y <= Top + BORDER_SIZE;
                    bool onS = inX && p.Y >= Top + Height - BORDER_SIZE && p.Y <= Top + Height + BORDER_SIZE;
                    bool onW = inY && p.X >= Left - BORDER_SIZE && p.X <= Left + BORDER_SIZE;
                    bool onE = inY && p.X >= Left + Width - BORDER_SIZE && p.X <= Left + Width + BORDER_SIZE;
                    if (onN) {
                        _mouseDownBorder = MouseBorder.N;
                        ctrl.Cursor = Cursors.SizeNS;
                    } else if (onS) {
                        _mouseDownBorder = MouseBorder.S;
                        ctrl.Cursor = Cursors.SizeNS;
                    } else if (onW) {
                        _mouseDownBorder = MouseBorder.W;
                        ctrl.Cursor = Cursors.SizeWE;
                    } else if (onE) {
                        _mouseDownBorder = MouseBorder.E;
                        ctrl.Cursor = Cursors.SizeWE;
                    } else {
                        _mouseDownBorder = MouseBorder.None;
                        ctrl.Cursor = Cursors.Default;
                    }
                }
            }

            // move & resize
            if (isCollectMouseBtn) {
                int newTop = _mouseDownWindowPosition.Y + Cursor.Position.Y - _mouseDownCursorPosition.Y;
                int newLeft = _mouseDownWindowPosition.X + Cursor.Position.X - _mouseDownCursorPosition.X;
                int newHeightN = _mouseDownWindowSize.Height + _mouseDownCursorPosition.Y - Cursor.Position.Y;
                int newHeightS = _mouseDownWindowSize.Height - _mouseDownCursorPosition.Y + Cursor.Position.Y;
                int newWidthW = _mouseDownWindowSize.Width + _mouseDownCursorPosition.X - Cursor.Position.X;
                int newWidthE = _mouseDownWindowSize.Width - _mouseDownCursorPosition.X + Cursor.Position.X;

                switch (_mouseDownBorder) {
                case MouseBorder.None:
                    Top = newTop;
                    Left = newLeft;
                    ctrl.Cursor = Cursors.SizeAll;
                    break;
                case MouseBorder.N:
                    int minHeight = MinimumSize.Height, maxHeight = MaximumSize.Height;
                    if ((minHeight == 0 || newHeightN > minHeight) && (maxHeight == 0 || newHeightN < maxHeight)) {
                        Top = newTop;
                        Height = newHeightN;
                    }
                    break;
                case MouseBorder.S:
                    Height = newHeightS;
                    break;
                case MouseBorder.W:
                    int minWidth = MinimumSize.Width, maxWidth = MaximumSize.Width;
                    if ((minWidth == 0 || newWidthW > minWidth) && (maxWidth == 0 || newWidthW < maxWidth)) {
                        Left = newLeft;
                        Width = newWidthW;
                    }
                    break;
                case MouseBorder.E:
                    Width = newWidthE;
                    break;
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

        private void MiOpacity_Click(object sender, EventArgs e) {
            foreach (ToolStripMenuItem item in miOpacity.DropDownItems) {
                item.Checked = false;
            }
            (sender as ToolStripMenuItem).Checked = true;

            Properties.Settings.Default.LyricOpacity = (double) (sender as ToolStripMenuItem).Tag;
            Properties.Settings.Default.Save();
            tmrShow.Enabled = true;
        }

        private void MiAdjustOffset_Click(object sender, EventArgs e) {
            miAdjustOffset.Checked = !miAdjustOffset.Checked;
            Properties.Settings.Default.LyricAdjustOffset = miAdjustOffset.Checked;
            Properties.Settings.Default.Save();
            btnSlower.Visible = miAdjustOffset.Checked;
            btnFaster.Visible = miAdjustOffset.Checked;
        }

        private void BtnSlower_Click(object sender, EventArgs e) {
            Global.Offset -= 0.5; // -0.5s
        }

        private void BtnFaster_Click(object sender, EventArgs e) {
            Global.Offset += 0.5; // +0.5s
        }

        private void BtnOption_Click(object sender, EventArgs e) {
            Point senderPnt = PointToScreen((sender as Button).Location);
            if (Screen.PrimaryScreen.WorkingArea.Height - Top - btnOption.Top - btnOption.Height < cmsOption.Height) {
                cmsOption.Show(senderPnt.X, senderPnt.Y - cmsOption.Height);
            } else {
                cmsOption.Show(senderPnt.X, senderPnt.Y + btnOption.Height);
            }
        }

        public void GlobalTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if (Properties.Settings.Default.LyricLock) {
                var onBtn = Utils.ControlInRange(this, btnOption) || Utils.ControlInRange(this, btnFaster) || Utils.ControlInRange(this, btnSlower);
                Utils.SetWindowCrossOver(this, Opacity, !onBtn);
            } else {
                Utils.SetWindowCrossOver(this, Opacity, false);
            }
        }
    }
}
