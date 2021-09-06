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
				Main4();
			}
			//Common.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
			// -- choose one --

			Main4();
			//new Test0001().Test01();
			//new Test0002().Test01();
			//new Test0003().Test01();

			// --

			//Common.Pause();
		}

		private void Main4()
		{
			File.WriteAllBytes(Consts.LOG_FILE, SCommon.EMPTY_BYTES); // ログファイルの内容クリア

			ProcMain.WriteLog = message =>
			{
				string line = "[" + DateTime.Now + "] " + message;

				Console.WriteLine(line);
				File.AppendAllLines(Consts.LOG_FILE, new string[] { line }, SCommon.ENCODING_SJIS);
			};

			ProcMain.WriteLog("BACKUP_ST");

			ProcMain.WriteLog("コピー元：" + Consts.SRC_DIR);
			ProcMain.WriteLog("コピー先：" + Consts.DEST_DIR);

			if (!Directory.Exists(Consts.SRC_DIR))
				throw new Exception("コピー元が存在しません。");

			if (!Directory.Exists(Consts.DEST_DIR))
				throw new Exception("コピー先が存在しません。");

			string[] rDirs = Directory.GetDirectories(Consts.SRC_DIR);
			string[] wDirs = Directory.GetDirectories(Consts.DEST_DIR);

			Array.Sort(rDirs, SCommon.Comp);
			Array.Sort(wDirs, SCommon.Comp);

			foreach (string dir in rDirs)
				ProcMain.WriteLog("1< " + dir);

			foreach (string dir in wDirs)
				ProcMain.WriteLog("1> " + dir);

			string[] rNames = rDirs.Select(dir => SCommon.ChangeRoot(dir, Consts.SRC_DIR)).ToArray();
			string[] wNames = wDirs.Select(dir => SCommon.ChangeRoot(dir, Consts.DEST_DIR)).ToArray();

			rNames = rNames.Where(name => !Consts.SRC_IGNORE_NAMES.Any(ignoreName => SCommon.EqualsIgnoreCase(ignoreName, name))).ToArray();
			wNames = wNames.Where(name => !Consts.DEST_IGNORE_NAMES.Any(ignoreName => SCommon.EqualsIgnoreCase(ignoreName, name))).ToArray();

			List<string> rOnlyNames = new List<string>();
			List<string> wOnlyNames = new List<string>();
			List<string> beNames = new List<string>();

			Common.Merge(rNames, wNames, SCommon.Comp, rOnlyNames, wOnlyNames, beNames);

			foreach (string name in rOnlyNames)
				ProcMain.WriteLog("2< " + name);

			foreach (string name in beNames)
				ProcMain.WriteLog("BE " + name);

			foreach (string name in wOnlyNames)
				ProcMain.WriteLog("2> " + name);

			if (MessageBox.Show(
				"メーラーなど、動作中のアプリを閉じて下さい。",
				"バックアップを開始します...",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Information
				) != DialogResult.OK
				)
			{
				ProcMain.WriteLog("BACKUP_CANCELLED");
				return;
			}

			foreach (string name in wOnlyNames)
			{
				string dir = Path.Combine(Consts.DEST_DIR, name);

				ProcMain.WriteLog("RD " + dir);

				Batch(string.Format(@"RD /S /Q ""{0}""", dir));
			}
			foreach (string name in rOnlyNames)
			{
				string dir = Path.Combine(Consts.DEST_DIR, name);

				ProcMain.WriteLog("MD " + dir);

				SCommon.CreateDir(dir);
			}
			foreach (string name in beNames.Concat(rOnlyNames))
			{
				string rDir = Path.Combine(Consts.SRC_DIR, name);
				string wDir = Path.Combine(Consts.DEST_DIR, name);

				ProcMain.WriteLog("< " + rDir);
				ProcMain.WriteLog("> " + wDir);

				ProcMain.WriteLog("ROBOCOPY_ST " + name);

				Batch(string.Format(@"ROBOCOPY.EXE ""{0}"" ""{1}"" /MIR", rDir, wDir));

				ProcMain.WriteLog("ROBOCOPY_ED " + name);
			}

			// ログファイルのフォルダを開く。
			{
				string dir = Path.GetDirectoryName(Consts.LOG_FILE);

				SCommon.Batch(new string[] { string.Format(@"START ""{0}""", dir) });
			}

			ProcMain.WriteLog("BACKUP_ED");
		}

		private void Batch(string command)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string outFile = wd.MakePath();
				string errFile = wd.MakePath();

				SCommon.Batch(new string[]
				{
					string.Format(@"{0} > ""{1}"" 2> ""{2}""", command, outFile, errFile),
				});

				using (FileStream writer = new FileStream(Consts.LOG_FILE, FileMode.Append, FileAccess.Write))
				{
					using (FileStream reader = new FileStream(outFile, FileMode.Open, FileAccess.Read))
					{
						SCommon.ReadToEnd(reader.Read, writer.Write);
					}
					using (FileStream reader = new FileStream(errFile, FileMode.Open, FileAccess.Read))
					{
						SCommon.ReadToEnd(reader.Read, writer.Write);
					}
				}
			}
		}
	}
}
