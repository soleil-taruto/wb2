using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public class SockServer
	{
		public int PortNo = 59999;
		public int Backlog = 100;
		public int ConnectMax = 30;
		public Action<SockChannel> Connected = channel => { };
		public Func<bool> Interlude = () => !Console.KeyAvailable;

		// <---- prm

		public static void ErrorOccurred_Ignorable(object message)
		{
			ProcMain.WriteLog(message);
		}

		public static void ErrorOccurred_Fatal(object message)
		{
			ProcMain.WriteLog("[FATAL] " + message);
		}

		public static void ErrorOccurred_RecvFirstLineIdleTimeout()
		{
			ProcMain.WriteLog("RECV_FIRST_LINE_IDLE_TIMEOUT");
		}

		private List<SockCommon.ThreadEx> ConnectedThs = new List<SockCommon.ThreadEx>();

		public void Perform()
		{
			SockChannel.Critical.Section(() =>
			{
				try
				{
					using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						IPEndPoint endPoint = new IPEndPoint(0L, this.PortNo);

						listener.Bind(endPoint);
						listener.Listen(this.Backlog);
						listener.Blocking = false;

						int connectWaitMillis = 0;

						while (this.Interlude())
						{
							Socket handler = this.ConnectedThs.Count < this.ConnectMax ? this.Connect(listener) : null;

							if (handler == null)
							{
								if (connectWaitMillis < 100)
									connectWaitMillis++;

								SockChannel.Critical.Unsection(() => Thread.Sleep(connectWaitMillis));
							}
							else
							{
								connectWaitMillis = 0;

								{
									SockChannel channel = new SockChannel();

									channel.Handler = handler;
									handler = null;
									channel.PostSetHandler();

									this.ConnectedThs.Add(new SockCommon.ThreadEx(() => SockChannel.Critical.Section(() =>
									{
										try
										{
											this.Connected(channel);
										}
										catch (HTTPServerChannel.RecvFirstLineIdleTimeoutException)
										{
											ErrorOccurred_RecvFirstLineIdleTimeout();
										}
										catch (Exception e)
										{
											ErrorOccurred_Ignorable(e);
										}

										try
										{
											channel.Handler.Shutdown(SocketShutdown.Both);
										}
										catch (Exception e)
										{
											ErrorOccurred_Ignorable(e);
										}

										try
										{
											channel.Handler.Close();
										}
										catch (Exception e)
										{
											ErrorOccurred_Ignorable(e);
										}
									}
									)));
								}
							}

							for (int index = this.ConnectedThs.Count - 1; 0 <= index; index--)
								if (this.ConnectedThs[index].IsEnded())
									this.ConnectedThs.RemoveAt(index);

							//GC.Collect(); // GeoDemo の Server.sln が重くなるため、暫定削除 @ 2019.4.9
						}
					}
				}
				catch (Exception e)
				{
					ErrorOccurred_Fatal(e);
				}

				this.Stop();
			});
		}

		private Socket Connect(Socket listener) // ret: null == 接続タイムアウト
		{
			try
			{
				return listener.Accept();
			}
			catch (SocketException e)
			{
				if (e.ErrorCode != 10035)
				{
					throw new Exception("接続エラー", e);
				}
				return null;
			}
		}

		private void Stop()
		{
			SockChannel.StopFlag = true;
			Stop_ChannelSafe();
			SockChannel.StopFlag = false;
		}

		private void Stop_ChannelSafe()
		{
			foreach (SockCommon.ThreadEx connectedTh in this.ConnectedThs)
				connectedTh.WaitToEnd(SockChannel.Critical);

			this.ConnectedThs.Clear();
		}
	}
}
