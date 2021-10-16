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
		/// 深さ優先探索によるリストの生成
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		public class RecursiveSearch<T>
		{
			/// <summary>
			/// これ以上リストを延長する必要が無いか判定する。
			/// 或いは ( 途中までのリストが間違っている || リストが完成している ) を返す。
			/// </summary>
			public Func<List<T>, bool> IsEnd = list => false;

			/// <summary>
			/// 要素を生成する。
			/// 最初の値ではないことに注意！
			/// </summary>
			public Func<List<T>, T> CreateZeroThElement = list => default(T);

			/// <summary>
			/// 初回：最初の値へ移動する。return true;
			/// 2回目以降：次の値へ移動する。return true;
			/// 最後(次の値は無い)：return false;
			/// </summary>
			public Func<List<T>, T, Action<T>, bool> MoveToFirstOrNextElement = (list, element, setter) => { setter(element); return true; };

			/// <summary>
			/// 要素を解放する。
			/// </summary>
			public Action<List<T>, T> ReleaseElement = (list, element) => { };

			/// <summary>
			/// 探索実行
			/// </summary>
			public void Perform()
			{
				List<T> list = new List<T>();

			forward:
				list.Add(this.CreateZeroThElement(list));

			next:
				if (this.MoveToFirstOrNextElement(list, list[list.Count - 1], element => list[list.Count - 1] = element))
				{
					if (this.IsEnd(list))
						goto next;

					goto forward;
				}
				this.ReleaseElement(list, SCommon.UnaddElement(list));

				if (1 <= list.Count)
					goto next;
			}
		}
	}
}
