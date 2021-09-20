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
			FIRST_LINE_TIMEOUT,
			NETWORK,
			NETWORK_OR_SERVER_LOGIC,
			FATAL,
		}

		public static void ErrorLog(ErrorLevel_e errorKind, object message)
		{
			switch (errorKind)
			{
				case ErrorLevel_e.FIRST_LINE_TIMEOUT:
					ProcMain.WriteLog("FIRST_LINE_TIMEOUT");
					break;

				case ErrorLevel_e.NETWORK:
				case ErrorLevel_e.NETWORK_OR_SERVER_LOGIC:
					ProcMain.WriteLog(message);
					break;

				case ErrorLevel_e.FATAL:
					ProcMain.WriteLog("[FATAL] " + message);
					break;

				default:
					throw null; // never
			}
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
	}
}
