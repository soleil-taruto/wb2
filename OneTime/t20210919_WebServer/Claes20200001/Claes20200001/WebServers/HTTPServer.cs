using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.WebServers
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

		public HTTPServer()
		{
			PortNo = 80;
			Connected = channel =>
			{
				HTTPServerChannel hsChannel = new HTTPServerChannel();

				hsChannel.Channel = channel;
				hsChannel.RecvRequest();

				SockCommon.NB("svlg", () =>
				{
					HTTPConnected(hsChannel);
					return -1; // dummy
				});

				hsChannel.SendResponse();
			};
		}
	}
}
