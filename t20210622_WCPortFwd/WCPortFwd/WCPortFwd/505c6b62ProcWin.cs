using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Threading;

namespace WCPortFwd
{
	public partial class 停止ProcWin : Form
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

		public 停止ProcWin()
		{
			InitializeComponent();
		}

		private void 停止ProcWin_Load(object sender, EventArgs e)
		{
			this.Icon = Ground.I.MainWin.Icon;

			foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
				if (fi.Proc != null)
					fi.停止してね();

			this.TimerOn = true;
			this.何もするな_Time = 5; // 必要か？
		}

		private bool TimerOn;
		private int 何もするな_Time;

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			if (this.TimerOn == false)
				return;

			if (0 < this.何もするな_Time)
			{
				this.何もするな_Time--;
				return;
			}
			foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
			{
				fi.停止();

				if (fi.Proc != null)
				{
					fi.停止してね();
					return;
				}
			}
			this.TimerOn = false;
			this.Close();
		}

		private void 停止ProcWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.TimerOn = false;
		}

		public static void Perform(bool noDlg = false)
		{
			if (noDlg)
			{
				foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
					if (fi.Proc != null)
						fi.停止してね();

				foreach (ForwardInfo fi in Ground.I.ForwardInfoList)
				{
					for (; ; )
					{
						fi.停止();

						if (fi.Proc == null)
							break;

						fi.停止してね();
						Thread.Sleep(100);
					}
				}
			}
			else
			{
				using (Form f = new 停止ProcWin())
				{
					f.ShowDialog();
				}
			}
		}
	}
}
