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

		public static double GetDistance(D2Point pt)
		{
			return Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
		}

		/// <summary>
		/// マージする。
		/// </summary>
		/// <typeparam name="T">任意の型</typeparam>
		/// <param name="list1">リスト1 -- ソート済みであること</param>
		/// <param name="list2">リスト2 -- ソート済みであること</param>
		/// <param name="comp">要素の比較メソッド</param>
		/// <param name="only1">出力先 -- リスト1のみ存在</param>
		/// <param name="only2">出力先 -- リスト2のみ存在</param>
		/// <param name="both">出力先 -- 両方に存在 -- リスト1の要素を追加</param>
		public static void Merge<T>(IList<T> list1, IList<T> list2, Comparison<T> comp, List<T> only1, List<T> only2, List<T> both)
		{
			int index1 = 0;
			int index2 = 0;

			while (index1 < list1.Count && index2 < list2.Count)
			{
				int ret = comp(list1[index1], list2[index2]);

				if (ret < 0)
				{
					only1.Add(list1[index1++]);
				}
				else if (0 < ret)
				{
					only2.Add(list2[index2++]);
				}
				else
				{
					both.Add(list1[index1++]);
					index2++;
				}
			}
			while (index1 < list1.Count)
			{
				only1.Add(list1[index1++]);
			}
			while (index2 < list2.Count)
			{
				only2.Add(list2[index2++]);
			}
		}
	}
}
