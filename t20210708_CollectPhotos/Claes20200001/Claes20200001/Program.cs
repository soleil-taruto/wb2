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

			Main4(ProcMain.ArgsReader);
			//new Test0001().Test01();
			//new Test0002().Test01();
			//new Test0003().Test01();

			// --

			//Common.Pause();
		}

		private bool ForceCheckPhotoSizeFlag = false;

		private void Main4(ArgsReader ar)
		{
			ForceCheckPhotoSizeFlag = ar.ArgIs("/F");

			string dir = ar.NextArg();
			dir = SCommon.MakeFullPath(dir);
			Console.WriteLine("dir: " + dir); // cout

			if (!Directory.Exists(dir))
				throw new Exception("指定されたフォルダは存在しません！");

			string[] files = Directory.GetFiles(dir);

			foreach (string file in files)
				ProcPhotoFile(file);
		}

		private void ProcPhotoFile(string file)
		{
			if (!SCommon.EndsWithIgnoreCase(file, ".jpg")) // ? 対象外
				return;

			string localName = Path.GetFileNameWithoutExtension(file);
			bool renamedFlag = false;

			if (!Regex.IsMatch(localName, "^[0-9]{4}-[0-9]{2}-[0-9]{2}_[0-9]{2}-[0-9]{2}-[0-9]{2}_[0-9]{3}$"))
			{
				FileInfo fileInfo = new FileInfo(file);
				SCommon.SimpleDateTime fileTime = new SCommon.SimpleDateTime(fileInfo.LastWriteTime);
				string fileNew;

				for (int count = 0; ; count++)
				{
					string localFileNew = string.Format(
						"{0:D4}-{1:D2}-{2:D2}_{3:D2}-{4:D2}-{5:D2}_{6:D3}.jpg",
						fileTime.Year,
						fileTime.Month,
						fileTime.Day,
						fileTime.Hour,
						fileTime.Minute,
						fileTime.Second,
						count
						);

					fileNew = Path.Combine(Path.GetDirectoryName(file), localFileNew);

					if (!File.Exists(fileNew))
						break;
				}

				Console.WriteLine("< " + file); // cout
				Console.WriteLine("> " + fileNew); // cout

				File.Move(file, fileNew);

				file = fileNew;
				renamedFlag = true;
			}

			if (renamedFlag || ForceCheckPhotoSizeFlag)
			{
				Console.WriteLine("# " + file); // cout

				Canvas canvas = Canvas.Load(file);

				// 幅・高さ共にターゲットサイズより大きい -> 少なくとも何方かはターゲットサイズ以下にする。
				if (
					Consts.TARGET_PHOTO_W < canvas.W &&
					Consts.TARGET_PHOTO_H < canvas.H
					)
				{
					D4Rect interior;
					D4Rect exterior;

					Common.AdjustRect(new D2Size(canvas.W, canvas.H), new D4Rect(0, 0, Consts.TARGET_PHOTO_W, Consts.TARGET_PHOTO_H), out interior, out exterior);

					int dest_w = SCommon.ToInt(exterior.W);
					int dest_h = SCommon.ToInt(exterior.H);

					Console.WriteLine("< " + canvas.W + " x " + canvas.H); // cout
					Console.WriteLine("> " + dest_w + " x " + dest_h); // cout

					canvas = canvas.Expand(dest_w, dest_h);
					canvas.SaveAsJpeg(file, 90);
				}
			}

			GC.Collect();
		}
	}
}
