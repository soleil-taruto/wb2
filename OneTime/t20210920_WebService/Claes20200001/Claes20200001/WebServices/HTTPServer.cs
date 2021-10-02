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

			foreach (var dummy in hsChannel.RecvRequest(() => { }))
				yield return GetRecvedOrSentReset(channel) ? 1 : 0;

			SockCommon.NB("svlg", () =>
			{
				HTTPConnected(hsChannel);
				return -1; // dummy
			});

			foreach (var dummy in hsChannel.SendResponse(() => { }))
				yield return GetRecvedOrSentReset(channel) ? 1 : 0;
		}

		private static bool GetRecvedOrSentReset(SockChannel channel)
		{
			if (channel.RecvedOrSent)
			{
				channel.RecvedOrSent = false;
				return true;
			}
			return false;
		}
	}
}
