namespace NeteaseM2DServer.Src.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.buttonListen = new System.Windows.Forms.Button();
            this.buttonShowLyric = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSongTitle = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.labelSongAlbum = new System.Windows.Forms.Label();
            this.labelSongArtist = new System.Windows.Forms.Label();
            this.labelSongDuration = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerGlobal = new System.Windows.Forms.Timer(this.components);
            this.buttonOpenWeb = new System.Windows.Forms.Button();
            this.buttonTimeAdjust = new System.Windows.Forms.Button();
            this.timeAdjustContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemFaster05 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSlower05 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFaster1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSlow1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.buttonQrCode = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.timeAdjustContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(263, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "请输入在本机监听的端口（用于安卓端的监听）:";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(281, 35);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(69, 23);
            this.numericUpDownPort.TabIndex = 5;
            this.numericUpDownPort.Value = new decimal(new int[] {
            1212,
            0,
            0,
            0});
            // 
            // buttonListen
            // 
            this.buttonListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonListen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonListen.Location = new System.Drawing.Point(12, 175);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(75, 25);
            this.buttonListen.TabIndex = 12;
            this.buttonListen.Text = "监听端口";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.buttonListen_Click);
            // 
            // buttonShowLyric
            // 
            this.buttonShowLyric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShowLyric.Enabled = false;
            this.buttonShowLyric.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonShowLyric.Location = new System.Drawing.Point(93, 175);
            this.buttonShowLyric.Name = "buttonShowLyric";
            this.buttonShowLyric.Size = new System.Drawing.Size(95, 25);
            this.buttonShowLyric.TabIndex = 13;
            this.buttonShowLyric.Text = "打开桌面歌词";
            this.buttonShowLyric.UseVisualStyleBackColor = true;
            this.buttonShowLyric.Click += new System.EventHandler(this.buttonShowLyric_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "当前歌曲：";
            // 
            // labelSongTitle
            // 
            this.labelSongTitle.AutoEllipsis = true;
            this.labelSongTitle.ContextMenuStrip = this.contextMenuStrip;
            this.labelSongTitle.Location = new System.Drawing.Point(12, 89);
            this.labelSongTitle.Name = "labelSongTitle";
            this.labelSongTitle.Size = new System.Drawing.Size(338, 17);
            this.labelSongTitle.TabIndex = 9;
            this.labelSongTitle.Text = "Title";
            this.toolTip.SetToolTip(this.labelSongTitle, "Title");
            this.labelSongTitle.Visible = false;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCopy});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(114, 26);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Name = "menuItemCopy";
            this.menuItemCopy.Size = new System.Drawing.Size(113, 22);
            this.menuItemCopy.Text = "复制(&C)";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // labelSongAlbum
            // 
            this.labelSongAlbum.AutoEllipsis = true;
            this.labelSongAlbum.ContextMenuStrip = this.contextMenuStrip;
            this.labelSongAlbum.Location = new System.Drawing.Point(12, 112);
            this.labelSongAlbum.Name = "labelSongAlbum";
            this.labelSongAlbum.Size = new System.Drawing.Size(338, 17);
            this.labelSongAlbum.TabIndex = 10;
            this.labelSongAlbum.Text = "Album";
            this.labelSongAlbum.Visible = false;
            // 
            // labelSongArtist
            // 
            this.labelSongArtist.AutoEllipsis = true;
            this.labelSongArtist.ContextMenuStrip = this.contextMenuStrip;
            this.labelSongArtist.Location = new System.Drawing.Point(12, 135);
            this.labelSongArtist.Name = "labelSongArtist";
            this.labelSongArtist.Size = new System.Drawing.Size(338, 17);
            this.labelSongArtist.TabIndex = 11;
            this.labelSongArtist.Text = "Artist";
            this.labelSongArtist.Visible = false;
            // 
            // labelSongDuration
            // 
            this.labelSongDuration.AutoSize = true;
            this.labelSongDuration.Location = new System.Drawing.Point(73, 64);
            this.labelSongDuration.Name = "labelSongDuration";
            this.labelSongDuration.Size = new System.Drawing.Size(83, 17);
            this.labelSongDuration.TabIndex = 7;
            this.labelSongDuration.Text = "00:00 / 00:00";
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonExit.Location = new System.Drawing.Point(275, 175);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 25);
            this.buttonExit.TabIndex = 15;
            this.buttonExit.Text = "退出";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // timerGlobal
            // 
            this.timerGlobal.Tick += new System.EventHandler(this.timerGlobal_Tick);
            // 
            // buttonOpenWeb
            // 
            this.buttonOpenWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOpenWeb.Enabled = false;
            this.buttonOpenWeb.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOpenWeb.Location = new System.Drawing.Point(194, 175);
            this.buttonOpenWeb.Name = "buttonOpenWeb";
            this.buttonOpenWeb.Size = new System.Drawing.Size(75, 25);
            this.buttonOpenWeb.TabIndex = 14;
            this.buttonOpenWeb.Text = "打开网页";
            this.buttonOpenWeb.UseVisualStyleBackColor = true;
            this.buttonOpenWeb.Click += new System.EventHandler(this.buttonOpenWeb_Click);
            // 
            // buttonTimeAdjust
            // 
            this.buttonTimeAdjust.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTimeAdjust.Location = new System.Drawing.Point(275, 64);
            this.buttonTimeAdjust.Name = "buttonTimeAdjust";
            this.buttonTimeAdjust.Size = new System.Drawing.Size(75, 25);
            this.buttonTimeAdjust.TabIndex = 8;
            this.buttonTimeAdjust.Text = "时间调整";
            this.buttonTimeAdjust.UseVisualStyleBackColor = true;
            this.buttonTimeAdjust.Click += new System.EventHandler(this.buttonTimeAdjust_Click);
            // 
            // timeAdjustContextMenu
            // 
            this.timeAdjustContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFaster05,
            this.menuItemSlower05,
            this.menuItemFaster1,
            this.menuItemSlow1});
            this.timeAdjustContextMenu.Name = "contextMenuStrip";
            this.timeAdjustContextMenu.Size = new System.Drawing.Size(137, 92);
            // 
            // menuItemFaster05
            // 
            this.menuItemFaster05.Name = "menuItemFaster05";
            this.menuItemFaster05.Size = new System.Drawing.Size(136, 22);
            this.menuItemFaster05.Tag = "+0.5";
            this.menuItemFaster05.Text = "快 0.5 秒 (&1)";
            this.menuItemFaster05.Click += new System.EventHandler(this.menuItemFasterSlower_Click);
            // 
            // menuItemSlower05
            // 
            this.menuItemSlower05.Name = "menuItemSlower05";
            this.menuItemSlower05.Size = new System.Drawing.Size(136, 22);
            this.menuItemSlower05.Tag = "-0.5";
            this.menuItemSlower05.Text = "慢 0.5 秒 (&2)";
            this.menuItemSlower05.Click += new System.EventHandler(this.menuItemFasterSlower_Click);
            // 
            // menuItemFaster1
            // 
            this.menuItemFaster1.Name = "menuItemFaster1";
            this.menuItemFaster1.Size = new System.Drawing.Size(136, 22);
            this.menuItemFaster1.Tag = "+1";
            this.menuItemFaster1.Text = "快 1 秒 (&3)";
            this.menuItemFaster1.Click += new System.EventHandler(this.menuItemFasterSlower_Click);
            // 
            // menuItemSlow1
            // 
            this.menuItemSlow1.Name = "menuItemSlow1";
            this.menuItemSlow1.Size = new System.Drawing.Size(136, 22);
            this.menuItemSlow1.Tag = "-1";
            this.menuItemSlow1.Text = "慢 1 秒 (&4)";
            this.menuItemSlow1.Click += new System.EventHandler(this.menuItemFasterSlower_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "当前局域网内的 IP:";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(128, 6);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.ReadOnly = true;
            this.textBoxIP.Size = new System.Drawing.Size(141, 23);
            this.textBoxIP.TabIndex = 2;
            // 
            // buttonQrCode
            // 
            this.buttonQrCode.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonQrCode.Location = new System.Drawing.Point(275, 5);
            this.buttonQrCode.Name = "buttonQrCode";
            this.buttonQrCode.Size = new System.Drawing.Size(75, 25);
            this.buttonQrCode.TabIndex = 3;
            this.buttonQrCode.Text = "显示二维码";
            this.buttonQrCode.UseVisualStyleBackColor = true;
            this.buttonQrCode.Visible = false;
            this.buttonQrCode.Click += new System.EventHandler(this.buttonQrCode_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonListen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonExit;
            this.ClientSize = new System.Drawing.Size(362, 209);
            this.Controls.Add(this.buttonQrCode);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonTimeAdjust);
            this.Controls.Add(this.buttonOpenWeb);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonShowLyric);
            this.Controls.Add(this.buttonListen);
            this.Controls.Add(this.labelSongDuration);
            this.Controls.Add(this.labelSongArtist);
            this.Controls.Add(this.labelSongAlbum);
            this.Controls.Add(this.labelSongTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownPort);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "NeteaseM2D Setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.timeAdjustContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.Button buttonShowLyric;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelSongTitle;
        private System.Windows.Forms.Label labelSongAlbum;
        private System.Windows.Forms.Label labelSongArtist;
        private System.Windows.Forms.Label labelSongDuration;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer timerGlobal;
        private System.Windows.Forms.Button buttonOpenWeb;
        private System.Windows.Forms.Button buttonTimeAdjust;
        private System.Windows.Forms.ContextMenuStrip timeAdjustContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemFaster05;
        private System.Windows.Forms.ToolStripMenuItem menuItemSlower05;
        private System.Windows.Forms.ToolStripMenuItem menuItemFaster1;
        private System.Windows.Forms.ToolStripMenuItem menuItemSlow1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Button buttonQrCode;
    }
}

