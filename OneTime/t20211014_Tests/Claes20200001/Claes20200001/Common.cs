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
			/// 途中までのリストが間違っているか判定する。
			/// </summary>
			public Func<List<T>, bool> IsInvalid = list => false;

			/// <summary>
			/// リストが完成しているか判定する。
			/// </summary>
			public Func<List<T>, bool> IsEnd = list => false;

			/// <summary>
			/// 完成したリストに対するリアクション
			/// 戻り値：探索を終了するか
			/// </summary>
			public Func<List<T>, bool> Ended = list => false;

			/// <summary>
			/// 要素を生成する。
			/// 要素の取り得る値を e(1), e(2), e(3), ... e(N - 2), e(N - 1), e(N) とすると e(0) を返す。
			/// </summary>
			public Func<List<T>, T> CreateZeroThElement = list => default(T);

			/// <summary>
			/// 要素の取り得る値を e(1), e(2), e(3), ... e(N - 2), e(N - 1), e(N) とする。
			/// - - -
			/// 要素の現在値 != e(N) --> 要素の現在値 == e(i) -- setter(e(i + 1)); return true;
			/// 要素の現在値 == e(N) --> return false;
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
				if (this.IsInvalid(list))
					goto back;

				if (this.IsEnd(list))
				{
					if (this.Ended(list))
						return;

					goto back;
				}
				list.Add(this.CreateZeroThElement(list));

			next:
				if (this.MoveToFirstOrNextElement(list, list[list.Count - 1], element => list[list.Count - 1] = element))
					goto forward;

			back:
				if (1 <= list.Count)
				{
					this.ReleaseElement(list, SCommon.UnaddElement(list));
					goto next;
				}
			}
		}
	}
}
