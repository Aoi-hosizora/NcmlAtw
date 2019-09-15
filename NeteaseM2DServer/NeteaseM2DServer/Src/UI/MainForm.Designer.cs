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
            this.labelSongAlbum = new System.Windows.Forms.Label();
            this.labelSongArtist = new System.Windows.Forms.Label();
            this.labelSongDuration = new System.Windows.Forms.Label();
            this.buttonExit = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timerSong = new System.Windows.Forms.Timer(this.components);
            this.buttonOpenWeb = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(263, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "请输入在本机监听的端口（用于安卓端的监听）:";
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(281, 7);
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
            this.numericUpDownPort.TabIndex = 1;
            this.numericUpDownPort.Value = new decimal(new int[] {
            1212,
            0,
            0,
            0});
            // 
            // buttonListen
            // 
            this.buttonListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonListen.Location = new System.Drawing.Point(12, 134);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(75, 25);
            this.buttonListen.TabIndex = 7;
            this.buttonListen.Text = "监听端口";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.buttonListen_Click);
            // 
            // buttonShowLyric
            // 
            this.buttonShowLyric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShowLyric.Location = new System.Drawing.Point(93, 134);
            this.buttonShowLyric.Name = "buttonShowLyric";
            this.buttonShowLyric.Size = new System.Drawing.Size(95, 25);
            this.buttonShowLyric.TabIndex = 8;
            this.buttonShowLyric.Text = "打开桌面歌词";
            this.buttonShowLyric.UseVisualStyleBackColor = true;
            this.buttonShowLyric.Click += new System.EventHandler(this.buttonShowLyric_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "当前歌曲：";
            // 
            // labelSongTitle
            // 
            this.labelSongTitle.AutoEllipsis = true;
            this.labelSongTitle.Location = new System.Drawing.Point(12, 63);
            this.labelSongTitle.Name = "labelSongTitle";
            this.labelSongTitle.Size = new System.Drawing.Size(347, 17);
            this.labelSongTitle.TabIndex = 4;
            this.labelSongTitle.Text = "Title";
            this.toolTip.SetToolTip(this.labelSongTitle, "Title");
            this.labelSongTitle.Visible = false;
            // 
            // labelSongAlbum
            // 
            this.labelSongAlbum.AutoEllipsis = true;
            this.labelSongAlbum.Location = new System.Drawing.Point(12, 85);
            this.labelSongAlbum.Name = "labelSongAlbum";
            this.labelSongAlbum.Size = new System.Drawing.Size(344, 17);
            this.labelSongAlbum.TabIndex = 5;
            this.labelSongAlbum.Text = "Album";
            this.labelSongAlbum.Visible = false;
            // 
            // labelSongArtist
            // 
            this.labelSongArtist.AutoEllipsis = true;
            this.labelSongArtist.Location = new System.Drawing.Point(12, 107);
            this.labelSongArtist.Name = "labelSongArtist";
            this.labelSongArtist.Size = new System.Drawing.Size(344, 17);
            this.labelSongArtist.TabIndex = 6;
            this.labelSongArtist.Text = "Artist";
            this.labelSongArtist.Visible = false;
            // 
            // labelSongDuration
            // 
            this.labelSongDuration.AutoSize = true;
            this.labelSongDuration.Location = new System.Drawing.Point(73, 36);
            this.labelSongDuration.Name = "labelSongDuration";
            this.labelSongDuration.Size = new System.Drawing.Size(83, 17);
            this.labelSongDuration.TabIndex = 3;
            this.labelSongDuration.Text = "00:00 / 00:00";
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonExit.Location = new System.Drawing.Point(275, 134);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 25);
            this.buttonExit.TabIndex = 10;
            this.buttonExit.Text = "退出";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // timerSong
            // 
            this.timerSong.Tick += new System.EventHandler(this.timerSong_Tick);
            // 
            // buttonOpenWeb
            // 
            this.buttonOpenWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOpenWeb.Enabled = false;
            this.buttonOpenWeb.Location = new System.Drawing.Point(194, 134);
            this.buttonOpenWeb.Name = "buttonOpenWeb";
            this.buttonOpenWeb.Size = new System.Drawing.Size(75, 25);
            this.buttonOpenWeb.TabIndex = 9;
            this.buttonOpenWeb.Text = "打开网页";
            this.buttonOpenWeb.UseVisualStyleBackColor = true;
            this.buttonOpenWeb.Click += new System.EventHandler(this.buttonOpenWeb_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonListen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonExit;
            this.ClientSize = new System.Drawing.Size(362, 171);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
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
        private System.Windows.Forms.Timer timerSong;
        private System.Windows.Forms.Button buttonOpenWeb;
    }
}

