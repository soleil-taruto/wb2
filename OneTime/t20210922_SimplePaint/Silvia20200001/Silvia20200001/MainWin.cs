﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	public partial class MainWin : Form
	{
		#region WndProc

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x112;
			const long SC_CLOSE = 0xF060L;

			if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE)
			{
				this.BeginInvoke((MethodInvoker)delegate { this.保存せずに終了Click(null, null); });
				return;
			}
			base.WndProc(ref m);
		}

		#endregion

		public MainWin()
		{
			InitializeComponent();

			this.MinimumSize = this.Size;
		}

		private void MainWin_Load(object sender, EventArgs e)
		{
			// ログ出力_初期化
			{
				string logFile = Path.Combine(Environment.GetEnvironmentVariable("TMP"), ProcMain.APP_IDENT + ".log");

				SCommon.DeletePath(logFile);

				ProcMain.WriteLog = message =>
				{
					try
					{
						File.AppendAllLines(logFile, new string[] { "[" + DateTime.Now + "] " + message }, Encoding.UTF8);
					}
					catch
					{ }
				};
			}

			Ground.I = new Ground();
		}

		private void MainWin_Shown(object sender, EventArgs e)
		{
			this.ClearCanvas(800, 600);
			this.EM.StartTimer();
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.EM.EndTimer(); // 念のため
		}

		private void CloseWindow()
		{
			this.EM.EndTimer();
			this.Close();
		}

		private Common.P_EventManager EM = new Common.P_EventManager();

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			this.EM.EventHandlerForTimer(() =>
			{
				this.UpdateSubStatus();
			});
		}

		private void UpdateSubStatus()
		{
			this.SubStatus.Text = string.Join(", ", this.LastMousePos.X, this.LastMousePos.Y, this.PenDown, this.EraserDown);
		}

		private void MainWin_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void MainWin_DragDrop(object sender, DragEventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				this.ClearCanvas(Bitmap.FromFile(((string[])e.Data.GetData(DataFormats.FileDrop))[0]));
			});
		}

		private void ファイルを開くClick(object sender, EventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				string homeDir = Directory.GetCurrentDirectory();
				try
				{
					using (OpenFileDialog f = new OpenFileDialog())
					{
						f.Title = "ファイルを開く";

						f.FileName = "何時ぞやのキャンバス.bmp";
						f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

						f.Filter = "BMPファイル(*.bmp)|*.bmp|PNGファイル(*.png)|*.png|全てのファイル(*.*)|*.*";
						f.FilterIndex = 1;

						if (f.ShowDialog() == DialogResult.OK)
						{
							this.ClearCanvas(Bitmap.FromFile(f.FileName));
						}
					}
				}
				finally
				{
					Directory.SetCurrentDirectory(homeDir);
				}
			});
		}

		private void 保存して終了Click(object sender, EventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				string homeDir = Directory.GetCurrentDirectory();
				try
				{
					using (SaveFileDialog f = new SaveFileDialog())
					{
						f.Title = "現在のキャンバスを保存して終了する";

						f.FileName = "現在のキャンバス.bmp";
						f.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

						f.Filter = "BMPファイル(*.bmp)|*.bmp|PNGファイル(*.png)|*.png|全てのファイル(*.*)|*.*";
						f.FilterIndex = 1;

						if (f.ShowDialog() == DialogResult.OK)
						{
							string ext = Path.GetExtension(f.FileName);

							if (SCommon.EqualsIgnoreCase(ext, ".bmp"))
								this.MainPicture.Image.Save(f.FileName, ImageFormat.Bmp);
							else
								this.MainPicture.Image.Save(f.FileName);

							this.CloseWindow();
							return;
						}
					}
				}
				finally
				{
					Directory.SetCurrentDirectory(homeDir);
				}
			});
		}

		private void 保存せずに終了Click(object sender, EventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				if (MessageBox.Show(
					"保存せずに終了します。",
					"確認",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Warning
					)
					!= DialogResult.OK
					)
					return;

				this.CloseWindow();
				return;
			});
		}

		private void MainPanel_Paint(object sender, PaintEventArgs e)
		{
			// noop
		}

		private void MainPicture_Click(object sender, EventArgs e)
		{
			// noop
		}

		private void ClearCanvas()
		{
			this.ClearCanvas(this.MainPicture.Image.Width, this.MainPicture.Image.Height);
		}

		private void ClearCanvas(int w, int h)
		{
			Image image = new Bitmap(w, h);

			using (Graphics g = Graphics.FromImage(image))
			{
				g.FillRectangle(Brushes.White, 0, 0, w, h);
			}
			this.ClearCanvas(image);
		}

		private void ClearCanvas(Image image)
		{
			// 位置ズレ対策
			this.MainPicture.Width = 10;
			this.MainPicture.Height = 10;

			this.MainPicture.Image = image;
			this.MainPicture.Left = 0;
			this.MainPicture.Top = 0;
			this.MainPicture.Width = image.Width;
			this.MainPicture.Height = image.Height;
		}

		private void DrawCanvas(bool antiAliasing, Action<Graphics> routine)
		{
			using (Graphics g = Graphics.FromImage(this.MainPicture.Image))
			{
				if (antiAliasing)
				{
					g.TextRenderingHint = TextRenderingHint.AntiAlias;
					g.SmoothingMode = SmoothingMode.AntiAlias;
				}
				routine(g);
			}
			this.MainPicture.Invalidate();
		}

		private I2Point LastMousePos = new I2Point(0, 0);
		private bool PenDown = false;
		private bool EraserDown = false;

		private void MainPicture_MouseDown(object sender, MouseEventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				if (e.Button == MouseButtons.Left)
					this.PenDown = true;
				else if (e.Button == MouseButtons.Right)
					this.EraserDown = true;
			});
		}

		private void MainPicture_MouseUp(object sender, MouseEventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				if (e.Button == MouseButtons.Left)
					this.PenDown = false;
				else if (e.Button == MouseButtons.Right)
					this.EraserDown = false;
			});
		}

		private void MainPicture_MouseMove(object sender, MouseEventArgs e)
		{
			this.EM.EventHandler(false, () =>
			{
				I2Point p1 = this.LastMousePos;
				I2Point p2 = new I2Point(e.X, e.Y);

				this.DrawCanvas(false, g =>
				{
					if (this.PenDown)
						g.DrawLine(new Pen(Color.Black), p1.X, p1.Y, p2.X, p2.Y);
					else if (this.EraserDown)
						g.DrawLine(new Pen(Color.White, 60F), p1.X, p1.Y, p2.X, p2.Y);
				});

				this.LastMousePos = p2;
			});
		}
	}
}
