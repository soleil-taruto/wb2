using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace WCPortFwd
{
	public partial class 何かを待つWin : Form
	{
		// ---- [X] ALT+F4 抑止 ----

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x112;
			const long SC_CLOSE = 0xF060L;

			if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE)
			{
				return;
			}
			base.WndProc(ref m);
		}

		// ----

		ForwardInfo FI;

		public 何かを待つWin(ForwardInfo fi)
		{
			this.FI = fi;

			InitializeComponent();

			this.TimerOn = true;
		}

		private void 何かを待つWin_Load(object sender, EventArgs e)
		{
			this.Icon = Ground.I.MainWin.Icon;
		}

		private bool TimerOn;
		private int Counter;

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			if (this.TimerOn == false)
				return;

			bool 要終了 = false;
			this.Counter++;

			if (this.FI != null && this.Counter % 20 == 0)
			{
				this.FI.停止してね();
				this.FI.停止();

				if (this.FI.Proc == null)
				{
					要終了 = true;
				}
			}
			if (this.Counter == 600)
			{
				要終了 = true;
			}
			if (要終了)
			{
				this.TimerOn = false;
				this.Close();
			}
		}

		private void 何かを待つWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.TimerOn = false;
		}
	}
}
