namespace NeteaseM2DServer.Src.UI
{
    partial class LyricForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LyricForm));
            this.labelLyric = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemAllLyric = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAdjust = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemLock = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOpacity = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFont = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemForeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuForeBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRestoreColor = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemShowMain = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerShow = new System.Windows.Forms.Timer(this.components);
            this.timerHide = new System.Windows.Forms.Timer(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonOption = new System.Windows.Forms.Button();
            this.buttonSlower = new System.Windows.Forms.Button();
            this.buttonFaster = new System.Windows.Forms.Button();
            this.timerLabelText = new System.Windows.Forms.Timer(this.components);
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelLyric
            // 
            this.labelLyric.ContextMenuStrip = this.contextMenuStrip;
            this.labelLyric.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLyric.Font = new System.Drawing.Font("Yu Gothic UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelLyric.ForeColor = System.Drawing.Color.White;
            this.labelLyric.Location = new System.Drawing.Point(0, 0);
            this.labelLyric.Name = "labelLyric";
            this.labelLyric.Size = new System.Drawing.Size(1350, 100);
            this.labelLyric.TabIndex = 0;
            this.labelLyric.Text = "未找到歌詞";
            this.labelLyric.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelLyric.FontChanged += new System.EventHandler(this.labelLyric_FontChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAllLyric,
            this.menuItemAdjust,
            this.toolStripSeparator3,
            this.menuItemLock,
            this.menuItemOpacity,
            this.menuItemPosition,
            this.menuItemSize,
            this.toolStripSeparator2,
            this.menuItemFont,
            this.menuItemForeColor,
            this.menuForeBackColor,
            this.menuItemRestoreColor,
            this.toolStripSeparator1,
            this.menuItemShowMain,
            this.menuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(154, 286);
            // 
            // menuItemAllLyric
            // 
            this.menuItemAllLyric.Name = "menuItemAllLyric";
            this.menuItemAllLyric.Size = new System.Drawing.Size(153, 22);
            this.menuItemAllLyric.Text = "所有歌词(&L)";
            this.menuItemAllLyric.Click += new System.EventHandler(this.menuItemAllLyric_Click);
            // 
            // menuItemAdjust
            // 
            this.menuItemAdjust.Name = "menuItemAdjust";
            this.menuItemAdjust.Size = new System.Drawing.Size(153, 22);
            this.menuItemAdjust.Text = "调整快慢(&D)";
            this.menuItemAdjust.Click += new System.EventHandler(this.menuItemAdjust_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(150, 6);
            // 
            // menuItemLock
            // 
            this.menuItemLock.Checked = true;
            this.menuItemLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemLock.Name = "menuItemLock";
            this.menuItemLock.Size = new System.Drawing.Size(153, 22);
            this.menuItemLock.Text = "锁定(&L)";
            this.menuItemLock.Click += new System.EventHandler(this.menuItemLock_Click);
            // 
            // menuItemOpacity
            // 
            this.menuItemOpacity.Name = "menuItemOpacity";
            this.menuItemOpacity.Size = new System.Drawing.Size(153, 22);
            this.menuItemOpacity.Text = "透明度(&I)";
            // 
            // menuItemPosition
            // 
            this.menuItemPosition.Name = "menuItemPosition";
            this.menuItemPosition.Size = new System.Drawing.Size(153, 22);
            this.menuItemPosition.Text = "原位置(&P)";
            this.menuItemPosition.Click += new System.EventHandler(this.menuItemPosition_Click);
            // 
            // menuItemSize
            // 
            this.menuItemSize.Name = "menuItemSize";
            this.menuItemSize.Size = new System.Drawing.Size(153, 22);
            this.menuItemSize.Text = "原大小(&S)";
            this.menuItemSize.Click += new System.EventHandler(this.menuItemSize_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
            // 
            // menuItemFont
            // 
            this.menuItemFont.Name = "menuItemFont";
            this.menuItemFont.Size = new System.Drawing.Size(153, 22);
            this.menuItemFont.Text = "修改字体(&F)";
            this.menuItemFont.Click += new System.EventHandler(this.menuItemFont_Click);
            // 
            // menuItemForeColor
            // 
            this.menuItemForeColor.Name = "menuItemForeColor";
            this.menuItemForeColor.Size = new System.Drawing.Size(153, 22);
            this.menuItemForeColor.Text = "文字颜色(&O)";
            this.menuItemForeColor.Click += new System.EventHandler(this.menuItemForeColor_Click);
            // 
            // menuForeBackColor
            // 
            this.menuForeBackColor.Name = "menuForeBackColor";
            this.menuForeBackColor.Size = new System.Drawing.Size(153, 22);
            this.menuForeBackColor.Text = "背景颜色(&B)";
            this.menuForeBackColor.Click += new System.EventHandler(this.menuForeBackColor_Click);
            // 
            // menuItemRestoreColor
            // 
            this.menuItemRestoreColor.Name = "menuItemRestoreColor";
            this.menuItemRestoreColor.Size = new System.Drawing.Size(153, 22);
            this.menuItemRestoreColor.Text = "恢复颜色(&C)";
            this.menuItemRestoreColor.Click += new System.EventHandler(this.menuItemRestoreColor_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // menuItemShowMain
            // 
            this.menuItemShowMain.Name = "menuItemShowMain";
            this.menuItemShowMain.Size = new System.Drawing.Size(153, 22);
            this.menuItemShowMain.Text = "显示主界面(&M)";
            this.menuItemShowMain.Click += new System.EventHandler(this.menuItemShowMain_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(153, 22);
            this.menuItemExit.Text = "关闭歌词(&X)";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // timerShow
            // 
            this.timerShow.Interval = 1;
            this.timerShow.Tick += new System.EventHandler(this.timerShow_Tick);
            // 
            // timerHide
            // 
            this.timerHide.Interval = 1;
            this.timerHide.Tick += new System.EventHandler(this.timerHide_Tick);
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            // 
            // buttonOption
            // 
            this.buttonOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOption.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOption.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonOption.ForeColor = System.Drawing.Color.White;
            this.buttonOption.Image = global::NeteaseM2DServer.Properties.Resources.Option;
            this.buttonOption.Location = new System.Drawing.Point(1320, 5);
            this.buttonOption.Name = "buttonOption";
            this.buttonOption.Size = new System.Drawing.Size(24, 21);
            this.buttonOption.TabIndex = 1;
            this.toolTip.SetToolTip(this.buttonOption, "左键弹出菜单，右键移动歌词。\r\n注意：仅当解除锁定时才能够移动窗口和调整大小。");
            this.buttonOption.UseVisualStyleBackColor = true;
            this.buttonOption.Click += new System.EventHandler(this.buttonOption_Click);
            // 
            // buttonSlower
            // 
            this.buttonSlower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSlower.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonSlower.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonSlower.ForeColor = System.Drawing.Color.White;
            this.buttonSlower.Image = ((System.Drawing.Image)(resources.GetObject("buttonSlower.Image")));
            this.buttonSlower.Location = new System.Drawing.Point(1293, 70);
            this.buttonSlower.Name = "buttonSlower";
            this.buttonSlower.Size = new System.Drawing.Size(24, 24);
            this.buttonSlower.TabIndex = 3;
            this.toolTip.SetToolTip(this.buttonSlower, "歌词放慢 0.5 秒");
            this.buttonSlower.UseVisualStyleBackColor = true;
            this.buttonSlower.Visible = false;
            this.buttonSlower.Click += new System.EventHandler(this.buttonSlower_Click);
            // 
            // buttonFaster
            // 
            this.buttonFaster.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFaster.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonFaster.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFaster.ForeColor = System.Drawing.Color.White;
            this.buttonFaster.Image = ((System.Drawing.Image)(resources.GetObject("buttonFaster.Image")));
            this.buttonFaster.Location = new System.Drawing.Point(1320, 70);
            this.buttonFaster.Name = "buttonFaster";
            this.buttonFaster.Size = new System.Drawing.Size(24, 24);
            this.buttonFaster.TabIndex = 2;
            this.toolTip.SetToolTip(this.buttonFaster, "歌词加快 0.5 秒");
            this.buttonFaster.UseVisualStyleBackColor = true;
            this.buttonFaster.Visible = false;
            this.buttonFaster.Click += new System.EventHandler(this.buttonFaster_Click);
            // 
            // timerLabelText
            // 
            this.timerLabelText.Interval = 1;
            this.timerLabelText.Tick += new System.EventHandler(this.timerLabelText_Tick);
            // 
            // fontDialog
            // 
            this.fontDialog.AllowScriptChange = false;
            this.fontDialog.AllowSimulations = false;
            this.fontDialog.AllowVectorFonts = false;
            this.fontDialog.AllowVerticalFonts = false;
            this.fontDialog.Font = new System.Drawing.Font("Yu Gothic UI", 48F);
            this.fontDialog.ShowEffects = false;
            // 
            // LyricForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1350, 100);
            this.Controls.Add(this.buttonSlower);
            this.Controls.Add(this.buttonFaster);
            this.Controls.Add(this.buttonOption);
            this.Controls.Add(this.labelLyric);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 62);
            this.Name = "LyricForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LyricForm";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LyricForm_FormClosing);
            this.Load += new System.EventHandler(this.LyricForm_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelLyric;
        private System.Windows.Forms.Timer timerShow;
        private System.Windows.Forms.Button buttonOption;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timerLabelText;
        private System.Windows.Forms.Button buttonFaster;
        private System.Windows.Forms.Button buttonSlower;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemAllLyric;
        private System.Windows.Forms.ToolStripMenuItem menuItemAdjust;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuItemLock;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpacity;
        private System.Windows.Forms.ToolStripMenuItem menuItemPosition;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuItemForeColor;
        private System.Windows.Forms.ToolStripMenuItem menuForeBackColor;
        private System.Windows.Forms.ToolStripMenuItem menuItemRestoreColor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowMain;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemFont;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.ToolStripMenuItem menuItemSize;
        private System.Windows.Forms.Timer timerHide;
    }
}