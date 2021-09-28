using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte
{
	public static class Common
	{
		public static void Pause()
		{
			Console.WriteLine("Press ENTER key.");
			Console.ReadLine();
		}

		#region GetOutputDir

		private static string GOD_Dir;

		public static string GetOutputDir()
		{
			if (GOD_Dir == null)
				GOD_Dir = GetOutputDir_Main();

			return GOD_Dir;
		}

		private static string GetOutputDir_Main()
		{
			for (int c = 1; c <= 999; c++)
			{
				string dir = "C:\\" + c;

				if (
					!Directory.Exists(dir) &&
					!File.Exists(dir)
					)
				{
					SCommon.CreateDir(dir);
					//SCommon.Batch(new string[] { "START " + dir });
					return dir;
				}
			}
			throw new Exception("C:\\1 ～ 999 は使用できません。");
		}

		public static void OpenOutputDir()
		{
			SCommon.Batch(new string[] { "START " + GetOutputDir() });
		}

		public static void OpenOutputDirIfCreated()
		{
			if (GOD_Dir != null)
			{
				OpenOutputDir();
			}
		}

		private static int NOP_Count = 0;

		public static string NextOutputPath()
		{
			return Path.Combine(GetOutputDir(), (++NOP_Count).ToString("D4"));
		}

		#endregion

		public static string EraseStartEnd(string str, string startPtn, string endPtn)
		{
			if (!str.StartsWith(startPtn))
				return null;

			str = str.Substring(startPtn.Length);

			if (!str.EndsWith(endPtn))
				return null;

			str = str.Substring(0, str.Length - endPtn.Length);
			return str;
		}

		/// <summary>
		/// パス文字列を比較する。
		/// 同じフォルダ内のローカル名が辞書順になるようにする。
		/// </summary>
		/// <param name="a">パス文字列_A</param>
		/// <param name="b">パス文字列_B</param>
		/// <returns>比較結果</returns>
		public static int CompPath(string a, string b)
		{
			a = a.Replace('\\', '\t');
			b = b.Replace('\\', '\t');

			return SCommon.CompIgnoreCase(a, b);
		}
	}
}
