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
				string rDir = Path.Combine(Consts.SRC_DIR, name);
				string wDir = Path.Combine(Consts.DEST_DIR, name);

				using (WorkingDir wd = new WorkingDir())
				{
					SCommon.Batch(
						new string[]
						{
							string.Format(@"ROBOCOPY.EXE ""{0}"" ""{1}"" /MIR > out", rDir, wDir),
						},
						wd.GetPath(".")
						);
				}
			}
		}
	}
}
