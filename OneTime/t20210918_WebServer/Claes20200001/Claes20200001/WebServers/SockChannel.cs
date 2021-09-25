using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public class SockChannel
	{
		public Socket Handler;

		// <---- prm

		public void PostSetHandler()
		{
			this.Handler.Blocking = false;
		}

		public static bool StopFlag = false;

		/// <summary>
		/// セッションタイムアウト日時
		/// null == INFINITE
		/// </summary>
		public DateTime? SessionTimeoutTime = null;

		/// <summary>
		/// 無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public int IdleTimeoutMillis = 180000; // 3 min

		private DateTime? ThreadTimeoutTime = null;

		private void PreRecvSend()
		{
			if (StopFlag)
			{
				throw new Exception("停止リクエスト");
			}
			if (this.SessionTimeoutTime != null && this.SessionTimeoutTime.Value < DateTime.Now)
			{
				throw new SessionTimeoutException();
			}
			if (this.ThreadTimeoutTime == null)
			{
				this.ThreadTimeoutTime = DateTime.Now + TimeSpan.FromMilliseconds(100.0);
			}
			else if (this.ThreadTimeoutTime.Value < DateTime.Now)
			{
				this.ThreadTimeoutTime = null;
				Critical.ContextSwitching();
			}
		}

		public byte[] Recv(int size)
		{
			byte[] data = new byte[size];

			this.Recv(data);

			return data;
		}

		public void Recv(byte[] data, int offset = 0)
		{
			this.Recv(data, offset, data.Length - offset);
		}

		public void Recv(byte[] data, int offset, int size)
		{
			while (1 <= size)
			{
				int recvSize = this.TryRecv(data, offset, size);

				size -= recvSize;
				offset += recvSize;
			}
		}

		public void Recv(byte[] buff, SCommon.Write_d writer)
		{
			writer(buff, 0, TryRecv(buff, 0, buff.Length));
		}

		public int TryRecv(byte[] data, int offset, int size)
		{
			int waitMillis = 0;
			int idleMillis = 0;

			for (; ; )
			{
				this.PreRecvSend();

				try
				{
					int recvSize = SockCommon.NonBlocking("recv", () => this.Handler.Receive(data, offset, size, SocketFlags.None));

					if (recvSize <= 0)
					{
						throw new Exception("受信エラー(切断)");
					}
					return recvSize;
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("受信エラー", e);
					}
				}
				if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis <= idleMillis)
				{
					throw new IdleTimeoutException();
				}
				if (waitMillis < 100)
					waitMillis++;

				Critical.Unsection(() => Thread.Sleep(waitMillis));

				idleMillis += waitMillis;
			}
		}

		public class SessionTimeoutException : Exception
		{ }

		public class IdleTimeoutException : Exception
		{ }

		public void Send(byte[] data, int offset = 0)
		{
			this.Send(data, offset, data.Length - offset);
		}

		public void Send(byte[] data, int offset, int size)
		{
			while (1 <= size)
			{
				int sentSize = this.TrySend(data, offset, Math.Min(4 * 1024 * 1024, size));

				size -= sentSize;
				offset += sentSize;
			}
		}

		private int TrySend(byte[] data, int offset, int size)
		{
			int waitMillis = 0;
			int idleMillis = 0;

			for (; ; )
			{
				this.PreRecvSend();

				try
				{
					int sentSize = SockCommon.NonBlocking("send", () => this.Handler.Send(data, offset, size, SocketFlags.None));

					if (sentSize <= 0)
					{
						throw new Exception("送信エラー(切断)");
					}
					return sentSize;
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("送信エラー", e);
					}
				}
				if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis <= idleMillis)
				{
					throw new Exception("送信タイムアウト");
				}
				if (waitMillis < 100)
					waitMillis++;

				Critical.Unsection(() => Thread.Sleep(waitMillis));

				idleMillis += waitMillis;
			}
		}

		public static SockCommon.Critical Critical = new SockCommon.Critical();
	}
}
