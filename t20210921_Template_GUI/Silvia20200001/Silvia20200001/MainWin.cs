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
			Ground.I = new Ground();
		}

		private void MainWin_Shown(object sender, EventArgs e)
		{
			this.MainTimer.Enabled = true;
			this.MT_Busy = false;
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.MainTimer.Enabled = false;
			this.MT_Busy = true;
		}

		private void CloseWindow()
		{
			this.MainTimer.Enabled = false;
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
				// none
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
	}
}
