using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	public static class Common
	{
		#region P_EventManager

		public class P_EventManager
		{
			private bool TimerStarted = false;

			/// <summary>
			/// タイマー開始
			/// </summary>
			public void StartTimer()
			{
				this.TimerStarted = true;
			}

			/// <summary>
			/// タイマー終了
			/// </summary>
			public void EndTimer()
			{
				this.TimerStarted = false;
			}

			/// <summary>
			/// タイマーのイベント実行
			/// </summary>
			/// <param name="routine">イベントロジック</param>
			public void TimerEventHandler(Action routine)
			{
				if (this.TimerStarted)
					this.EventHandler(true, routine);
			}

			private bool Busy = false;

			/// <summary>
			/// タイマー以外のイベント実行
			/// </summary>
			/// <param name="background">バックグラウンドで動くイベントか</param>
			/// <param name="routine">イベントロジック</param>
			public void EventHandler(bool background, Action routine)
			{
				if (this.Busy)
					return;

				this.Busy = true;
				try
				{
					routine();
				}
				catch (Exception e)
				{
					if (background)
						ProcMain.WriteLog(e);
					else
						MessageBox.Show("" + e, "失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				finally
				{
					this.Busy = false;
				}
			}
		}

		#endregion
	}
}
