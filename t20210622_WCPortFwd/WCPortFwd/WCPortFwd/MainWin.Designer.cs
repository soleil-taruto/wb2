namespace WCPortFwd
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
			this.Btn追加 = new System.Windows.Forms.Button();
			this.Btn削除 = new System.Windows.Forms.Button();
			this.Btn変更 = new System.Windows.Forms.Button();
			this.Btn上 = new System.Windows.Forms.Button();
			this.Btn下 = new System.Windows.Forms.Button();
			this.MainTimer = new System.Windows.Forms.Timer(this.components);
			this.TaskTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.TaskTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.Btn開く = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.Btn終了 = new System.Windows.Forms.ToolStripMenuItem();
			this.Btn開始停止 = new System.Windows.Forms.Button();
			this.MainSheet = new System.Windows.Forms.DataGridView();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.SubStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.TaskTrayMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MainSheet)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// Btn追加
			// 
			this.Btn追加.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Btn追加.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn追加.Location = new System.Drawing.Point(328, 12);
			this.Btn追加.Name = "Btn追加";
			this.Btn追加.Size = new System.Drawing.Size(102, 43);
			this.Btn追加.TabIndex = 1;
			this.Btn追加.Text = "追加";
			this.Btn追加.UseVisualStyleBackColor = true;
			this.Btn追加.Click += new System.EventHandler(this.Btn追加_Click);
			// 
			// Btn削除
			// 
			this.Btn削除.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Btn削除.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn削除.Location = new System.Drawing.Point(436, 12);
			this.Btn削除.Name = "Btn削除";
			this.Btn削除.Size = new System.Drawing.Size(102, 43);
			this.Btn削除.TabIndex = 2;
			this.Btn削除.Text = "削除";
			this.Btn削除.UseVisualStyleBackColor = true;
			this.Btn削除.Click += new System.EventHandler(this.Btn削除_Click);
			// 
			// Btn変更
			// 
			this.Btn変更.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Btn変更.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn変更.Location = new System.Drawing.Point(544, 12);
			this.Btn変更.Name = "Btn変更";
			this.Btn変更.Size = new System.Drawing.Size(102, 43);
			this.Btn変更.TabIndex = 3;
			this.Btn変更.Text = "変更";
			this.Btn変更.UseVisualStyleBackColor = true;
			this.Btn変更.Click += new System.EventHandler(this.Btn変更_Click);
			// 
			// Btn上
			// 
			this.Btn上.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Btn上.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn上.Location = new System.Drawing.Point(652, 12);
			this.Btn上.Name = "Btn上";
			this.Btn上.Size = new System.Drawing.Size(46, 43);
			this.Btn上.TabIndex = 4;
			this.Btn上.Text = "▲";
			this.Btn上.UseVisualStyleBackColor = true;
			this.Btn上.Click += new System.EventHandler(this.Btn上_Click);
			// 
			// Btn下
			// 
			this.Btn下.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Btn下.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn下.Location = new System.Drawing.Point(704, 12);
			this.Btn下.Name = "Btn下";
			this.Btn下.Size = new System.Drawing.Size(46, 43);
			this.Btn下.TabIndex = 5;
			this.Btn下.Text = "▼";
			this.Btn下.UseVisualStyleBackColor = true;
			this.Btn下.Click += new System.EventHandler(this.Btn下_Click);
			// 
			// MainTimer
			// 
			this.MainTimer.Enabled = true;
			this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// TaskTrayIcon
			// 
			this.TaskTrayIcon.BalloonTipText = "CPortFwd";
			this.TaskTrayIcon.ContextMenuStrip = this.TaskTrayMenu;
			this.TaskTrayIcon.Text = "CPortFwd";
			this.TaskTrayIcon.Visible = true;
			this.TaskTrayIcon.DoubleClick += new System.EventHandler(this.TaskTrayIcon_DoubleClick);
			// 
			// TaskTrayMenu
			// 
			this.TaskTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Btn開く,
            this.toolStripMenuItem1,
            this.Btn終了});
			this.TaskTrayMenu.Name = "TaskTrayMenu";
			this.TaskTrayMenu.Size = new System.Drawing.Size(120, 54);
			// 
			// Btn開く
			// 
			this.Btn開く.Name = "Btn開く";
			this.Btn開く.Size = new System.Drawing.Size(119, 22);
			this.Btn開く.Text = "開く(&O)";
			this.Btn開く.Click += new System.EventHandler(this.Btn開く_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(116, 6);
			// 
			// Btn終了
			// 
			this.Btn終了.Name = "Btn終了";
			this.Btn終了.Size = new System.Drawing.Size(119, 22);
			this.Btn終了.Text = "終了(&X)";
			this.Btn終了.Click += new System.EventHandler(this.Btn終了_Click);
			// 
			// Btn開始停止
			// 
			this.Btn開始停止.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn開始停止.Location = new System.Drawing.Point(12, 12);
			this.Btn開始停止.Name = "Btn開始停止";
			this.Btn開始停止.Size = new System.Drawing.Size(150, 43);
			this.Btn開始停止.TabIndex = 0;
			this.Btn開始停止.Text = "開始 / 停止";
			this.Btn開始停止.UseVisualStyleBackColor = true;
			this.Btn開始停止.Click += new System.EventHandler(this.Btn開始停止_Click);
			// 
			// MainSheet
			// 
			this.MainSheet.AllowUserToAddRows = false;
			this.MainSheet.AllowUserToDeleteRows = false;
			this.MainSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MainSheet.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.MainSheet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.MainSheet.DefaultCellStyle = dataGridViewCellStyle1;
			this.MainSheet.Location = new System.Drawing.Point(12, 61);
			this.MainSheet.MultiSelect = false;
			this.MainSheet.Name = "MainSheet";
			this.MainSheet.ReadOnly = true;
			this.MainSheet.RowHeadersVisible = false;
			this.MainSheet.RowTemplate.Height = 21;
			this.MainSheet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.MainSheet.Size = new System.Drawing.Size(738, 300);
			this.MainSheet.TabIndex = 6;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.SubStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 373);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(762, 23);
			this.statusStrip1.TabIndex = 7;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			this.StatusLabel.Size = new System.Drawing.Size(719, 18);
			this.StatusLabel.Spring = true;
			this.StatusLabel.Text = "----";
			this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SubStatusLabel
			// 
			this.SubStatusLabel.Name = "SubStatusLabel";
			this.SubStatusLabel.Size = new System.Drawing.Size(28, 18);
			this.SubStatusLabel.Text = "----";
			// 
			// MainWin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(762, 396);
			this.Controls.Add(this.MainSheet);
			this.Controls.Add(this.Btn下);
			this.Controls.Add(this.Btn上);
			this.Controls.Add(this.Btn変更);
			this.Controls.Add(this.Btn削除);
			this.Controls.Add(this.Btn追加);
			this.Controls.Add(this.Btn開始停止);
			this.Controls.Add(this.statusStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWin";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "CPortFwd";
			this.Activated += new System.EventHandler(this.MainWin_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWin_FormClosed);
			this.Load += new System.EventHandler(this.MainWin_Load);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainWin_MouseClick);
			this.TaskTrayMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MainSheet)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Btn追加;
		private System.Windows.Forms.Button Btn削除;
		private System.Windows.Forms.Button Btn変更;
		private System.Windows.Forms.Button Btn上;
		private System.Windows.Forms.Button Btn下;
		private System.Windows.Forms.Timer MainTimer;
		private System.Windows.Forms.NotifyIcon TaskTrayIcon;
		private System.Windows.Forms.ContextMenuStrip TaskTrayMenu;
		private System.Windows.Forms.ToolStripMenuItem Btn開く;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem Btn終了;
		private System.Windows.Forms.Button Btn開始停止;
		private System.Windows.Forms.DataGridView MainSheet;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel SubStatusLabel;
	}
}

