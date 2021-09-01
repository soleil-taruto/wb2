namespace WCPortFwd
{
	partial class 停止ProcWin
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
			this.プロぐれ棒 = new System.Windows.Forms.ProgressBar();
			this.MainTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// プロぐれ棒
			// 
			this.プロぐれ棒.Location = new System.Drawing.Point(12, 22);
			this.プロぐれ棒.Name = "プロぐれ棒";
			this.プロぐれ棒.Size = new System.Drawing.Size(394, 31);
			this.プロぐれ棒.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.プロぐれ棒.TabIndex = 0;
			// 
			// MainTimer
			// 
			this.MainTimer.Enabled = true;
			this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// 停止ProcWin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(418, 79);
			this.ControlBox = false;
			this.Controls.Add(this.プロぐれ棒);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "停止ProcWin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "終了しています...";
			this.TopMost = true;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.停止ProcWin_FormClosed);
			this.Load += new System.EventHandler(this.停止ProcWin_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ProgressBar プロぐれ棒;
		private System.Windows.Forms.Timer MainTimer;
	}
}
