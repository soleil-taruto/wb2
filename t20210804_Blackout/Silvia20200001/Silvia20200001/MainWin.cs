using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Permissions;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	public partial class MainWin : Form
	{
		#region ALT_F4 抑止

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x112;
			const long SC_CLOSE = 0xF060L;

			if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE)
				return;

			base.WndProc(ref m);
		}

		#endregion

		public MainWin()
		{
			InitializeComponent();
		}

		private void MainWin_Load(object sender, EventArgs e)
		{
			// none
		}

		private void MainWin_Shown(object sender, EventArgs e)
		{
			Rectangle rect = Screen.AllScreens[0].Bounds;

			this.Left = rect.Left;
			this.Top = rect.Top;
			this.Width = rect.Width;
			this.Height = rect.Height;

			this.BackColor = Color.Black;

			this.MainTimerEnabled = true;
		}

		private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.MainTimerEnabled = false;
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.MainTimerEnabled = false;
		}

		private void CloseWindow()
		{
			this.MainTimerEnabled = false;
			this.Close();
		}

		private bool MainTimerEnabled = false;
		private long MainTimerCount = 0L;

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			if (!this.MainTimerEnabled)
				return;

			this.MainTimerCount++;

			switch (this.MainTimerCount)
			{
				case 1: Cursor.Hide(); break; // カーソルが最初から入っていても _MouseEnter は走るみたいだけど、念のため。
				case 2: this.TopMost = true; break;
				case 3: this.TopMost = false; break;
				case 4: this.TopMost = true; break;
				case 5: this.TopMost = false; break;
				case 6: this.TopMost = true; break;

				default:
					break;
			}
		}

		private void MainWin_Click(object sender, EventArgs e)
		{
			this.CloseWindow();
		}

		private void MainWin_MouseEnter(object sender, EventArgs e)
		{
			Cursor.Hide();
		}
	}
}
