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
				this.BeginInvoke((MethodInvoker)this.CloseWindow);
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
			// init WriteLog
			{
				string consoleLogFile = ProcMain.SelfFile + "-console.log";

				File.WriteAllBytes(consoleLogFile, SCommon.EMPTY_BYTES);

				ProcMain.WriteLog = message =>
				{
					try
					{
						File.AppendAllLines(consoleLogFile, new string[] { "[" + DateTime.Now + "] " + message }, Encoding.UTF8);
					}
					catch
					{ }
				};
			}

			Ground.I = new Ground();
		}

		private void MainWin_Shown(object sender, EventArgs e)
		{
			// none
		}

		private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
		{
			// none
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			// none
		}

		private void CloseWindow()
		{
			this.Close();
		}
	}
}
