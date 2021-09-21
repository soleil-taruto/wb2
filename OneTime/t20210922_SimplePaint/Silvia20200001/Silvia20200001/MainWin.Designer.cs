namespace Charlotte
{
	partial class MainWin
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
			this.MainTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.SubStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.アプリToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ファイルを開くToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.保存して終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.保存せずに終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MainPanel = new System.Windows.Forms.Panel();
			this.MainPicture = new System.Windows.Forms.PictureBox();
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.MainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MainPicture)).BeginInit();
			this.SuspendLayout();
			// 
			// MainTimer
			// 
			this.MainTimer.Enabled = true;
			this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status,
            this.SubStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 339);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(584, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// Status
			// 
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(510, 17);
			this.Status.Spring = true;
			this.Status.Text = "Status";
			this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SubStatus
			// 
			this.SubStatus.Name = "SubStatus";
			this.SubStatus.Size = new System.Drawing.Size(59, 17);
			this.SubStatus.Text = "SubStatus";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.アプリToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// アプリToolStripMenuItem
			// 
			this.アプリToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルを開くToolStripMenuItem,
            this.toolStripMenuItem1,
            this.保存して終了ToolStripMenuItem,
            this.保存せずに終了ToolStripMenuItem});
			this.アプリToolStripMenuItem.Name = "アプリToolStripMenuItem";
			this.アプリToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
			this.アプリToolStripMenuItem.Text = "アプリ";
			// 
			// ファイルを開くToolStripMenuItem
			// 
			this.ファイルを開くToolStripMenuItem.Name = "ファイルを開くToolStripMenuItem";
			this.ファイルを開くToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.ファイルを開くToolStripMenuItem.Text = "ファイルを開く";
			this.ファイルを開くToolStripMenuItem.Click += new System.EventHandler(this.ファイルを開くClick);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(148, 6);
			// 
			// 保存して終了ToolStripMenuItem
			// 
			this.保存して終了ToolStripMenuItem.Name = "保存して終了ToolStripMenuItem";
			this.保存して終了ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.保存して終了ToolStripMenuItem.Text = "保存して終了";
			this.保存して終了ToolStripMenuItem.Click += new System.EventHandler(this.保存して終了Click);
			// 
			// 保存せずに終了ToolStripMenuItem
			// 
			this.保存せずに終了ToolStripMenuItem.Name = "保存せずに終了ToolStripMenuItem";
			this.保存せずに終了ToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.保存せずに終了ToolStripMenuItem.Text = "保存せずに終了";
			this.保存せずに終了ToolStripMenuItem.Click += new System.EventHandler(this.保存せずに終了Click);
			// 
			// MainPanel
			// 
			this.MainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainPanel.AutoScroll = true;
			this.MainPanel.Controls.Add(this.MainPicture);
			this.MainPanel.Location = new System.Drawing.Point(12, 27);
			this.MainPanel.Name = "MainPanel";
			this.MainPanel.Size = new System.Drawing.Size(560, 309);
			this.MainPanel.TabIndex = 2;
			this.MainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MainPanel_Paint);
			// 
			// MainPicture
			// 
			this.MainPicture.Location = new System.Drawing.Point(3, 3);
			this.MainPicture.Name = "MainPicture";
			this.MainPicture.Size = new System.Drawing.Size(100, 50);
			this.MainPicture.TabIndex = 0;
			this.MainPicture.TabStop = false;
			this.MainPicture.Click += new System.EventHandler(this.MainPicture_Click);
			this.MainPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainPicture_MouseDown);
			this.MainPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainPicture_MouseMove);
			this.MainPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainPicture_MouseUp);
			// 
			// MainWin
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 361);
			this.Controls.Add(this.MainPanel);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "MainWin";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Silvia20200001";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWin_FormClosed);
			this.Load += new System.EventHandler(this.MainWin_Load);
			this.Shown += new System.EventHandler(this.MainWin_Shown);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWin_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWin_DragEnter);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.MainPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MainPicture)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer MainTimer;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel Status;
		private System.Windows.Forms.ToolStripStatusLabel SubStatus;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem アプリToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 保存して終了ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 保存せずに終了ToolStripMenuItem;
		private System.Windows.Forms.Panel MainPanel;
		private System.Windows.Forms.PictureBox MainPicture;
		private System.Windows.Forms.ToolStripMenuItem ファイルを開くToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
	}
}

