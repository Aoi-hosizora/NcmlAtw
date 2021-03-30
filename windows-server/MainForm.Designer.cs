
namespace NcmlAtwServer {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.lblIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.btnQrcode = new System.Windows.Forms.Button();
            this.lblDuration = new System.Windows.Forms.Label();
            this.btnAdjustOffset = new System.Windows.Forms.Button();
            this.btnListen = new System.Windows.Forms.Button();
            this.btnShowLyric = new System.Windows.Forms.Button();
            this.btnVisitLink = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTitleHint = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmsText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.lblAlbumHint = new System.Windows.Forms.Label();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.lblArtistHint = new System.Windows.Forms.Label();
            this.lblArtist = new System.Windows.Forms.Label();
            this.edtIP = new System.Windows.Forms.TextBox();
            this.cbbInterface = new System.Windows.Forms.ComboBox();
            this.gpbNetwork = new System.Windows.Forms.GroupBox();
            this.gpbMusic = new System.Windows.Forms.GroupBox();
            this.lblLinkHint = new System.Windows.Forms.Label();
            this.lblLink = new System.Windows.Forms.Label();
            this.cmsAdjustOffset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miOffsetText = new System.Windows.Forms.ToolStripMenuItem();
            this.miResetOffset = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAdjustOffset = new System.Windows.Forms.ToolStripSeparator();
            this.miFastHalfHalfSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.miSlowHalfHalfSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.miFastHalfSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.miSlowHalfSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.miFastSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.miSlowSecond = new System.Windows.Forms.ToolStripMenuItem();
            this.tipText = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.cmsText.SuspendLayout();
            this.gpbNetwork.SuspendLayout();
            this.gpbMusic.SuspendLayout();
            this.cmsAdjustOffset.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(6, 55);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(155, 17);
            this.lblIP.TabIndex = 2;
            this.lblIP.Text = "当前局域网内的 IPv4 地址 :";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 83);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(171, 17);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "当前监听的端口 (安卓端通信) :";
            // 
            // numPort
            // 
            this.numPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numPort.Location = new System.Drawing.Point(183, 81);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(68, 23);
            this.numPort.TabIndex = 5;
            this.numPort.Value = new decimal(new int[] {
            12122,
            0,
            0,
            0});
            // 
            // btnQrcode
            // 
            this.btnQrcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQrcode.Enabled = false;
            this.btnQrcode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnQrcode.Location = new System.Drawing.Point(257, 80);
            this.btnQrcode.Name = "btnQrcode";
            this.btnQrcode.Size = new System.Drawing.Size(75, 25);
            this.btnQrcode.TabIndex = 6;
            this.btnQrcode.Text = "二维码";
            this.btnQrcode.UseVisualStyleBackColor = true;
            this.btnQrcode.Click += new System.EventHandler(this.BtnQrcode_Click);
            // 
            // lblDuration
            // 
            this.lblDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDuration.AutoEllipsis = true;
            this.lblDuration.Location = new System.Drawing.Point(6, 26);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(245, 17);
            this.lblDuration.TabIndex = 8;
            this.lblDuration.Text = "未监听...";
            // 
            // btnAdjustOffset
            // 
            this.btnAdjustOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdjustOffset.Enabled = false;
            this.btnAdjustOffset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdjustOffset.Location = new System.Drawing.Point(257, 22);
            this.btnAdjustOffset.Name = "btnAdjustOffset";
            this.btnAdjustOffset.Size = new System.Drawing.Size(75, 25);
            this.btnAdjustOffset.TabIndex = 9;
            this.btnAdjustOffset.Text = "调整时间";
            this.btnAdjustOffset.UseVisualStyleBackColor = true;
            this.btnAdjustOffset.Click += new System.EventHandler(this.BtnAdjustOffset_Click);
            // 
            // btnListen
            // 
            this.btnListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnListen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnListen.Location = new System.Drawing.Point(12, 287);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(75, 25);
            this.btnListen.TabIndex = 18;
            this.btnListen.Text = "开始监听";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.BtnListen_Click);
            // 
            // btnShowLyric
            // 
            this.btnShowLyric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowLyric.Enabled = false;
            this.btnShowLyric.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnShowLyric.Location = new System.Drawing.Point(93, 287);
            this.btnShowLyric.Name = "btnShowLyric";
            this.btnShowLyric.Size = new System.Drawing.Size(95, 25);
            this.btnShowLyric.TabIndex = 19;
            this.btnShowLyric.Text = "显示桌面歌词";
            this.btnShowLyric.UseVisualStyleBackColor = true;
            this.btnShowLyric.Click += new System.EventHandler(this.BtnShowLyric_Click);
            // 
            // btnVisitLink
            // 
            this.btnVisitLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVisitLink.Enabled = false;
            this.btnVisitLink.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnVisitLink.Location = new System.Drawing.Point(194, 287);
            this.btnVisitLink.Name = "btnVisitLink";
            this.btnVisitLink.Size = new System.Drawing.Size(75, 25);
            this.btnVisitLink.TabIndex = 20;
            this.btnVisitLink.Text = "打开链接";
            this.btnVisitLink.UseVisualStyleBackColor = true;
            this.btnVisitLink.Click += new System.EventHandler(this.BtnVisitLink_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExit.Location = new System.Drawing.Point(275, 287);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 21;
            this.btnExit.Text = "退出";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // lblTitleHint
            // 
            this.lblTitleHint.AutoSize = true;
            this.lblTitleHint.Location = new System.Drawing.Point(6, 57);
            this.lblTitleHint.Name = "lblTitleHint";
            this.lblTitleHint.Size = new System.Drawing.Size(39, 17);
            this.lblTitleHint.TabIndex = 10;
            this.lblTitleHint.Text = "标题 :";
            this.lblTitleHint.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.ContextMenuStrip = this.cmsText;
            this.lblTitle.Location = new System.Drawing.Point(41, 57);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(291, 17);
            this.lblTitle.TabIndex = 11;
            this.lblTitle.Text = "未知标题";
            this.lblTitle.Visible = false;
            // 
            // cmsText
            // 
            this.cmsText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopy});
            this.cmsText.Name = "cmsAdjustOffset";
            this.cmsText.Size = new System.Drawing.Size(114, 26);
            // 
            // miCopy
            // 
            this.miCopy.Name = "miCopy";
            this.miCopy.Size = new System.Drawing.Size(113, 22);
            this.miCopy.Text = "复制(&C)";
            this.miCopy.Click += new System.EventHandler(this.MiCopy_Click);
            // 
            // lblAlbumHint
            // 
            this.lblAlbumHint.AutoSize = true;
            this.lblAlbumHint.Location = new System.Drawing.Point(6, 80);
            this.lblAlbumHint.Name = "lblAlbumHint";
            this.lblAlbumHint.Size = new System.Drawing.Size(39, 17);
            this.lblAlbumHint.TabIndex = 12;
            this.lblAlbumHint.Text = "专辑 :";
            this.lblAlbumHint.Visible = false;
            // 
            // lblAlbum
            // 
            this.lblAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlbum.AutoEllipsis = true;
            this.lblAlbum.ContextMenuStrip = this.cmsText;
            this.lblAlbum.Location = new System.Drawing.Point(41, 80);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(291, 17);
            this.lblAlbum.TabIndex = 13;
            this.lblAlbum.Text = "未知专辑";
            this.lblAlbum.Visible = false;
            // 
            // lblArtistHint
            // 
            this.lblArtistHint.AutoSize = true;
            this.lblArtistHint.Location = new System.Drawing.Point(6, 103);
            this.lblArtistHint.Name = "lblArtistHint";
            this.lblArtistHint.Size = new System.Drawing.Size(39, 17);
            this.lblArtistHint.TabIndex = 14;
            this.lblArtistHint.Text = "歌手 :";
            this.lblArtistHint.Visible = false;
            // 
            // lblArtist
            // 
            this.lblArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArtist.AutoEllipsis = true;
            this.lblArtist.ContextMenuStrip = this.cmsText;
            this.lblArtist.Location = new System.Drawing.Point(41, 103);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(291, 17);
            this.lblArtist.TabIndex = 15;
            this.lblArtist.Text = "未知歌手";
            this.lblArtist.Visible = false;
            // 
            // edtIP
            // 
            this.edtIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtIP.Location = new System.Drawing.Point(167, 52);
            this.edtIP.Name = "edtIP";
            this.edtIP.ReadOnly = true;
            this.edtIP.Size = new System.Drawing.Size(165, 23);
            this.edtIP.TabIndex = 3;
            this.edtIP.Text = "127.0.0.1";
            // 
            // cbbInterface
            // 
            this.cbbInterface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbInterface.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbbInterface.FormattingEnabled = true;
            this.cbbInterface.ItemHeight = 17;
            this.cbbInterface.Location = new System.Drawing.Point(6, 22);
            this.cbbInterface.Name = "cbbInterface";
            this.cbbInterface.Size = new System.Drawing.Size(326, 25);
            this.cbbInterface.TabIndex = 1;
            this.cbbInterface.SelectedIndexChanged += new System.EventHandler(this.CbbInterface_SelectedIndexChanged);
            // 
            // gpbNetwork
            // 
            this.gpbNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbNetwork.Controls.Add(this.cbbInterface);
            this.gpbNetwork.Controls.Add(this.lblIP);
            this.gpbNetwork.Controls.Add(this.edtIP);
            this.gpbNetwork.Controls.Add(this.lblPort);
            this.gpbNetwork.Controls.Add(this.numPort);
            this.gpbNetwork.Controls.Add(this.btnQrcode);
            this.gpbNetwork.Location = new System.Drawing.Point(12, 12);
            this.gpbNetwork.Name = "gpbNetwork";
            this.gpbNetwork.Size = new System.Drawing.Size(338, 110);
            this.gpbNetwork.TabIndex = 0;
            this.gpbNetwork.TabStop = false;
            this.gpbNetwork.Text = "网络监听";
            // 
            // gpbMusic
            // 
            this.gpbMusic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbMusic.Controls.Add(this.lblDuration);
            this.gpbMusic.Controls.Add(this.btnAdjustOffset);
            this.gpbMusic.Controls.Add(this.lblTitle);
            this.gpbMusic.Controls.Add(this.lblTitleHint);
            this.gpbMusic.Controls.Add(this.lblAlbumHint);
            this.gpbMusic.Controls.Add(this.lblLinkHint);
            this.gpbMusic.Controls.Add(this.lblArtistHint);
            this.gpbMusic.Controls.Add(this.lblLink);
            this.gpbMusic.Controls.Add(this.lblArtist);
            this.gpbMusic.Controls.Add(this.lblAlbum);
            this.gpbMusic.Location = new System.Drawing.Point(12, 128);
            this.gpbMusic.Name = "gpbMusic";
            this.gpbMusic.Size = new System.Drawing.Size(338, 153);
            this.gpbMusic.TabIndex = 7;
            this.gpbMusic.TabStop = false;
            this.gpbMusic.Text = "当前歌曲";
            // 
            // lblLinkHint
            // 
            this.lblLinkHint.AutoSize = true;
            this.lblLinkHint.Location = new System.Drawing.Point(6, 126);
            this.lblLinkHint.Name = "lblLinkHint";
            this.lblLinkHint.Size = new System.Drawing.Size(39, 17);
            this.lblLinkHint.TabIndex = 16;
            this.lblLinkHint.Text = "链接 :";
            this.lblLinkHint.Visible = false;
            // 
            // lblLink
            // 
            this.lblLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLink.AutoEllipsis = true;
            this.lblLink.ContextMenuStrip = this.cmsText;
            this.lblLink.Location = new System.Drawing.Point(41, 126);
            this.lblLink.Name = "lblLink";
            this.lblLink.Size = new System.Drawing.Size(291, 17);
            this.lblLink.TabIndex = 17;
            this.lblLink.Text = "未知链接";
            this.lblLink.Visible = false;
            // 
            // cmsAdjustOffset
            // 
            this.cmsAdjustOffset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFastHalfHalfSecond,
            this.miSlowHalfHalfSecond,
            this.miFastHalfSecond,
            this.miSlowHalfSecond,
            this.miFastSecond,
            this.miSlowSecond,
            this.sepAdjustOffset,
            this.miOffsetText,
            this.miResetOffset});
            this.cmsAdjustOffset.Name = "cmsAdjustOffset";
            this.cmsAdjustOffset.Size = new System.Drawing.Size(165, 186);
            // 
            // miOffsetText
            // 
            this.miOffsetText.Enabled = false;
            this.miOffsetText.Name = "miOffsetText";
            this.miOffsetText.Size = new System.Drawing.Size(164, 22);
            this.miOffsetText.Text = "当前时间差 : 0 秒";
            // 
            // miResetOffset
            // 
            this.miResetOffset.Name = "miResetOffset";
            this.miResetOffset.Size = new System.Drawing.Size(164, 22);
            this.miResetOffset.Text = "复原";
            this.miResetOffset.Click += new System.EventHandler(this.MiResetOffset_Click);
            // 
            // sepAdjustOffset
            // 
            this.sepAdjustOffset.Name = "sepAdjustOffset";
            this.sepAdjustOffset.Size = new System.Drawing.Size(161, 6);
            // 
            // miFastHalfHalfSecond
            // 
            this.miFastHalfHalfSecond.Name = "miFastHalfHalfSecond";
            this.miFastHalfHalfSecond.Size = new System.Drawing.Size(164, 22);
            this.miFastHalfHalfSecond.Tag = 0.25D;
            this.miFastHalfHalfSecond.Text = "快 0.25 秒";
            this.miFastHalfHalfSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // miSlowHalfHalfSecond
            // 
            this.miSlowHalfHalfSecond.Name = "miSlowHalfHalfSecond";
            this.miSlowHalfHalfSecond.Size = new System.Drawing.Size(164, 22);
            this.miSlowHalfHalfSecond.Tag = -0.25D;
            this.miSlowHalfHalfSecond.Text = "慢 0.25 秒";
            this.miSlowHalfHalfSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // miFastHalfSecond
            // 
            this.miFastHalfSecond.Name = "miFastHalfSecond";
            this.miFastHalfSecond.Size = new System.Drawing.Size(164, 22);
            this.miFastHalfSecond.Tag = 0.5D;
            this.miFastHalfSecond.Text = "快 0.5 秒";
            this.miFastHalfSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // miSlowHalfSecond
            // 
            this.miSlowHalfSecond.Name = "miSlowHalfSecond";
            this.miSlowHalfSecond.Size = new System.Drawing.Size(164, 22);
            this.miSlowHalfSecond.Tag = -0.5D;
            this.miSlowHalfSecond.Text = "慢 0.5 秒";
            this.miSlowHalfSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // miFastSecond
            // 
            this.miFastSecond.Name = "miFastSecond";
            this.miFastSecond.Size = new System.Drawing.Size(164, 22);
            this.miFastSecond.Tag = 1D;
            this.miFastSecond.Text = "快 1 秒";
            this.miFastSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // miSlowSecond
            // 
            this.miSlowSecond.Name = "miSlowSecond";
            this.miSlowSecond.Size = new System.Drawing.Size(164, 22);
            this.miSlowSecond.Tag = -1D;
            this.miSlowSecond.Text = "慢 1 秒";
            this.miSlowSecond.Click += new System.EventHandler(this.MiAdjustOffset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(362, 324);
            this.Controls.Add(this.gpbMusic);
            this.Controls.Add(this.gpbNetwork);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnVisitLink);
            this.Controls.Add(this.btnShowLyric);
            this.Controls.Add(this.btnListen);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(378, 0);
            this.Name = "MainForm";
            this.Text = "NcmlAtw Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.cmsText.ResumeLayout(false);
            this.gpbNetwork.ResumeLayout(false);
            this.gpbNetwork.PerformLayout();
            this.gpbMusic.ResumeLayout(false);
            this.gpbMusic.PerformLayout();
            this.cmsAdjustOffset.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Button btnQrcode;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Button btnAdjustOffset;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.Button btnShowLyric;
        private System.Windows.Forms.Button btnVisitLink;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblTitleHint;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAlbumHint;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.Label lblArtistHint;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.TextBox edtIP;
        private System.Windows.Forms.ComboBox cbbInterface;
        private System.Windows.Forms.GroupBox gpbNetwork;
        private System.Windows.Forms.GroupBox gpbMusic;
        private System.Windows.Forms.Label lblLinkHint;
        private System.Windows.Forms.Label lblLink;
        private System.Windows.Forms.ContextMenuStrip cmsAdjustOffset;
        private System.Windows.Forms.ContextMenuStrip cmsText;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miFastHalfSecond;
        private System.Windows.Forms.ToolStripMenuItem miSlowHalfSecond;
        private System.Windows.Forms.ToolStripMenuItem miFastSecond;
        private System.Windows.Forms.ToolStripMenuItem miSlowSecond;
        private System.Windows.Forms.ToolTip tipText;
        private System.Windows.Forms.ToolStripMenuItem miResetOffset;
        private System.Windows.Forms.ToolStripMenuItem miOffsetText;
        private System.Windows.Forms.ToolStripSeparator sepAdjustOffset;
        private System.Windows.Forms.ToolStripMenuItem miFastHalfHalfSecond;
        private System.Windows.Forms.ToolStripMenuItem miSlowHalfHalfSecond;
    }
}

