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
			if (!Directory.Exists(Consts.SRC_DIR))
				throw new Exception("バックアップのコピー元が存在しません。");

			if (!Directory.Exists(Consts.DEST_DIR))
				throw new Exception("バックアップのコピー先が存在しません。");

			string[] rDirs = Directory.GetDirectories(Consts.SRC_DIR);
			string[] wDirs = Directory.GetDirectories(Consts.DEST_DIR);

			string[] rNames = rDirs.Select(dir => SCommon.ChangeRoot(dir, Consts.SRC_DIR)).ToArray();
			string[] wNames = wDirs.Select(dir => SCommon.ChangeRoot(dir, Consts.DEST_DIR)).ToArray();

			List<string> rOnlyNames = new List<string>();
			List<string> wOnlyNames = new List<string>();
			List<string> beNames = new List<string>();

			Common.Merge(rNames, wNames, SCommon.Comp, rOnlyNames, wOnlyNames, beNames);

			foreach (string name in wOnlyNames)
			{
				string dir = Path.Combine(Consts.DEST_DIR, name);

				SCommon.DeletePath(dir);
			}
			foreach (string name in rOnlyNames)
			{
				string dir = Path.Combine(Consts.DEST_DIR, name);

				SCommon.CreateDir(dir);
			}
			foreach (string name in beNames)
			{
				string dir1 = Path.Combine(Consts.SRC_DIR, name);
				string dir2 = Path.Combine(Consts.DEST_DIR, name);

				差分CopyDir(dir1, dir2);
			}
		}

		private void 差分CopyDir(string rDir, string wDir)
		{
			string[] rSubDirs = Directory.GetDirectories(rDir);
			string[] wSubDirs = Directory.GetDirectories(wDir);
			string[] rFiles = Directory.GetFiles(rDir);
			string[] wFiles = Directory.GetFiles(wDir);

			string[] rSubDirNames = rSubDirs.Select(dir => SCommon.ChangeRoot(dir, rDir)).ToArray();
			string[] wSubDirNames = wSubDirs.Select(dir => SCommon.ChangeRoot(dir, wDir)).ToArray();
			string[] rFileNames = rFiles.Select(file => SCommon.ChangeRoot(file, rDir)).ToArray();
			string[] wFileNames = wFiles.Select(file => SCommon.ChangeRoot(file, wDir)).ToArray();

			List<string> rOnlySubDirNames = new List<string>();
			List<string> wOnlySubDirNames = new List<string>();
			List<string> beSubDirNames = new List<string>();
			List<string> rOnlyFileNames = new List<string>();
			List<string> wOnlyFileNames = new List<string>();
			List<string> beFileNames = new List<string>();

			Common.Merge(rSubDirNames, wSubDirNames, SCommon.Comp, rOnlySubDirNames, wOnlySubDirNames, beSubDirNames);
			Common.Merge(rFileNames, wFileNames, SCommon.Comp, rOnlyFileNames, wOnlyFileNames, beFileNames);

			foreach (string name in wOnlySubDirNames)
			{
				string dir = Path.Combine(wDir, name);

				SCommon.DeletePath(dir);
			}
			foreach (string name in wOnlyFileNames)
			{
				string file = Path.Combine(wDir, name);

				SCommon.DeletePath(file);
			}
			foreach (string name in rOnlySubDirNames)
			{
				string dir1 = Path.Combine(rDir, name);
				string dir2 = Path.Combine(wDir, name);

				SCommon.CreateDir(dir2);

				差分CopyDir(dir1, dir2);
			}
			foreach (string name in rOnlyFileNames)
			{
				string file1 = Path.Combine(rDir, name);
				string file2 = Path.Combine(wDir, name);

				DoCopyFile(file1, file2);
			}
			foreach (string name in beSubDirNames)
			{
				string dir1 = Path.Combine(rDir, name);
				string dir2 = Path.Combine(wDir, name);

				差分CopyDir(dir1, dir2);
			}
			foreach (string name in beFileNames)
			{
				string file1 = Path.Combine(rDir, name);
				string file2 = Path.Combine(wDir, name);

				差分CopyFile(file1, file2);
			}
		}

		private void 差分CopyFile(string rFile, string wFile)
		{
			FileInfo rInfo = new FileInfo(rFile);
			FileInfo wInfo = new FileInfo(wFile);

			if (
				rInfo.Length != wInfo.Length ||
				rInfo.CreationTime != wInfo.CreationTime ||
				rInfo.LastWriteTime != wInfo.LastWriteTime
				)
				DoCopyFile(rFile, wFile);
		}

		private void DoCopyFile(string rFile, string wFile)
		{
			using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
			using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
			{
				SCommon.ReadToEnd(reader.Read, writer.Write);
			}

			FileInfo rInfo = new FileInfo(rFile);
			FileInfo wInfo = new FileInfo(wFile);

			wInfo.CreationTime = rInfo.CreationTime;
			wInfo.LastWriteTime = rInfo.LastWriteTime;
		}
	}
}
