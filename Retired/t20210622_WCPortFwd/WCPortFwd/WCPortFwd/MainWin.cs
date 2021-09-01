using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WCPortFwd
{
	public partial class MainWin : Form
	{
		private int FirstPosL;
		private int FirstPosT;

		public MainWin()
		{
			InitializeComponent();

			this.MinimumSize = this.Size;
			this.Left = -this.Size.Width - 100;
			this.Top = -this.Size.Height - 100;

			{
				Rectangle prmScr = Screen.AllScreens[0].Bounds;

				int pScr_l = prmScr.Left;
				int pScr_t = prmScr.Top;
				int pScr_w = prmScr.Width;
				int pScr_h = prmScr.Height;

				this.FirstPosL = pScr_l + (pScr_w - this.Size.Width) / 2;
				this.FirstPosT = pScr_t + (pScr_h - this.Size.Height) / 2;
			}

			this.StatusLabel.Text = Ground.I.STATUSLABEL_BLANK;
			this.SubStatusLabel.Text = Ground.I.STATUSLABEL_BLANK;

			Ground.I.MainWin = this;
			Ground.I.LoadForwardInfoList();
		}

		private void MainWin_Load(object sender, EventArgs e)
		{
			this.TaskTrayIcon.Icon = this.Icon;
			this.TaskTrayIcon.Visible = false;
		}

		private bool ActivatedFlag;
		private int MinimizeCount;

		private void MainWin_Activated(object sender, EventArgs e)
		{
			if (this.ActivatedFlag)
				return;

			this.ActivatedFlag = true;
			//this.MinimumSize = this.Size; // moved -> ctor
			Tools.DoubleBufferOn(this.MainSheet);
			this.UpdateUi();

			//this.WindowState = FormWindowState.Minimized;
			this.MinimizeCount = 3;
			this.FISleepCount = 2;

			this.TimerOff.Decrement();
		}

		public Counter TimerOff = new Counter(1);
		public ulong TimerCount;
		private int WSSleepCount;
		private int FISleepCount;

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			if (this.TimerOff.IsZero() == false)
				return;

			if (1 <= Ground.I.シャットダウン_Count)
			{
				Ground.I.シャットダウン_Count--;
				return;
			}
			using (this.TimerOff.LocalIncrement())
			{
				if (this.TimerCount % 2 == 0 && 0 < this.MinimizeCount)
				{
					this.WindowState = FormWindowState.Minimized;
					this.MinimizeCount--;
				}

				if (0 < this.WSSleepCount)
				{
					this.WSSleepCount--;
				}
				else if (this.TimerCount % 20 == 0)
				{
					if (this.WindowState == FormWindowState.Minimized)
					{
						this.Visible = false;
						this.TaskTrayIcon.Visible = true;
					}
					else
					{
						this.Visible = true;
						this.TaskTrayIcon.Visible = false;
					}
				}

				if (0 < this.FISleepCount)
				{
					this.FISleepCount--;
				}
				else
				{
					int cycle = Math.Max(10, Ground.I.ForwardInfoList.Count);
					int index = (int)(this.TimerCount % (ulong)cycle);

					if (index < Ground.I.ForwardInfoList.Count)
					{
						ForwardInfo fi = Ground.I.ForwardInfoList[index];

						fi.巡回Proc();

						if (fi.Modified)
						{
							fi.Modified = false;
#if true
							try
							{
								this.MainSheet.Rows[index].Cells[(int)MSColumn_e.MSC_STARTED].Value = "停止(エラー)";
							}
							catch
							{ }
#else // 行数が多いと重すぎる。
							this.UpdateUi();
#endif
						}
					}
				}

				if (this.TimerCount % 600 == 0)
					GC.Collect();

				this.TimerCount++;
			}
		}

		protected override void WndProc(ref Message m)
		{
			const int WM_QUERYENDSESSION = 0x11;
			const int WM_ENDSESSION = 0x16;

			switch (m.Msg)
			{
				case WM_QUERYENDSESSION:
				case WM_ENDSESSION:
					Ground.I.シャットダウン_Count = 200; // 20[sec]
					break;
			}
			base.WndProc(ref m);
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.TimerOff.Increment();
			this.TaskTrayIcon.Visible = false;

			Ground.I.SaveForwardInfoList();
		}

		private bool TTIOA_Passed = false;

		private void TTI開くAction(bool アイコンをすぐ消す)
		{
			this.Visible = true;

			if (アイコンをすぐ消す)
				this.TaskTrayIcon.Visible = false;
			else
				this.WSSleepCount = 5;

			if (!this.TTIOA_Passed)
			{
				this.Left = this.FirstPosL;
				this.Top = this.FirstPosT;
			}
			this.WindowState = FormWindowState.Normal;
			this.TTIOA_Passed = true;
		}

		private void Btn開く_Click(object sender, EventArgs e)
		{
			this.TTI開くAction(true);
		}
		private void TaskTrayIcon_DoubleClick(object sender, EventArgs e)
		{
			this.TTI開くAction(false); // すぐに消すと隣のアイコンをクリックしてしまう。
		}

		private void Btn終了_Click(object sender, EventArgs e)
		{
			this.TimerOff.Increment();
			this.TaskTrayIcon.Visible = false;

			停止ProcWin.Perform(); // Program.cs にもあるので、ここを通らなくても良い。

			this.Close();
		}

		private enum MSColumn_e
		{
			MSC_STARTED,
			MSC_RECV_PORT,
			MSC_FORWARD_PORT,
			MSC_FORWARD_DOMAIN,
			MSC_CONNECT_MAX,
			MSC_CRYPT_MODE,
			MSC_RAWKEY,
		};

		private void UpdateUi()
		{
			this.StatusLabel.Text = "リスト更新中...";
			this.Refresh();

			try
			{
				using (new UISuspend(this.MainSheet))
				{
					int lastSelectRowIndex = this.GetMSSelectRowIndex();

					DataGridView ms = this.MainSheet;

					if (ms.ColumnCount == 0) // ? 未初期化
					{
						lastSelectRowIndex = 0;

						ms.RowCount = 0;
						ms.ColumnCount = 0;

						//ms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
						//ms.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

						ms.DefaultCellStyle.Font = new Font("メイリオ", 10f);

						this.MSAddColumn("状態");
						this.MSAddColumn("待ち受けポート");
						this.MSAddColumn("転送先ポート");
						this.MSAddColumn("転送先ホスト名");
						this.MSAddColumn("最大接続数");
						this.MSAddColumn("暗号モード");
						this.MSAddColumn("鍵");
					}
					if (Ground.I.ForwardInfoList.Count < ms.RowCount)
					{
						ms.RowCount = Ground.I.ForwardInfoList.Count;
					}
					ms.RowCount = Ground.I.ForwardInfoList.Count;

					for (int rowidx = 0; rowidx < Ground.I.ForwardInfoList.Count; rowidx++)
					{
						ForwardInfo fi = Ground.I.ForwardInfoList[rowidx];
						//ms.Rows.Add();
						DataGridViewRow r = ms.Rows[rowidx];

						string cryptMode = "----";

						if (fi.RawKey == "")
						{
							cryptMode = "なし";
						}
						else if (fi.DecMode)
						{
							cryptMode = "復号";
						}
						else
						{
							cryptMode = "暗号化";
						}

						r.Cells[(int)MSColumn_e.MSC_STARTED].Value = fi.Started ? "開始" : "停止";
						r.Cells[(int)MSColumn_e.MSC_RECV_PORT].Value = fi.RecvPortNo;
						r.Cells[(int)MSColumn_e.MSC_FORWARD_PORT].Value = fi.ForwardPortNo;
						r.Cells[(int)MSColumn_e.MSC_FORWARD_DOMAIN].Value = fi.ForwardDomain;
						r.Cells[(int)MSColumn_e.MSC_CONNECT_MAX].Value = fi.ConnectMax;
						r.Cells[(int)MSColumn_e.MSC_CRYPT_MODE].Value = cryptMode;
						r.Cells[(int)MSColumn_e.MSC_RAWKEY].Value = fi.RawKey;
					}
					ms.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

					foreach (DataGridViewColumn c in ms.Columns)
					{
						c.Width += 20;
					}
					this.MSSelectRow(lastSelectRowIndex, false);
				}
			}
			finally
			{
				this.StatusLabel.Text = Ground.I.STATUSLABEL_BLANK;
			}
		}
		private void MSAddColumn(string title)
		{
			this.MainSheet.Columns.Add(title, title);

			DataGridViewColumn c = this.MainSheet.Columns[this.MainSheet.Columns.Count - 1];

			c.HeaderCell.Style.Font = new Font("メイリオ", 10f);
			c.SortMode = DataGridViewColumnSortMode.NotSortable;
			c.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
		}
		private void MSSelectRow(int rowidx, bool scrollFlag)
		{
			if (rowidx < 0)
				rowidx = this.MainSheet.Rows.Count + rowidx;

			rowidx = Math.Min(rowidx, this.MainSheet.Rows.Count - 1);

			if (rowidx < 0)
			{
				this.MainSheet.ClearSelection();
			}
			else
			{
				this.MainSheet.Rows[rowidx].Selected = true;

				if (scrollFlag)
				{
					this.MainSheet.FirstDisplayedScrollingRowIndex = rowidx;
				}
			}
		}
		private int GetMSSelectRowIndex()
		{
			for (int rowidx = 0; rowidx < this.MainSheet.Rows.Count; rowidx++)
			{
				if (this.MainSheet.Rows[rowidx].Selected)
				{
					return rowidx;
				}
			}
			return -1; // not found
		}

		private void Btn開始停止_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				int rowidx = this.GetMSSelectRowIndex();

				if (rowidx == -1)
					return;

				ForwardInfo fi = Ground.I.ForwardInfoList[rowidx];

				if (fi.Started == false && fi.ポートの重複有り())
				{
#if true
#if false
					if (MessageBox.Show(
						"ポート " + fi.RecvPortNo + " を使用しているフォワードがあります。\n" +
						"続行すると当該フォワードを停止します。",
						"ポート番号の重複",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Warning,
						MessageBoxDefaultButton.Button2
						) != DialogResult.OK
						)
						return;
#endif

					foreach (ForwardInfo sFI in Ground.I.ForwardInfoList)
						if (sFI.RecvPortNo == fi.RecvPortNo)
							sFI.Started = false;
#else
					MessageBox.Show(
						"ポート " + fi.RecvPortNo + " を使用しているフォワードがあります。",
						"ポート番号の重複",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					return;
#endif
				}
				fi.Started = fi.Started ? false : true;

				// 間違えて停止してしまった時の猶予期間、要らない気がする...
				if (fi.Started == false)
					this.FISleepCount = 5;

				this.UpdateUi();
			}
		}

		private void Btn追加_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				if (Ground.I.MS_ROW_MAX <= Ground.I.ForwardInfoList.Count)
				{
					MessageBox.Show(
						"これ以上新しい項目は追加できません。",
						"行数制限",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);

					return;
				}

				Ground.I.ForwardInfoList.Add(new ForwardInfo());
				this.UpdateUi();
				this.MSSelectRow(-1, true);
			}
		}

		private void Btn削除_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				int rowidx = this.GetMSSelectRowIndex();

				if (rowidx == -1)
					return;

				if (Ground.I.ForwardInfoList[rowidx].停止チェック() == false)
					return;

				Ground.I.ForwardInfoList.RemoveAt(rowidx);
				this.UpdateUi();
				this.MSSelectRow(rowidx, false);
			}
		}

		private void Btn変更_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				int rowidx = this.GetMSSelectRowIndex();

				if (rowidx == -1)
					return;

				ForwardInfo fi = Ground.I.ForwardInfoList[rowidx];
				bool startFIStarted = fi.Started;

				using (Form f = new EditWin(fi))
				{
					f.ShowDialog();
				}
				fi.停止チェック();

				fi.Started = startFIStarted;
				this.UpdateUi();
			}
		}

		private void Btn上_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				int rowidx = this.GetMSSelectRowIndex();

				if (rowidx < 1)
					return;

				Tools.Swap(Ground.I.ForwardInfoList, rowidx - 1, rowidx);
				this.UpdateUi();
				this.MSSelectRow(rowidx - 1, false);
			}
		}

		private void Btn下_Click(object sender, EventArgs e)
		{
			using (this.TimerOff.LocalIncrement())
			{
				int rowidx = this.GetMSSelectRowIndex();

				if (rowidx == -1 || Ground.I.ForwardInfoList.Count - 1 <= rowidx)
					return;

				Tools.Swap(Ground.I.ForwardInfoList, rowidx, rowidx + 1);
				this.UpdateUi();
				this.MSSelectRow(rowidx + 1, false);
			}
		}

		public void SetSubStatusLabel(string text)
		{
			try
			{
				this.SubStatusLabel.Text = text;

				// zantei {
				this.SubStatusLabel.ForeColor = Color.Red;
				this.SubStatusLabel.BackColor = Color.Yellow;
				// }
			}
			catch
			{ }
		}

		private void MainWin_MouseClick(object sender, MouseEventArgs e)
		{
			//this.MainSheet.ClearSelection();
		}
	}
}
