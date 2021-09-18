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
		public class ThreadEx : IDisposable
		{
			private Thread Th;
			private Exception Ex = null;

			public ThreadEx(Action routine)
			{
				Th = new Thread(() =>
				{
					try
					{
						routine();
					}
					catch (Exception e)
					{
						Ex = e;
					}
				});

				Th.Start();
			}

			public bool IsEnded(int millis = 0)
			{
				if (Th != null && Th.Join(millis))
					Th = null;

				return Th == null;
			}

			public void WaitToEnd()
			{
				if (Th != null)
				{
					Th.Join();
					Th = null;
				}
			}

			public void WaitToEnd(Critical critical)
			{
				if (Th != null)
				{
					critical.Unsection(() => Th.Join());
					Th = null;
				}
			}

			public void RelayThrow()
			{
				this.WaitToEnd();

				if (this.Ex != null)
					throw new AggregateException("Relay", this.Ex);
			}

			public Exception GetException()
			{
				this.WaitToEnd();
				return this.Ex;
			}

			public void Dispose()
			{
				this.WaitToEnd();
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
