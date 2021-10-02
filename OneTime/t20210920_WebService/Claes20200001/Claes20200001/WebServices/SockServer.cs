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
	public abstract class SockServer
	{
		/// <summary>
		/// ポート番号
		/// </summary>
		public int PortNo = 59999;

		/// <summary>
		/// 接続待ちキューの長さ
		/// </summary>
		public int Backlog = 200;

		/// <summary>
		/// 最大同時接続数
		/// </summary>
		public int ConnectMax = 100;

		/// <summary>
		/// サーバーロジック
		/// 通信量：
		/// -- 0 未満 == 通信終了
		/// -- 0 == 通信無し
		/// -- 1 以上 == 通信有り
		/// </summary>
		/// <param name="channel">接続チャネル</param>
		/// <returns>通信量</returns>
		public abstract IEnumerable<int> E_Connected(SockChannel channel);

		/// <summary>
		/// 処理の合間に呼ばれる処理
		/// 戻り値：
		/// -- サーバーを継続するか
		/// </summary>
		public Func<bool> Interlude = () => !Console.KeyAvailable;

		// <---- prm

		private List<SockChannel> Channels = new List<SockChannel>();

		public void Perform()
		{
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

					int waitMillis = 0;

					while (this.Interlude())
					{
						if (0 < waitMillis)
							Thread.Sleep(waitMillis);

						if (waitMillis < 100)
							waitMillis++;

						Socket handler = this.Channels.Count < this.ConnectMax ? this.Connect(listener) : null;

						if (handler != null)
						{
							waitMillis = 0;

							SockCommon.TimeWaitMonitor.Connected();

							{
								SockChannel channel = new SockChannel();

								channel.Handler = handler;
								handler = null;
								channel.PostSetHandler();
								channel.ID = channel.GetHashCode(); // HACK
								channel.Connected = SCommon.Supplier(this.E_Connected(channel));
								channel.BodyOutputStream = new HTTPBodyOutputStream();

								this.Channels.Add(channel);

								SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信開始 " + channel.ID);
							}
						}
						for (int index = this.Channels.Count - 1; 0 <= index; index--)
						{
							SockChannel channel = this.Channels[index];
							int size;

							try
							{
								size = channel.Connected();

								if (0 < size) // ? 通信有り
								{
									waitMillis = 0;
								}
							}
							catch (HTTPServerChannel.RecvFirstLineIdleTimeoutException)
							{
								SockCommon.WriteLog(SockCommon.ErrorLevel_e.FIRST_LINE_TIMEOUT, null);
								size = -1;
							}
							catch (Exception e)
							{
								SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK_OR_SERVER_LOGIC, e);
								size = -1;
							}

							if (size < 0) // ? 切断
							{
								SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信終了 " + channel.ID);

								this.Disconnect(channel);
								SCommon.FastDesertElement(this.Channels, index);

								SockCommon.TimeWaitMonitor.Disconnect();
							}
						}

						//GC.Collect(); // GeoDemo の Server.sln が重くなるため、暫定削除 @ 2019.4.9
					}

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しています...");

					this.Stop();
				}
			}
			catch (Exception e)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, e);
			}

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しました。");
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
			foreach (SockChannel channel in this.Channels)
				this.Disconnect(channel);

			this.Channels.Clear();
		}

		private void Disconnect(SockChannel channel)
		{
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

			try
			{
				channel.BodyOutputStream.Dispose();
			}
			catch (Exception e)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, e);
			}
		}
	}
}
