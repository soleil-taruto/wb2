using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Charlotte.Commons;

namespace Charlotte.WebServices
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
			{ // TODO del
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しています...");

				try
				{
					using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						IPEndPoint endPoint = new IPEndPoint(0L, this.PortNo);

						listener.Bind(endPoint);
						listener.Listen(this.Backlog);
						listener.Blocking = false;

						SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しました。");

						DateTime? threadTimeoutTime = null; // TODO del
						int connectWaitMillis = 0;

						while (this.Interlude())
						{
							Socket handler = this.ConnectedThs.Count < this.ConnectMax ? this.Connect(listener) : null;

							if (handler == null)
							{
								threadTimeoutTime = null;

								if (connectWaitMillis < 100)
									connectWaitMillis++;

								Thread.Sleep(connectWaitMillis); // TODO del
							}
							else
							{
								connectWaitMillis = 0;

								SockCommon.TimeWaitMonitor.Connected();

								{
									SockChannel channel = new SockChannel();

									channel.Handler = handler;
									handler = null;
									channel.PostSetHandler();

									Thread th = new Thread(() => // TODO del Thread
									{
										SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信開始 " + Thread.CurrentThread.ManagedThreadId + " " + channel.Handler.RemoteEndPoint);

										try
										{
											this.Connected(channel);

											SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信終了 " + Thread.CurrentThread.ManagedThreadId);
										}
										catch (HTTPServerChannel.RecvFirstLineIdleTimeoutException)
										{
											SockCommon.WriteLog(SockCommon.ErrorLevel_e.FIRST_LINE_TIMEOUT, null);
										}
										catch (Exception e)
										{
											SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK_OR_SERVER_LOGIC, e);
										}

										try
										{
											channel.Handler.Shutdown(SocketShutdown.Both);
										}
										catch (Exception e)
										{
											SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, e);
										}

										try
										{
											channel.Handler.Close();
										}
										catch (Exception e)
										{
											SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, e);
										}

										SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "切断します。" + Thread.CurrentThread.ManagedThreadId);
										SockCommon.TimeWaitMonitor.Disconnect();
									}
									);

									th.Start();

									this.ConnectedThs.Add(th);
								}
							}

							for (int index = this.ConnectedThs.Count - 1; 0 <= index; index--)
								if (!this.ConnectedThs[index].IsAlive)
									SCommon.FastDesertElement(this.ConnectedThs, index);

							//GC.Collect(); // GeoDemo の Server.sln が重くなるため、暫定削除 @ 2019.4.9
						}

						SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しています...");
					}
				}
				catch (Exception e)
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, e);
				}

				this.Stop();

				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しました。");
			} // del
		}

		private Socket Connect(Socket listener) // ret: null == 接続タイムアウト
		{
			try
			{
				return SockCommon.NB("conn", () => listener.Accept());
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
			WaitToStop();
			SockChannel.StopFlag = false;
		}

		private void WaitToStop()
		{
			foreach (Thread connectedTh in this.ConnectedThs)
				connectedTh.Join(); // TODO: del Thread

			this.ConnectedThs.Clear();
		}
	}
}
