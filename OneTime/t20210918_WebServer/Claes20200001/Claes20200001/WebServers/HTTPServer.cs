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
				Console.WriteLine("*1"); // test
				hsChannel.RecvRequest();
				Console.WriteLine("*2"); // test

				HTTPConnected(hsChannel);
				Console.WriteLine("*3"); // test

				hsChannel.SendResponse();
				Console.WriteLine("*4"); // test
			};
		}
	}
}
