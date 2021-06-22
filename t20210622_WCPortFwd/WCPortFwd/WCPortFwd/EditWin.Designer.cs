namespace WCPortFwd
{
	partial class EditWin
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
			this.label1 = new System.Windows.Forms.Label();
			this.RecvPortNo = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ForwardPortNo = new System.Windows.Forms.TextBox();
			this.ForwardDomain = new System.Windows.Forms.TextBox();
			this.RawKey = new System.Windows.Forms.TextBox();
			this.RB暗号化 = new System.Windows.Forms.RadioButton();
			this.RB復号 = new System.Windows.Forms.RadioButton();
			this.RB暗号化ナシ = new System.Windows.Forms.RadioButton();
			this.Btn自動生成 = new System.Windows.Forms.Button();
			this.RawKeyLabel = new System.Windows.Forms.Label();
			this.BtnReset = new System.Windows.Forms.Button();
			this.ConnectMax = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.RawKeyInfo = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.Btnファイルに保存 = new System.Windows.Forms.Button();
			this.Btnファイルから読み込み = new System.Windows.Forms.Button();
			this.Btnクリア = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.Location = new System.Drawing.Point(30, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(122, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "待ち受けポート";
			// 
			// RecvPortNo
			// 
			this.RecvPortNo.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RecvPortNo.Location = new System.Drawing.Point(169, 29);
			this.RecvPortNo.MaxLength = 5;
			this.RecvPortNo.Name = "RecvPortNo";
			this.RecvPortNo.Size = new System.Drawing.Size(120, 36);
			this.RecvPortNo.TabIndex = 1;
			this.RecvPortNo.Text = "65535";
			this.RecvPortNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.RecvPortNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RecvPortNo_KeyPress);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.Location = new System.Drawing.Point(30, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "転送先ポート";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label3.Location = new System.Drawing.Point(30, 119);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(122, 24);
			this.label3.TabIndex = 4;
			this.label3.Text = "転送先ホスト名";
			// 
			// ForwardPortNo
			// 
			this.ForwardPortNo.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.ForwardPortNo.Location = new System.Drawing.Point(169, 71);
			this.ForwardPortNo.MaxLength = 5;
			this.ForwardPortNo.Name = "ForwardPortNo";
			this.ForwardPortNo.Size = new System.Drawing.Size(120, 36);
			this.ForwardPortNo.TabIndex = 3;
			this.ForwardPortNo.Text = "65535";
			this.ForwardPortNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ForwardPortNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ForwardPortNo_KeyPress);
			// 
			// ForwardDomain
			// 
			this.ForwardDomain.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.ForwardDomain.Location = new System.Drawing.Point(169, 113);
			this.ForwardDomain.MaxLength = 1000;
			this.ForwardDomain.Name = "ForwardDomain";
			this.ForwardDomain.Size = new System.Drawing.Size(368, 36);
			this.ForwardDomain.TabIndex = 5;
			this.ForwardDomain.Text = "localhost";
			this.ForwardDomain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ForwardDomain_KeyPress);
			// 
			// RawKey
			// 
			this.RawKey.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RawKey.Location = new System.Drawing.Point(34, 324);
			this.RawKey.MaxLength = 1000;
			this.RawKey.Multiline = true;
			this.RawKey.Name = "RawKey";
			this.RawKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.RawKey.Size = new System.Drawing.Size(503, 86);
			this.RawKey.TabIndex = 16;
			this.RawKey.Text = "default";
			this.RawKey.TextChanged += new System.EventHandler(this.RawKey_TextChanged);
			this.RawKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RawKey_KeyPress);
			// 
			// RB暗号化
			// 
			this.RB暗号化.AutoSize = true;
			this.RB暗号化.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RB暗号化.Location = new System.Drawing.Point(34, 225);
			this.RB暗号化.Name = "RB暗号化";
			this.RB暗号化.Size = new System.Drawing.Size(124, 28);
			this.RB暗号化.TabIndex = 8;
			this.RB暗号化.TabStop = true;
			this.RB暗号化.Text = "暗号化モード";
			this.RB暗号化.UseVisualStyleBackColor = true;
			this.RB暗号化.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RB暗号化_KeyPress);
			// 
			// RB復号
			// 
			this.RB復号.AutoSize = true;
			this.RB復号.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RB復号.Location = new System.Drawing.Point(164, 225);
			this.RB復号.Name = "RB復号";
			this.RB復号.Size = new System.Drawing.Size(108, 28);
			this.RB復号.TabIndex = 9;
			this.RB復号.TabStop = true;
			this.RB復号.Text = "復号モード";
			this.RB復号.UseVisualStyleBackColor = true;
			this.RB復号.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RB復号_KeyPress);
			// 
			// RB暗号化ナシ
			// 
			this.RB暗号化ナシ.AutoSize = true;
			this.RB暗号化ナシ.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RB暗号化ナシ.Location = new System.Drawing.Point(278, 225);
			this.RB暗号化ナシ.Name = "RB暗号化ナシ";
			this.RB暗号化ナシ.Size = new System.Drawing.Size(108, 28);
			this.RB暗号化ナシ.TabIndex = 10;
			this.RB暗号化ナシ.TabStop = true;
			this.RB暗号化ナシ.Text = "暗号化なし";
			this.RB暗号化ナシ.UseVisualStyleBackColor = true;
			this.RB暗号化ナシ.CheckedChanged += new System.EventHandler(this.RB暗号化ナシ_CheckedChanged);
			this.RB暗号化ナシ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RB暗号化ナシ_KeyPress);
			// 
			// Btn自動生成
			// 
			this.Btn自動生成.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btn自動生成.Location = new System.Drawing.Point(328, 279);
			this.Btn自動生成.Name = "Btn自動生成";
			this.Btn自動生成.Size = new System.Drawing.Size(97, 39);
			this.Btn自動生成.TabIndex = 12;
			this.Btn自動生成.Text = "鍵生成";
			this.toolTip1.SetToolTip(this.Btn自動生成, "新しい鍵を自動生成します。");
			this.Btn自動生成.UseVisualStyleBackColor = true;
			this.Btn自動生成.Click += new System.EventHandler(this.Btn自動生成_Click);
			// 
			// RawKeyLabel
			// 
			this.RawKeyLabel.AutoSize = true;
			this.RawKeyLabel.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RawKeyLabel.Location = new System.Drawing.Point(30, 297);
			this.RawKeyLabel.Name = "RawKeyLabel";
			this.RawKeyLabel.Size = new System.Drawing.Size(170, 24);
			this.RawKeyLabel.TabIndex = 11;
			this.RawKeyLabel.Text = "鍵またはパスフレーズ";
			// 
			// BtnReset
			// 
			this.BtnReset.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.BtnReset.Location = new System.Drawing.Point(440, 416);
			this.BtnReset.Name = "BtnReset";
			this.BtnReset.Size = new System.Drawing.Size(97, 39);
			this.BtnReset.TabIndex = 18;
			this.BtnReset.Text = "リセット";
			this.toolTip1.SetToolTip(this.BtnReset, "このダイアログを開いた時の状態に戻します。");
			this.BtnReset.UseVisualStyleBackColor = true;
			this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
			// 
			// ConnectMax
			// 
			this.ConnectMax.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.ConnectMax.Location = new System.Drawing.Point(169, 155);
			this.ConnectMax.MaxLength = 3;
			this.ConnectMax.Name = "ConnectMax";
			this.ConnectMax.Size = new System.Drawing.Size(120, 36);
			this.ConnectMax.TabIndex = 7;
			this.ConnectMax.Text = "500";
			this.ConnectMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ConnectMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConnectMax_KeyPress);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label5.Location = new System.Drawing.Point(30, 161);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(122, 24);
			this.label5.TabIndex = 6;
			this.label5.Text = "最大同時接続数";
			// 
			// RawKeyInfo
			// 
			this.RawKeyInfo.AutoSize = true;
			this.RawKeyInfo.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.RawKeyInfo.Location = new System.Drawing.Point(40, 416);
			this.RawKeyInfo.Name = "RawKeyInfo";
			this.RawKeyInfo.Size = new System.Drawing.Size(69, 20);
			this.RawKeyInfo.TabIndex = 17;
			this.RawKeyInfo.Text = "comment";
			// 
			// Btnファイルに保存
			// 
			this.Btnファイルに保存.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btnファイルに保存.Location = new System.Drawing.Point(440, 234);
			this.Btnファイルに保存.Name = "Btnファイルに保存";
			this.Btnファイルに保存.Size = new System.Drawing.Size(97, 39);
			this.Btnファイルに保存.TabIndex = 14;
			this.Btnファイルに保存.Text = "Export";
			this.toolTip1.SetToolTip(this.Btnファイルに保存, "鍵またはパスフレーズをファイルから読み込みます。");
			this.Btnファイルに保存.UseVisualStyleBackColor = true;
			this.Btnファイルに保存.Click += new System.EventHandler(this.Btnファイルに保存_Click);
			// 
			// Btnファイルから読み込み
			// 
			this.Btnファイルから読み込み.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btnファイルから読み込み.Location = new System.Drawing.Point(440, 189);
			this.Btnファイルから読み込み.Name = "Btnファイルから読み込み";
			this.Btnファイルから読み込み.Size = new System.Drawing.Size(97, 39);
			this.Btnファイルから読み込み.TabIndex = 13;
			this.Btnファイルから読み込み.Text = "Import";
			this.toolTip1.SetToolTip(this.Btnファイルから読み込み, "鍵またはパスフレーズをファイルに書き出します。");
			this.Btnファイルから読み込み.UseVisualStyleBackColor = true;
			this.Btnファイルから読み込み.Click += new System.EventHandler(this.Btnファイルから読み込み_Click);
			// 
			// Btnクリア
			// 
			this.Btnクリア.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Btnクリア.Location = new System.Drawing.Point(440, 279);
			this.Btnクリア.Name = "Btnクリア";
			this.Btnクリア.Size = new System.Drawing.Size(97, 39);
			this.Btnクリア.TabIndex = 15;
			this.Btnクリア.Text = "クリア";
			this.toolTip1.SetToolTip(this.Btnクリア, "鍵またはパスフレーズをクリアします。");
			this.Btnクリア.UseVisualStyleBackColor = true;
			this.Btnクリア.Click += new System.EventHandler(this.Btnクリア_Click);
			// 
			// EditWin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(568, 484);
			this.Controls.Add(this.Btnクリア);
			this.Controls.Add(this.Btnファイルから読み込み);
			this.Controls.Add(this.Btnファイルに保存);
			this.Controls.Add(this.RawKeyInfo);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.ConnectMax);
			this.Controls.Add(this.BtnReset);
			this.Controls.Add(this.RawKeyLabel);
			this.Controls.Add(this.Btn自動生成);
			this.Controls.Add(this.RB暗号化ナシ);
			this.Controls.Add(this.RB復号);
			this.Controls.Add(this.RB暗号化);
			this.Controls.Add(this.RawKey);
			this.Controls.Add(this.ForwardDomain);
			this.Controls.Add(this.ForwardPortNo);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.RecvPortNo);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditWin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "転送設定";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditWin_FormClosed);
			this.Load += new System.EventHandler(this.EditWin_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox RecvPortNo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox ForwardPortNo;
		private System.Windows.Forms.TextBox ForwardDomain;
		private System.Windows.Forms.TextBox RawKey;
		private System.Windows.Forms.RadioButton RB暗号化;
		private System.Windows.Forms.RadioButton RB復号;
		private System.Windows.Forms.RadioButton RB暗号化ナシ;
		private System.Windows.Forms.Button Btn自動生成;
		private System.Windows.Forms.Label RawKeyLabel;
		private System.Windows.Forms.Button BtnReset;
		private System.Windows.Forms.TextBox ConnectMax;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label RawKeyInfo;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button Btnファイルに保存;
		private System.Windows.Forms.Button Btnファイルから読み込み;
		private System.Windows.Forms.Button Btnクリア;
	}
}
