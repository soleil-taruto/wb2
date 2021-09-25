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
		/// <summary>
		/// ポート番号
		/// </summary>
		public int PortNo = 59999;

		/// <summary>
		/// 接続待ちキューの長さ
		/// </summary>
		public int Backlog = 100;

		/// <summary>
		/// 最大同時接続数
		/// </summary>
		public int ConnectMax = 30;

		/// <summary>
		/// サーバーロジック
		/// 引数：
		/// -- channel: 接続チャネル
		/// </summary>
		public Action<SockChannel> Connected = channel => { };

		/// <summary>
		/// 処理の合間に呼ばれる処理
		/// 戻り値：
		/// -- サーバーを継続するか
		/// </summary>
		public Func<bool> Interlude = () => !Console.KeyAvailable;

		// <---- prm

		private List<Thread> ConnectedThs = new List<Thread>();

		public void Perform()
		{
			SockChannel.Critical.Section(() =>
			{
				try
				{
					ProcMain.WriteLog("STARTING"); // test
					using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						IPEndPoint endPoint = new IPEndPoint(0L, this.PortNo);

						listener.Bind(endPoint);
						listener.Listen(this.Backlog);
						listener.Blocking = false;
						ProcMain.WriteLog("STARTED"); // test

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

									Thread th = new Thread(() => SockChannel.Critical.Section(() =>
									{
										try
										{
											SockCommon.NonBlocking("server-logic", () => this.Connected(channel));
										}
										catch (HTTPServerChannel.RecvFirstLineIdleTimeoutException)
										{
											SockCommon.ErrorLog(SockCommon.ErrorLevel_e.FIRST_LINE_TIMEOUT, null);
										}
										catch (Exception e)
										{
											SockCommon.ErrorLog(SockCommon.ErrorLevel_e.NETWORK_OR_SERVER_LOGIC, e);
										}

										try
										{
											SockCommon.NonBlocking("shutdown", () => channel.Handler.Shutdown(SocketShutdown.Both));
										}
										catch (Exception e)
										{
											SockCommon.ErrorLog(SockCommon.ErrorLevel_e.NETWORK, e);
										}

										try
										{
											SockCommon.NonBlocking("close", () => channel.Handler.Close());
										}
										catch (Exception e)
										{
											SockCommon.ErrorLog(SockCommon.ErrorLevel_e.NETWORK, e);
										}
									}
									));

									th.Start();

									this.ConnectedThs.Add(th);
								}
							}

							for (int index = this.ConnectedThs.Count - 1; 0 <= index; index--)
								if (!this.ConnectedThs[index].IsAlive)
									SCommon.FastDesertElement(this.ConnectedThs, index);

							//GC.Collect(); // GeoDemo の Server.sln が重くなるため、暫定削除 @ 2019.4.9
						}
					}
				}
				catch (Exception e)
				{
					SockCommon.ErrorLog(SockCommon.ErrorLevel_e.FATAL, e);
				}

				ProcMain.WriteLog("ENDING"); // test
				this.Stop();
				ProcMain.WriteLog("ENDED"); // test
			});
		}

		private Socket Connect(Socket listener) // ret: null == 接続タイムアウト
		{
			try
			{
				return SockCommon.NonBlocking("conn", () => listener.Accept());
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
			foreach (Thread connectedTh in this.ConnectedThs)
				SockChannel.Critical.Unsection(() => connectedTh.Join());

			this.ConnectedThs.Clear();
		}
	}
}
