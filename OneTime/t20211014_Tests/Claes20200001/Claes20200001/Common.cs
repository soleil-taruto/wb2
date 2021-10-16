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

		public class RecursiveSearch<T>
		{
			// TODO
			// TODO
			// TODO



			public Func<List<T>, bool> IsInvalid = list => false;

			public Func<List<T>, bool> IsEnd = list => false;

			public Action<List<T>> Ended = list => { };

			public Action<List<T>> AddElementNew = list => list.Add(default(T));

			public Func<List<T>, bool> MoveNextLastElement = list => true;

			public Action<List<T>> RemoveLastElement = list => SCommon.UnaddElement(list);

			public void Perform()
			{
				List<T> list = new List<T>();

			forward:
				if (this.IsInvalid(list))
					goto back;

				if (this.IsEnd(list))
				{
					this.Ended(list);
					goto back;
				}
				this.AddElementNew(list);

			next:
				if (this.MoveNextLastElement(list))
					goto forward;

			back:
				if (1 <= list.Count)
				{
					this.RemoveLastElement(list);
					goto next;
				}
			}
		}
	}
}
