using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x112;
			const long SC_CLOSE = 0xF060L;

			if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE)
			{
				this.BeginInvoke((MethodInvoker)delegate { this.保存して終了Click(null, null); });
				return;
			}
			base.WndProc(ref m);
		}

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
			this.MT_Busy = false;
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.MT_Busy = true;
		}

		private void CloseWindow()
		{
			this.MT_Busy = true;
			this.Close();
		}

		private bool MT_Busy = true;

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			if (this.MT_Busy)
				return;

			this.MT_Busy = true;
			try
			{
				this.UpdateSubStatus();
			}
			catch (Exception ex)
			{
				ProcMain.WriteLog(ex);
			}
			finally
			{
				this.MT_Busy = false;
			}
		}

		private void UpdateSubStatus()
		{
			// TODO
		}

		private void 保存して終了Click(object sender, EventArgs e)
		{
			// TODO
		}

		private void 保存せずに終了Click(object sender, EventArgs e)
		{
			// TODO
		}
	}
}
