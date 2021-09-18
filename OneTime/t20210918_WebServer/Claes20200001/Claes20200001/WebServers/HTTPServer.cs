using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.WebServers
{
	public class HTTPServer : SockServer
	{
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

				HTTPConnected(hsChannel);

				hsChannel.SendResponse();
			};
		}
	}
}
