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

			Main4(new ArgsReader(new string[] { @"C:\temp" }));
			//new Test0001().Test01();
			//new Test0001().Test02();
			//new Test0001().Test03();

			// --

			Common.Pause();
		}

		private void Main4(ArgsReader ar)
		{
			using (EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.AutoReset, Consts.STOP_EVENT_NAME))
			{
				if (ar.HasArgs())
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

					HTTPServerChannel.RequestTimeoutMillis = 10000;
					//HTTPServerChannel.ResponseTimeoutMillis = -1;
					//HTTPServerChannel.FirstLineTimeoutMillis = 2000;
					HTTPServerChannel.IdleTimeoutMillis = 10000;
					HTTPServerChannel.BodySizeMax = 0;

					// 設定ここまで

					this.DocRoot = SCommon.MakeFullPath(ar.NextArg());

					if (!Directory.Exists(this.DocRoot))
						throw new Exception("ドキュメントルートが見つかりません");

					if (ar.HasArgs())
					{
						hs.PortNo = int.Parse(ar.NextArg());

						if (hs.PortNo < 1 || 65535 < hs.PortNo)
							throw new Exception("不正なポート番号");
					}
					hs.Perform();
				}
				else
				{
					evStop.Set();
				}
			}
		}

		private string DocRoot;

		private void P_Connected(HTTPServerChannel channel)
		{
			if (channel.Method != "GET")
				throw new Exception("対応していないメソッド：" + channel.Method);

			string urlPath = channel.PathQuery.Split('?')[0];
			string[] pTkns = urlPath.Split('/').Where(v => v != "").Select(v => Common.ToFairLocalPath(v, 0)).ToArray();
			string path = Path.Combine(new string[] { this.DocRoot }.Concat(pTkns).ToArray());

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "目的パス：" + path);

			if (urlPath.EndsWith("/"))
			{
				path = Path.Combine(path, "index.htm");

				if (!File.Exists(path))
					path += "l";
			}
			else if (Directory.Exists(path))
			{
				channel.ResStatus = 301;
				//channel.ResContentType = null;
				channel.ResHeaderPairs.Add(new string[] { "Location", "http://" + GetHeaderValue(channel, "Host") + "/" + string.Join("", pTkns.Select(v => EncodeUrl(v) + "/")) });
				//channel.ResBody = null;
				return;
			}
			if (File.Exists(path))
			{
				//channel.ResStatus = 200;
				channel.ResContentType = ContentTypeCollection.I.GetContentType(Path.GetExtension(path));
				//channel.ResHeaderPairs.Add();
				channel.ResBody = E_ReadFile(path);
			}
			else
			{
				channel.ResStatus = 404;
				//channel.ResContentType = null;
				//channel.ResHeaderPairs.Add();
				//channel.ResBody = null;
			}
		}

		private static string GetHeaderValue(HTTPServerChannel channel, string name)
		{
			foreach (string[] pair in channel.HeaderPairs)
				if (SCommon.EqualsIgnoreCase(pair[0], name))
					return pair[1];

			throw new Exception();
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
