using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.WebServers
{
	public class HTTPServerChannel
	{
		public SockChannel Channel;

		/// <summary>
		/// 要求タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int RequestTimeoutMillis = -1;

		/// <summary>
		/// 応答タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int ResponseTimeoutMillis = -1;

		// memo: チャンク毎のタイムアウトは IdleTimeoutMillis で代替する。

		/// <summary>
		/// リクエストの最初の行のみの無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int FirstLineTimeoutMillis = 2000;

		/// <summary>
		/// リクエストの最初の行以外の(レスポンスも含む)無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int IdleTimeoutMillis = 180000; // 3 min

		/// <summary>
		/// リクエストのボディの最大サイズ_バイト数
		/// </summary>
		public static int BodySizeMax = 300000000; // 300 MB

		public void RecvRequest()
		{
			this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(RequestTimeoutMillis);
			this.Channel.IdleTimeoutMillis = FirstLineTimeoutMillis;

			try
			{
				this.FirstLine = this.RecvLine();
			}
			catch (SockChannel.IdleTimeoutException)
			{
				throw new RecvFirstLineIdleTimeoutException();
			}

			{
				string[] tokens = this.FirstLine.Split(' ');

				this.Method = tokens[0];
				this.Path = DecodeURL(tokens[1]);
				this.HTTPVersion = tokens[2];
			}

			this.Channel.IdleTimeoutMillis = IdleTimeoutMillis;

			this.RecvHeader();
			this.CheckHeader();

			if (this.Expect100Continue)
			{
				this.SendLine("HTTP/1.1 100 Continue");
				this.SendLine("");
			}
			this.RecvBody();
		}

		private static DateTime? TimeoutMillisToDateTime(int millis)
		{
			if (millis == -1)
				return null;

			return DateTime.Now + new TimeSpan((long)millis * TimeSpan.TicksPerMillisecond);
		}

		public class RecvFirstLineIdleTimeoutException : Exception
		{ }

		private string DecodeURL(string path)
		{
			byte[] src = Encoding.ASCII.GetBytes(path);

			using (MemoryStream dest = new MemoryStream())
			{
				for (int index = 0; index < src.Length; index++)
				{
					if (src[index] == 0x25) // ? '%'
					{
						dest.WriteByte((byte)Convert.ToInt32(Encoding.ASCII.GetString(SCommon.GetSubBytes(src, index + 1, 2)), 16));
						index += 2;
					}
					else if (src[index] == 0x2b) // ? '+'
					{
						dest.WriteByte(0x20); // ' '
					}
					else
					{
						dest.WriteByte(src[index]);
					}
				}
				return Encoding.UTF8.GetString(dest.ToArray());
			}
		}

		public string FirstLine;
		public string Method;
		public string Path;
		public string HTTPVersion;
		public string Schema;
		public List<string[]> HeaderPairs = new List<string[]>();
		public byte[] Body;

		private const byte CR = 0x0d;
		private const byte LF = 0x0a;

		private readonly byte[] CRLF = new byte[] { CR, LF };

		private string RecvLine()
		{
			using (MemoryStream buff = new MemoryStream())
			{
				for (; ; )
				{
					byte chr = this.Channel.Recv(1)[0];

					if (chr == CR)
						continue;

					if (chr == LF)
						break;

					if (512000 < buff.Length)
						throw new OverflowException();

					buff.WriteByte(chr);
				}
				return Encoding.ASCII.GetString(buff.ToArray());
			}
		}

		private void RecvHeader()
		{
			const int HEADERS_LEN_MAX = 612000;
			const int WEIGHT = 1000;

			int roughHeaderLength = 0;

			for (; ; )
			{
				string line = this.RecvLine();

				if (line == "")
					break;

				roughHeaderLength += line.Length + WEIGHT;

				if (HEADERS_LEN_MAX < roughHeaderLength)
					throw new OverflowException();

				if (line[0] <= ' ')
				{
					this.HeaderPairs[this.HeaderPairs.Count - 1][1] += " " + line.Trim();
				}
				else
				{
					int colon = line.IndexOf(':');

					this.HeaderPairs.Add(new string[]
					{
						line.Substring(0, colon).Trim(),
						line.Substring(colon + 1).Trim(),
					});
				}
			}
		}

		public int ContentLength = 0;
		public bool Chunked = false;
		public string ContentType = null;
		public bool Expect100Continue = false;

		private void CheckHeader()
		{
			foreach (string[] pair in this.HeaderPairs)
			{
				string key = pair[0];
				string value = pair[1];

				if (SCommon.EqualsIgnoreCase(key, "Content-Length"))
				{
					this.ContentLength = int.Parse(value);
				}
				else if (SCommon.EqualsIgnoreCase(key, "Transfer-Encoding") && SCommon.EqualsIgnoreCase(value, "chunked"))
				{
					this.Chunked = true;
				}
				else if (SCommon.EqualsIgnoreCase(key, "Content-Type"))
				{
					this.ContentType = value;
				}
				else if (SCommon.EqualsIgnoreCase(key, "Expect") && SCommon.EqualsIgnoreCase(value, "100-continue"))
				{
					this.Expect100Continue = true;
				}
			}
		}

		private void RecvBody()
		{
			const int READ_SIZE_MAX = 3000000; // 3 MB

			using (HTTPBodyOutputStream buff = new HTTPBodyOutputStream())
			{
				if (this.Chunked)
				{
					for (; ; )
					{
						string line = this.RecvLine();

						// chunk-extension の削除
						{
							int i = line.IndexOf(';');

							if (i != -1)
								line = line.Substring(0, i);
						}

						int size = Convert.ToInt32(line.Trim(), 16);

						if (size == 0)
							break;

						if (size < 0)
							throw new Exception("不正なチャンクサイズです。" + size);

						if (BodySizeMax - buff.Count < size)
							throw new Exception("ボディサイズが大きすぎます。" + buff.Count + " + " + size);

						int chunkEnd = buff.Count + size;

						while (buff.Count < chunkEnd)
							buff.Write(this.Channel.Recv(Math.Min(READ_SIZE_MAX, chunkEnd - buff.Count)));

						this.Channel.Recv(2); // CR-LF
					}
					while (this.RecvLine() != "") // RFC 7230 4.1.2 Chunked Trailer Part
					{ }
				}
				else
				{
					if (this.ContentLength < 0)
						throw new Exception("不正なボディサイズです。" + this.ContentLength);

					if (BodySizeMax < this.ContentLength)
						throw new Exception("ボディサイズが大きすぎます。" + this.ContentLength);

					while (buff.Count < this.ContentLength)
						buff.Write(this.Channel.Recv(Math.Min(READ_SIZE_MAX, this.ContentLength - buff.Count)));
				}
				this.Body = buff.ToByteArray();
			}
		}

		// HTTPConnected 内で(必要に応じて)設定しなければならないフィールド -->

		public int ResStatus = 200;
		public string ResContentType = null;
		public List<string[]> ResHeaderPairs = new List<string[]>();
		public IEnumerable<byte[]> ResBody = null;

		// <-- HTTPConnected 内で(必要に応じて)設定しなければならないフィールド

		public void SendResponse()
		{
			this.Body = null;
			this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(ResponseTimeoutMillis);

			this.SendLine("HTTP/1.1 " + this.ResStatus + " Heartland");
			this.SendLine("Server: Heartland");

			if (this.ResContentType != null)
				this.SendLine("Content-Type: " + this.ResContentType);

			foreach (string[] pair in this.ResHeaderPairs)
				this.SendLine(pair[0] + ": " + pair[1]);

			if (this.ResBody == null)
			{
				this.EndHeader();
			}
			else
			{
				using (IEnumerator<byte[]> resBodyIterator = this.ResBody.GetEnumerator())
				{
					if (resBodyIterator.MoveNext())
					{
						byte[] first = resBodyIterator.Current;

						if (resBodyIterator.MoveNext())
						{
							this.SendLine("Transfer-Encoding: Chunked");
							this.EndHeader();
							SendChunk(first);

							do
							{
								SendChunk(resBodyIterator.Current);
							}
							while (resBodyIterator.MoveNext());

							this.SendLine("0");
							this.Channel.Send(CRLF);
						}
						else
						{
							this.SendLine("Content-Length: " + first.Length);
							this.EndHeader();
							this.Channel.Send(first);
						}
					}
					else
					{
						this.SendLine("Content-Length: 0");
						this.EndHeader();
					}
				}
			}
		}

		private void EndHeader()
		{
			this.SendLine("Connection: close");
			this.Channel.Send(CRLF);
		}

		private void SendChunk(byte[] chunk)
		{
			if (1 <= chunk.Length)
			{
				this.SendLine(chunk.Length.ToString("x"));
				this.Channel.Send(chunk);
				this.Channel.Send(CRLF);
			}
		}

		private void SendLine(string line)
		{
			this.Channel.Send(Encoding.ASCII.GetBytes(line));
			this.Channel.Send(CRLF);
		}
	}
}
