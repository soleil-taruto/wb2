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
		#region WndProc

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
			this.EM.TimerEventHandler(() =>
			{
				this.UpdateSubStatus();
			});
		}

		private void UpdateSubStatus()
		{
			// TODO
		}

		private void 保存して終了Click(object sender, EventArgs e)
		{
			this.EM.EventHandler(() =>
			{
				// TODO
			});
		}

		private void 保存せずに終了Click(object sender, EventArgs e)
		{
			this.EM.EventHandler(() =>
			{
				// TODO
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
	}
}
