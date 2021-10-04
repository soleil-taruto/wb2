﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.WebServices
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

		public IEnumerable<int> RecvRequest()
		{
			this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(RequestTimeoutMillis);
			this.Channel.IdleTimeoutMillis = FirstLineTimeoutMillis;

			foreach (int relay in this.RecvLine(ret => this.FirstLine = ret))
				yield return relay;

			this.Channel.FirstLineRecved = true;

			{
				string[] tokens = this.FirstLine.Split(' ');

				this.Method = tokens[0];
				this.Path = DecodeURL(tokens[1]);
				this.HTTPVersion = tokens[2];
			}

			this.Channel.IdleTimeoutMillis = IdleTimeoutMillis;

			foreach (int relay in this.RecvHeader())
				yield return relay;

			this.CheckHeader();

			if (this.Expect100Continue)
			{
				foreach (int relay in this.SendLine("HTTP/1.1 100 Continue"))
					yield return relay;

				foreach (int relay in this.SendLine(""))
					yield return relay;
			}
			foreach (int relay in this.RecvBody())
				yield return relay;
		}

		private static DateTime? TimeoutMillisToDateTime(int millis)
		{
			if (millis == -1)
				return null;

			return DateTime.Now + TimeSpan.FromMilliseconds((double)millis);
		}

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

		private IEnumerable<int> RecvLine(Action<string> a_return)
		{
			const int LINE_LEN_MAX = 512000;

			List<byte> buff = new List<byte>(LINE_LEN_MAX);

			for (; ; )
			{
				byte[] chrs = null;

				foreach (int relay in this.Channel.Recv(1, ret => chrs = ret))
					yield return relay;

				byte chr = chrs[0];

				if (chr == CR)
					continue;

				if (chr == LF)
					break;

				if (LINE_LEN_MAX < buff.Count)
					throw new OverflowException();

				buff.Add(chr);
			}
			a_return(Encoding.ASCII.GetString(buff.ToArray()));
		}

		private IEnumerable<int> RecvHeader()
		{
			const int HEADERS_LEN_MAX = 612000;
			const int WEIGHT = 1000;

			int roughHeaderLength = 0;

			for (; ; )
			{
				string line = null;

				foreach (int relay in this.RecvLine(ret => line = ret))
					yield return relay;

				if (line == null)
					throw null; // never

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

		private IEnumerable<int> RecvBody()
		{
			const int READ_SIZE_MAX = 2000000; // 2 MB

			HTTPBodyOutputStream buff = this.Channel.BodyOutputStream;

			if (this.Chunked)
			{
				for (; ; )
				{
					string line = null;

					foreach (int relay in this.RecvLine(ret => line = ret))
						yield return relay;

					if (line == null)
						throw null; // never

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
					{
						byte[] data = null;

						foreach (int relay in this.Channel.Recv(Math.Min(READ_SIZE_MAX, chunkEnd - buff.Count), ret => data = ret))
							yield return relay;

						if (data == null)
							throw null; // never

						buff.Write(data);
					}
					foreach (int relay in this.Channel.Recv(2, ret => { })) // CR-LF
						yield return relay;
				}

				for (; ; ) // RFC 7230 4.1.2 Chunked Trailer Part
				{
					string line = null;

					foreach (int relay in this.RecvLine(ret => { }))
						yield return relay;

					if (line == null)
						throw null; // never

					if (line == "")
						break;
				}
			}
			else
			{
				if (this.ContentLength < 0)
					throw new Exception("不正なボディサイズです。" + this.ContentLength);

				if (BodySizeMax < this.ContentLength)
					throw new Exception("ボディサイズが大きすぎます。" + this.ContentLength);

				while (buff.Count < this.ContentLength)
				{
					byte[] data = null;

					foreach (int relay in this.Channel.Recv(Math.Min(READ_SIZE_MAX, this.ContentLength - buff.Count), ret => data = ret))
						yield return relay;

					if (data == null)
						throw null; // never

					buff.Write(data);
				}
			}
			this.Body = buff.ToByteArray();
		}

		// HTTPConnected 内で(必要に応じて)設定しなければならないフィールド -->

		public int ResStatus = 200;
		public string ResContentType = null;
		public List<string[]> ResHeaderPairs = new List<string[]>();
		public IEnumerable<byte[]> ResBody = null;

		// <-- HTTPConnected 内で(必要に応じて)設定しなければならないフィールド

		public IEnumerable<int> SendResponse()
		{
			this.Body = null;
			this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(ResponseTimeoutMillis);

			foreach (int relay in this.SendLine("HTTP/1.1 " + this.ResStatus + " Heartland"))
				yield return relay;

			foreach (int relay in this.SendLine("Server: Heartland"))
				yield return relay;

			if (this.ResContentType != null)
				foreach (int relay in this.SendLine("Content-Type: " + this.ResContentType))
					yield return relay;

			foreach (string[] pair in this.ResHeaderPairs)
				foreach (int relay in this.SendLine(pair[0] + ": " + pair[1]))
					yield return relay;

			if (this.ResBody == null)
			{
				foreach (int relay in this.EndHeader())
					yield return relay;
			}
			else
			{
				IEnumerator<byte[]> resBodyIterator = this.ResBody.GetEnumerator();

				if (SockCommon.NB("chu1", () => resBodyIterator.MoveNext()))
				{
					byte[] first = resBodyIterator.Current;

					if (SockCommon.NB("chu2", () => resBodyIterator.MoveNext()))
					{
						foreach (int relay in this.SendLine("Transfer-Encoding: chunked"))
							yield return relay;

						foreach (int relay in this.EndHeader())
							yield return relay;

						foreach (int relay in this.SendChunk(first))
							yield return relay;

						do
						{
							foreach (int relay in this.SendChunk(resBodyIterator.Current))
								yield return relay;
						}
						while (SockCommon.NB("chux", () => resBodyIterator.MoveNext()));

						foreach (int relay in this.SendLine("0"))
							yield return relay;

						foreach (int relay in this.Channel.Send(CRLF))
							yield return relay;
					}
					else
					{
						foreach (int relay in this.SendLine("Content-Length: " + first.Length))
							yield return relay;

						foreach (int relay in this.EndHeader())
							yield return relay;

						foreach (int relay in this.Channel.Send(first))
							yield return relay;
					}
				}
				else
				{
					foreach (int relay in this.SendLine("Content-Length: 0"))
						yield return relay;

					foreach (int relay in this.EndHeader())
						yield return relay;
				}
			}
		}

		private IEnumerable<int> EndHeader()
		{
			foreach (int relay in this.SendLine("Connection: close"))
				yield return relay;

			foreach (int relay in this.Channel.Send(CRLF))
				yield return relay;
		}

		private IEnumerable<int> SendChunk(byte[] chunk)
		{
			if (1 <= chunk.Length)
			{
				foreach (int relay in this.SendLine(chunk.Length.ToString("x")))
					yield return relay;

				foreach (int relay in this.Channel.Send(chunk))
					yield return relay;

				foreach (int relay in this.Channel.Send(CRLF))
					yield return relay;
			}
		}

		private IEnumerable<int> SendLine(string line)
		{
			foreach (int relay in this.Channel.Send(Encoding.ASCII.GetBytes(line)))
				yield return relay;

			foreach (int relay in this.Channel.Send(CRLF))
				yield return relay;
		}
	}
}
