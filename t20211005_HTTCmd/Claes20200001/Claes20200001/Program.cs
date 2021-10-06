using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
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
			HTTPServer hs = new HTTPServer()
			{
				HTTPConnected = P_Connected,
			};

			if (ar.HasArgs())
			{
				this.DocRoot = SCommon.MakeFullPath(ar.NextArg());

				if (!Directory.Exists(this.DocRoot))
					throw new Exception("ドキュメントルートが見つかりません");

				if (ar.HasArgs())
				{
					hs.PortNo = int.Parse(ar.NextArg());

					if (hs.PortNo < 1 || 65535 < hs.PortNo)
						throw new Exception("不正なポート番号");
				}
			}

			hs.Perform();
		}

		private string DocRoot = ".";

		private void P_Connected(HTTPServerChannel channel)
		{
			if (channel.Method != "GET")
				throw new Exception("Method Ignore: " + channel.Method);

			// TODO
			// TODO
			// TODO
		}
	}
}
