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

		public IEnumerable<byte[]> Recv(int size)
		{
			if (size <= 0)
				throw null; // never

			byte[] data = new byte[size];
			int offset = 0;

			foreach (int recvSize in this.TryRecv(data, offset, size))
			{
				size -= recvSize;
				offset += recvSize;

				if (size <= 0)
					break;

				yield return null;
			}
			yield return data;
		}

		public IEnumerable<int> TryRecv(byte[] data, int offset, int size)
		{
			DateTime startedTime = DateTime.Now;

			for (; ; )
			{
				int recvSize = 0;

				this.PreRecvSend();

				try
				{
					recvSize = SockCommon.NB("recv", () => this.Handler.Receive(data, offset, size, SocketFlags.None));

					if (recvSize <= 0)
					{
						throw new Exception("受信エラー(切断)");
					}
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
				yield return recvSize;
			}
		}

		/// <summary>
		/// 受信の無通信タイムアウト
		/// </summary>
		public class RecvIdleTimeoutException : Exception
		{ }

		public IEnumerable<bool> Send(byte[] data)
		{
			int offset = 0;
			int size = data.Length;

			if (size <= 0)
				throw null; // never

			foreach (int sentSize in this.TrySend(data, offset, Math.Min(4 * 1024 * 1024, size)))
			{
				size -= sentSize;
				offset += sentSize;

				if (size <= 0)
					break;

				yield return true;
			}
		}

		private IEnumerable<int> TrySend(byte[] data, int offset, int size)
		{
			DateTime startedTime = DateTime.Now;

			for (; ; )
			{
				int sentSize = 0;

				this.PreRecvSend();

				try
				{
					sentSize = SockCommon.NB("send", () => this.Handler.Send(data, offset, size, SocketFlags.None));

					if (sentSize <= 0)
					{
						throw new Exception("送信エラー(切断)");
					}
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
				yield return sentSize;
			}
		}
	}
}
