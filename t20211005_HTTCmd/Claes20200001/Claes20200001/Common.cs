using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
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

		#region ToFairLocalPath

		/// <summary>
		/// Windowsのローカル名に使用出来ない予約名のリストを返す。
		/// 今では使用可能なものも含む。
		/// 元にしたコード：
		/// https://github.com/stackprobe/Factory/blob/master/Common/DataConv.c#L460-L491
		/// </summary>
		/// <returns>予約名リスト</returns>
		private static IEnumerable<string> GetReservedWordsForWindowsPath()
		{
			yield return "AUX";
			yield return "CON";
			yield return "NUL";
			yield return "PRN";

			for (int no = 1; no <= 9; no++)
			{
				yield return "COM" + no;
				yield return "LPT" + no;
			}

			// グレーゾーン
			{
				yield return "COM0";
				yield return "LPT0";
				yield return "CLOCK$";
				yield return "CONFIG$";
			}
		}

		/// <summary>
		/// 歴としたローカル名に変換する。
		/// 実際に使用可能なローカル名より基準が厳しい。
		/// 元にしたコード：
		/// https://github.com/stackprobe/Factory/blob/master/Common/DataConv.c#L503-L552
		/// </summary>
		/// <param name="str">対象文字列(対象パス)</param>
		/// <param name="dirSize">対象パスが存在するディレクトリのフルパスの長さ、考慮しない場合は 0 を指定すること。</param>
		/// <returns>ローカル名</returns>
		public static string ToFairLocalPath(string str, int dirSize)
		{
			const int MY_PATH_MAX = 250;
			const string NG_CHARS = "\"*/:<>?\\|";
			const string ALT_WORD = "_";

			int localPathSizeMax = Math.Max(0, MY_PATH_MAX - dirSize);

			if (localPathSizeMax < str.Length) // HACK: 元にしたコードではバイト列の長さで判定している。
				str = str.Substring(0, localPathSizeMax);

			str = SCommon.ToJString(SCommon.ENCODING_SJIS.GetBytes(str), true, false, false, true);

			string[] words = str.Split('.');

			for (int index = 0; index < words.Length; index++)
			{
				string word = words[index];

				word = word.Trim();

				if (
					index == 0 &&
					GetReservedWordsForWindowsPath().Any(resWord => SCommon.EqualsIgnoreCase(resWord, word)) ||
					word.Any(chr => NG_CHARS.IndexOf(chr) != -1)
					)
					word = ALT_WORD;

				words[index] = word;
			}
			str = string.Join(".", words);

			if (str == "")
				str = ALT_WORD;

			if (str.EndsWith("."))
				str = str.Substring(0, str.Length - 1) + ALT_WORD;

			return str;
		}

		#endregion

		public static double GetDistance(D2Point pt)
		{
			return Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
		}
	}
}
