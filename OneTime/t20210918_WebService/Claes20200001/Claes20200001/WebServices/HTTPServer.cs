using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.WebServices
{
	public class HTTPServer : SockServer
	{
		/// <summary>
		/// サーバーロジック
		/// 引数：
		/// -- channel: 接続チャネル
		/// </summary>
		public Action<HTTPServerChannel> HTTPConnected = channel => { };

		// <---- prm

		/// <summary>
		/// Keep-Alive-タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int KeepAliveTimeoutMillis = 5000;

		public HTTPServer()
		{
			PortNo = 80;
			Connected = channel =>
			{
				DateTime startedTime = DateTime.Now;

				for (; ; )
				{
					HTTPServerChannel hsChannel = new HTTPServerChannel();

					hsChannel.Channel = channel;
					hsChannel.RecvRequest();

					SockCommon.NB("svlg", () =>
					{
						HTTPConnected(hsChannel);
						return -1; // dummy
					});

					if (KeepAliveTimeoutMillis != -1 && KeepAliveTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						hsChannel.KeepAlive = false;
					}

					hsChannel.SendResponse();

					if (!hsChannel.KeepAlive)
						break;
				}
			};
		}
	}
}
