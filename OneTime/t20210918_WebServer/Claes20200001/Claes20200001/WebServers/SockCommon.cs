using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public static class SockCommon
	{
		public enum ErrorLevel_e
		{
			INFO = 1,
			FIRST_LINE_TIMEOUT,
			NETWORK,
			NETWORK_OR_SERVER_LOGIC,
			FATAL,
		}

		public static void WriteLog(ErrorLevel_e errorLevel, object message)
		{
			switch (errorLevel)
			{
				case ErrorLevel_e.INFO:
					ProcMain.WriteLog(message);
					break;

				case ErrorLevel_e.FIRST_LINE_TIMEOUT:
					ProcMain.WriteLog("[FIRST-LINE-TIMEOUT]");
					break;

				case ErrorLevel_e.NETWORK:
					ProcMain.WriteLog("[NETWORK] " + message);
					break;

				case ErrorLevel_e.NETWORK_OR_SERVER_LOGIC:
					ProcMain.WriteLog("[NETWORK-SERVER-LOGIC] " + message);
					break;

				case ErrorLevel_e.FATAL:
					ProcMain.WriteLog("[FATAL] " + message);
					break;

				default:
					throw null; // never
			}
		}

		public static T NB<T>(string title, Func<T> routine)
		{
#if !true
			return routine();
#else
			DateTime startedTime = DateTime.Now;
			try
			{
				return routine();
			}
			finally
			{
				double millis = (DateTime.Now - startedTime).TotalMilliseconds;

				const double MILLIS_LIMIT = 50.0;

				if (MILLIS_LIMIT < millis)
					ProcMain.WriteLog("ブロックしない処理に掛かった時間 " + title + " " + Thread.CurrentThread.ManagedThreadId + " " + millis.ToString("F0"));
			}
#endif
		}

		public class Critical : Semaphore
		{
			public Critical()
				: base(1)
			{ }
		}

		public class Semaphore
		{
			private int Permit;

			public Semaphore(int permit)
			{
				if (permit < 1 || SCommon.IMAX < permit)
					throw new ArgumentException();

				this.Permit = permit;
			}

			private object Enter_SYNCROOT = new object();
			private object SYNCROOT = new object();
			private int Entry = 0;

			// @ 2019.1.7
			// Monitor.Enter -> Monitor.Exit は同一スレッドでなければならないっぽい。
			// (Enter -> 別スレッドで Leave)出来るように ---> (Monitor.Wait -> Monitor.Pulse)にした。

			public void Enter()
			{
				lock (Enter_SYNCROOT)
				{
					lock (SYNCROOT)
					{
						Entry++;

						if (Entry == Permit + 1)
							Monitor.Wait(SYNCROOT);
					}
				}
			}

			public void Leave()
			{
				lock (SYNCROOT)
				{
					if (Entry == 0)
						throw null; // never

					Entry--;

					if (Entry == Permit)
						Monitor.Pulse(SYNCROOT);
				}
			}

			public void Section(Action routine)
			{
				this.Enter();
				try
				{
					routine();
				}
				finally
				{
					this.Leave();
				}
			}

			public void Unsection(Action routine)
			{
				this.Leave();
				try
				{
					routine();
				}
				finally
				{
					this.Enter();
				}
			}

			public void ContextSwitching()
			{
				this.Leave();
				this.Enter();
			}
		}

		public static class TimeWaitMonitor
		{
			private static int[] Counters = new int[5]; // 過去４～５分の間に発生した切断(TIME_WAIT)の回数
			private static int CounterIndex = 0;
			private static DateTime NextRotateTime = GetNextRotateTime();

			public static void Connected()
			{
				AddTimeWait(0);

				if (10000 < Counters.Sum()) // ? TIME_WAIT 多すぎ -> 時間当たりの接続数を制限する。-- TIME_WAIT を減らす。
				{
					ProcMain.WriteLog("[WARNING] PORT-EXHAUSTION");

					SockChannel.Critical.Unsection(() => Thread.Sleep(50));
				}
			}

			public static void Disconnect()
			{
				AddTimeWait(1);
			}

			private static void AddTimeWait(int valAdd)
			{
				if (NextRotateTime < DateTime.Now)
				{
					CounterIndex++;
					CounterIndex %= Counters.Length;
					Counters[CounterIndex] = valAdd;
					NextRotateTime = GetNextRotateTime();
				}
				else
					Counters[CounterIndex] += valAdd;

				ProcMain.WriteLog("TIME-WAIT-MONITOR: " + valAdd + ", " + Counters.Sum());
			}

			private static DateTime GetNextRotateTime()
			{
				return DateTime.Now + TimeSpan.FromMinutes(1.0);
			}
		}
	}
}
