using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public class SockClient : SockChannel, IDisposable
	{
		public SockClient()
		{
			SockChannel.Critical.Enter();
		}

		public void Connect(string domain, int portNo, int connectTimeoutMillis = 20000) // 20 sec
		{
			// TODO connectTimeoutMillis 対応

			IPHostEntry hostEntry = Dns.GetHostEntry(domain);
			IPAddress address = GetFairAddress(hostEntry.AddressList);
			IPEndPoint endPoint = new IPEndPoint(address, portNo);

			this.Handler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.Handler.Connect(endPoint);

			this.PostSetHandler();
		}

		private static IPAddress GetFairAddress(IPAddress[] addresses)
		{
			foreach (IPAddress address in addresses)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork) // ? IPv4
				{
					return address;
				}
			}
			return addresses[0];
		}

		/// <summary>
		/// このメソッドは例外を投げないこと。
		/// </summary>
		public void Dispose()
		{
			if (this.Handler != null)
			{
				try
				{
					this.Handler.Disconnect(false);
				}
				catch (Exception e)
				{
					ProcMain.WriteLog(e);
				}

				try
				{
					this.Handler.Dispose();
				}
				catch (Exception e)
				{
					ProcMain.WriteLog(e);
				}

				this.Handler = null;

				try
				{
					SockChannel.Critical.Leave();
				}
				catch (Exception e)
				{
					ProcMain.WriteLog(e);
				}
			}
		}
	}
}
