using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Charlotte.Commons;

namespace Charlotte.WebServices
{
	public class SockChannel
	{
		public Socket Handler;
		public int ID;
		public Func<int> Connected;
		public HTTPBodyOutputStream BodyOutputStream;

		// <---- prm

		public void PostSetHandler()
		{
			this.Handler.Blocking = false;
		}

		public bool RecvFirstLineFlag = false;

		/// <summary>
		/// セッションタイムアウト日時
		/// null == INFINITE
		/// </summary>
		public DateTime? SessionTimeoutTime = null;

		/// <summary>
		/// 無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public int IdleTimeoutMillis = -1;

		private void PreRecvSend()
		{
			if (this.SessionTimeoutTime != null && this.SessionTimeoutTime.Value < DateTime.Now)
			{
				throw new Exception("セッション時間切れ");
			}
		}

		public IEnumerable<int> Recv(int size, Action<byte[]> a_return)
		{
			byte[] data = new byte[size];
			int offset = 0;

			while (1 <= size)
			{
				int? recvSize = null;

				foreach (var relay in this.TryRecv(data, offset, size, ret => recvSize = ret))
					yield return relay;

				size -= recvSize.Value;
				offset += recvSize.Value;
			}
			a_return(data);
		}

		public IEnumerable<int> TryRecv(byte[] data, int offset, int size, Action<int> a_return)
		{
			DateTime startedTime = DateTime.Now;

			for (; ; )
			{
				this.PreRecvSend();

				try
				{
					int recvSize = SockCommon.NB("recv", () => this.Handler.Receive(data, offset, size, SocketFlags.None));

					if (recvSize <= 0)
					{
						throw new Exception("受信エラー(切断)");
					}
					a_return(recvSize);
					break;
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("受信エラー", e);
					}
				}
				if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
				{
					throw new RecvIdleTimeoutException();
				}
				yield return -1;
			}
			yield return 1;
		}

		/// <summary>
		/// 受信の無通信タイムアウト
		/// </summary>
		public class RecvIdleTimeoutException : Exception
		{ }

		public IEnumerable<int> Send(byte[] data, Action a_return)
		{
			int offset = 0;
			int size = data.Length;

			while (1 <= size)
			{
				int? sentSize = null;

				foreach (var relay in this.TrySend(data, offset, Math.Min(4 * 1024 * 1024, size), ret => sentSize = ret))
					yield return relay;

				size -= sentSize.Value;
				offset += sentSize.Value;
			}
			a_return();
		}

		private IEnumerable<int> TrySend(byte[] data, int offset, int size, Action<int> a_return)
		{
			DateTime startedTime = DateTime.Now;

			for (; ; )
			{
				this.PreRecvSend();

				try
				{
					int sentSize = SockCommon.NB("send", () => this.Handler.Send(data, offset, size, SocketFlags.None));

					if (sentSize <= 0)
					{
						throw new Exception("送信エラー(切断)");
					}
					a_return(sentSize);
					break;
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("送信エラー", e);
					}
				}
				if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
				{
					throw new Exception("送信の無通信タイムアウト");
				}
				yield return -1;
			}
			yield return 1;
		}
	}
}
