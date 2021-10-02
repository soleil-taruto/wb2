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

		public HTTPServer()
		{
			PortNo = 80;
		}

		public override IEnumerable<int> E_Connected(SockChannel channel)
		{
			HTTPServerChannel hsChannel = new HTTPServerChannel();

			hsChannel.Channel = channel;
			hsChannel.RecvRequest(); // TODO

			SockCommon.NB("svlg", () =>
			{
				HTTPConnected(hsChannel);
				return -1; // dummy
			});

			hsChannel.SendResponse(); // TODO

			yield break; // TODO
		}
	}
}
