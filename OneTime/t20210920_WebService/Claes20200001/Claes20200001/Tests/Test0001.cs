using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.WebServices;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void Test01()
		{
			Test01a("https://www.google.com");
			Test01a("https://www.youtube.com");
			Test01a("https://www.amazon.co.jp");
		}

		private void Test01a(string url)
		{
			const string RES_FILE = @"C:\temp\Test01a.dat";

			HTTPClient hc = new HTTPClient(url, RES_FILE);
			hc.Get();
			string contentType = hc.ResHeaders["Content-Type"];
			string[] charsetParts = contentType == null ? null : Common.ParseIsland(contentType, "charset=", true);
			string charset = charsetParts == null ? "none" : charsetParts[2].Trim();
			Console.WriteLine(charset); // cout
			Encoding encoding;

			// charset -> encoding
			// 他の文字セットがあれば追加すること。
			if (SCommon.EqualsIgnoreCase(charset, "Shift_JIS"))
				encoding = SCommon.ENCODING_SJIS;
			else if (SCommon.EqualsIgnoreCase(charset, "ISO-8859-1"))
				encoding = Encoding.GetEncoding(28591);
			else
				encoding = Encoding.UTF8;

			Console.WriteLine(encoding); // cout
			string resBodyText = encoding.GetString(File.ReadAllBytes(RES_FILE));
			//Console.WriteLine(resBodyText); // cout
			File.WriteAllText(Common.NextOutputPath() + ".txt", resBodyText, Encoding.UTF8);

			SCommon.DeletePath(RES_FILE);
		}

		public void Test02()
		{
			new HTTPServer()
			{
				HTTPConnected = channel =>
				{
					//channel.ResStatus = 200;
					channel.ResContentType = "text/plain; charset=US-ASCII";
					//channel.ResHeaderPairs.Add(new string[] { "X-Key-01", "Value-01" });
					//channel.ResHeaderPairs.Add(new string[] { "X-Key-02", "Value-02" });
					//channel.ResHeaderPairs.Add(new string[] { "X-Key-03", "Value-03" });
					channel.ResBody = "Hello, Happy World!".ToCharArray().Select(chr => Encoding.ASCII.GetBytes("" + chr));
				},
				//PortNo = 80,
				//Backlog = 100,
				//ConnectMax = 30,
				//Interlude = () => !Console.KeyAvailable,
			}
			.Perform();
		}

		public void Test03()
		{
			new HTTPServer()
			{
				HTTPConnected = channel =>
				{
					channel.ResContentType = "text/plain; charset=US-ASCII";
					channel.ResHeaderPairs.Add(new string[] { "Content-Disposition", "attachment" }); // ダウンロードさせる。
					channel.ResBody = Test03_E_ResBody();
				},
			}
			.Perform();
		}

		/// <summary>
		/// カウンタを2MB毎に連結して(塊にして)列挙する。
		/// </summary>
		/// <returns>カウンタの塊の列挙</returns>
		private IEnumerable<byte[]> Test03_E_ResBody()
		{
			List<byte[]> parts = new List<byte[]>();
			int size = 0;

			foreach (byte[] part in Test03_E_Counter())
			{
				parts.Add(part);
				size += part.Length;

				if (2000000 < size)
				{
					yield return SCommon.Join(parts);
					parts.Clear();
					size = 0;
				}
			}
			yield return SCommon.Join(parts);
		}

		/// <summary>
		/// カウンタ
		/// 1～1億
		/// およそ1GB弱になる。
		/// </summary>
		/// <returns>カウンタ</returns>
		private IEnumerable<byte[]> Test03_E_Counter()
		{
			for (int count = 1; count <= 100000000; count++)
			{
				yield return Encoding.ASCII.GetBytes(count + "\r\n");
			}
		}
	}
}
