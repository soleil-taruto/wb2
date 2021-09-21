﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	public static class Common
	{
		#region EventManager

		public class EventManager
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
			public void ExecuteTimer(Action routine)
			{
				if (this.TimerStarted)
					this.Execute(routine);
			}

			private bool Busy = false;

			/// <summary>
			/// タイマー以外のイベント実行
			/// </summary>
			/// <param name="routine">イベントロジック</param>
			public void Execute(Action routine)
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
					ProcMain.WriteLog(e);
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
