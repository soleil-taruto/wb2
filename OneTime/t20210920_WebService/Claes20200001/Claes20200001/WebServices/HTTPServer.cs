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
			int retval = -1;

			hsChannel.Channel = channel;

			foreach (int size in hsChannel.RecvRequest(() => { }))
			{
				if (size < 0)
				{
					yield return retval;
					retval = -1;
				}
				else
					retval = 1;
			}

			SockCommon.NB("svlg", () =>
			{
				HTTPConnected(hsChannel);
				return false; // dummy
			});

			foreach (int size in hsChannel.SendResponse(() => { }))
			{
				if (size < 0)
				{
					yield return retval;
					retval = -1;
				}
				else
					retval = 1;
			}
		}
	}
}
