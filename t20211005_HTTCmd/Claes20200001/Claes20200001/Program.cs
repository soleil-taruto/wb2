﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using Charlotte.Commons;
using Charlotte.Tests;
using Charlotte.WebServices;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4(ar);
			}
			Common.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
			// -- choose one --

			//Main4(new ArgsReader(new string[] { }));
			//Main4(new ArgsReader(new string[] { @"C:\temp" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/T", @"C:\temp\1.tsv" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/T", @"C:\temp\1.tsv", "/K" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K", "/T", @"C:\temp\1.tsv" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K", "/T", @"C:\temp\1.tsv" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K", "/T", @"C:\temp\1.tsv", "/H", @"C:\temp\2.tsv" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K", "/H", @"C:\temp\2.tsv" }));
			//Main4(new ArgsReader(new string[] { @"C:\temp", "8080", "/K", "/T", @"C:\temp\1.tsv", "/H", @"C:\temp\2.tsv" }));
			Main4(new ArgsReader(new string[] { @"C:\temp", "80", "/K", "/L", @"C:\temp\1.log" }));
			//new Test0001().Test01();
			//new Test0001().Test02();
			//new Test0001().Test03();

			// --

			Common.Pause();
		}

		private void Main4(ArgsReader ar)
		{
			ProcMain.WriteLog = message =>
			{
				string line = "[" + DateTime.Now + "] " + message;

				Console.WriteLine(line);

				if (this.DebugLogFile != null)
				{
					using (StreamWriter writer = new StreamWriter(DebugLogFile, true, Encoding.UTF8))
					{
						writer.WriteLine(line);
					}
				}
			};

			try
			{
				Main5(ar);
			}
			catch (Exception e)
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, e);
			}

			// 実行ファイルのダブルクリックやドキュメントルート(フォルダ)のドラッグアンドドロップで起動して
			// エラーになった場合、一瞬でコンソールが閉じてしまうので、少しだけ待つ。
			Thread.Sleep(500);
		}

		private void Main5(ArgsReader ar)
		{
			// 複数のサーバーを起動していた場合、全て停止出来るようにマニュアル・リセットとする。
			using (EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.ManualReset, Consts.SERVER_STOP_EVENT_NAME))
			{
				HTTPServer hs = new HTTPServer()
				{
					//PortNo = 80,
					//Backlog = 300,
					//ConnectMax = 100,
					Interlude = () => !evStop.WaitOne(0),
					HTTPConnected = P_Connected,
				};

				//SockChannel.ThreadTimeoutMillis = 100;

				//HTTPServer.KeepAliveTimeoutMillis = 5000;

				HTTPServerChannel.RequestTimeoutMillis = 10000; // 10 sec
				//HTTPServerChannel.ResponseTimeoutMillis = -1;
				//HTTPServerChannel.FirstLineTimeoutMillis = 2000;
				HTTPServerChannel.IdleTimeoutMillis = 600000; // 10 min
				HTTPServerChannel.BodySizeMax = 0;

				// サーバーの設定ここまで

				if (ar.ArgIs("/S"))
				{
					evStop.Set();
					return;
				}
				if (ar.HasArgs())
				{
					this.DocRoot = SCommon.ToFullPath(ar.NextArg());

					if (!Directory.Exists(this.DocRoot))
						throw new Exception("ドキュメントルートが見つかりません");

					if (ar.HasArgs())
					{
						hs.PortNo = int.Parse(ar.NextArg());

						if (hs.PortNo < 1 || 65535 < hs.PortNo)
							throw new Exception("不正なポート番号");

						for (; ; ) // 拡張オプション
						{
							if (ar.ArgIs("/K"))
							{
								Func<bool> baseEvent = hs.Interlude;
								hs.Interlude = () => baseEvent() && !Console.KeyAvailable;
								ProcMain.WriteLog("キー入力を検出するとサーバーは停止します。");
								continue;
							}
							if (ar.ArgIs("/T"))
							{
								ContentTypeCollection.I.AddContentTypesByTsvFile(ar.NextArg());
								continue;
							}
							if (ar.ArgIs("/H"))
							{
								LoadHost2DocRoot(ar.NextArg());
								continue;
							}
							if (ar.ArgIs("/L"))
							{
								DebugLogFile = SCommon.ToFullPath(ar.NextArg());
								ProcMain.WriteLog("DebugLogFile: " + DebugLogFile);
								continue;
							}
							break;
						}
					}
				}
				else
				{
					this.DocRoot = Directory.GetCurrentDirectory();
				}

				ProcMain.WriteLog("HTTCmd-Start");
				ProcMain.WriteLog("DocRoot: " + this.DocRoot);
				ProcMain.WriteLog("PortNo: " + hs.PortNo);

				hs.Perform();

				ProcMain.WriteLog("HTTCmd-End");
			}
		}

		private void LoadHost2DocRoot(string tsvFile)
		{
			this.Host2DocRoot = SCommon.CreateDictionaryIgnoreCase<string>();

			using (CsvFileReader reader = new CsvFileReader(tsvFile, Encoding.UTF8, CsvFileReader.DELIMITER_TAB))
			{
				foreach (string[] row in reader.ReadToEnd())
				{
					if (row.Length != 2)
						continue;

					string host = row[0];
					string docRoot = SCommon.ToFullPath(row[1]);

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, string.Format("Add Host-DocRoot Pair: {0} = {1}", host, docRoot));

					if (!Directory.Exists(docRoot))
						throw new Exception("ドキュメントルートが見つかりません(Host2DocRoot)");

					this.Host2DocRoot.Add(host, docRoot);
				}
			}
		}

		private string DocRoot;
		private Dictionary<string, string> Host2DocRoot = null;
		private string DebugLogFile = null;

		private void P_Connected(HTTPServerChannel channel)
		{
			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "クライアント：" + channel.Channel.Handler.RemoteEndPoint);

			if (10 < channel.Method.Length) // rough limit
				throw new Exception("Received method is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "要求メソッド：" + channel.Method);

			bool head;
			if (channel.Method == "GET")
				head = false;
			else if (channel.Method == "HEAD")
				head = true;
			else
				throw new Exception("Unsupported method: " + channel.Method);

			string docRoot = this.DocRoot;
			string host = GetHeaderValue(channel, "Host");
			if (host != null && this.Host2DocRoot != null)
			{
				string hostName = host;

				// ポート番号除去
				{
					int colon = hostName.IndexOf(':');

					if (colon != -1)
						hostName = hostName.Substring(0, colon);
				}

				if (this.Host2DocRoot.ContainsKey(hostName))
					docRoot = this.Host2DocRoot[hostName];
			}

			string urlPath = channel.PathQuery;

			// クエリ除去
			{
				int ques = urlPath.IndexOf('?');

				if (ques != -1)
					urlPath = urlPath.Substring(0, ques);
			}

			if (1000 < urlPath.Length) // rough limit
				throw new Exception("Received path is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "要求パス：" + urlPath);

			string[] pTkns = urlPath.Split('/').Where(v => v != "").Select(v => Common.ToFairLocalPath(v, 0)).ToArray();
			string path = Path.Combine(new string[] { docRoot }.Concat(pTkns).ToArray());

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "目的パス：" + path);

			if (urlPath.EndsWith("/"))
			{
				path = Path.Combine(path, "index.htm");

				if (!File.Exists(path))
					path += "l";
			}
			else if (Directory.Exists(path))
			{
				if (host == null)
					throw new Exception("No HOST header value");

				channel.ResStatus = 301;
				channel.ResHeaderPairs.Add(new string[] { "Location", "http://" + host + "/" + string.Join("", pTkns.Select(v => EncodeUrl(v) + "/")) });
				//channel.ResBody = null;

				goto endFunc;
			}
			if (File.Exists(path))
			{
				//channel.ResStatus = 200;
				channel.ResHeaderPairs.Add(new string[] { "Content-Type", ContentTypeCollection.I.GetContentType(Path.GetExtension(path)) });
				channel.ResBody = E_ReadFile(path);
			}
			else
			{
				channel.ResStatus = 404;
				//channel.ResHeaderPairs.Add();
				//channel.ResBody = null;
			}

		endFunc:
			channel.ResHeaderPairs.Add(new string[] { "Server", "HTTCmd" });

			if (head && channel.ResBody != null)
			{
				FileInfo fileInfo = new FileInfo(path);

				channel.ResHeaderPairs.Add(new string[] { "Content-Length", fileInfo.Length.ToString() });
				channel.ResHeaderPairs.Add(new string[] { "X-Last-Modified-Time", new SCommon.SimpleDateTime(fileInfo.LastWriteTime).ToString("{0}/{1:D2}/{2:D2} {4:D2}:{5:D2}:{6:D2}") });

				channel.ResBody = null;
			}

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-STATUS " + channel.ResStatus);

			foreach (string[] pair in channel.ResHeaderPairs)
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-HEADER " + pair[0] + " = " + pair[1]);

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-BODY " + (channel.ResBody != null));
		}

		private static string GetHeaderValue(HTTPServerChannel channel, string name)
		{
			foreach (string[] pair in channel.HeaderPairs)
				if (SCommon.EqualsIgnoreCase(pair[0], name))
					return pair[1];

			return null;
		}

		private static string EncodeUrl(string str)
		{
			StringBuilder buff = new StringBuilder();

			foreach (byte chr in Encoding.UTF8.GetBytes(str))
			{
				buff.Append('%');
				buff.Append(chr.ToString("x2"));
			}
			return buff.ToString();
		}

		private static IEnumerable<byte[]> E_ReadFile(string file)
		{
			long fileSize = new FileInfo(file).Length;

			for (long offset = 0L; offset < fileSize; )
			{
				int readSize = (int)Math.Min(fileSize - offset, 2000000L);
				byte[] buff = new byte[readSize];

				//SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "READ " + offset + " " + readSize + " " + fileSize + " " + (offset * 100.0 / fileSize).ToString("F2") + " " + ((offset + readSize) * 100.0 / fileSize).ToString("F2")); // 頻出するので抑止

				using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
				{
					reader.Seek(offset, SeekOrigin.Begin);
					reader.Read(buff, 0, readSize);
				}
				yield return buff;

				offset += (long)readSize;
			}
		}
	}
}
