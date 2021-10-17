using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0002
	{
		public void Test01()
		{
			for (int queen = 1; queen <= 18; queen++)
			{
				Test01_a(queen);
			}
		}

		private void Test01_a(int queen)
		{
			// n-クイーン問題

			int answer = 0;

			new RecursiveSearch<int>()
			{
				IsEnd = list =>
				{
					int ex = list[list.Count - 1];
					int ey = list.Count - 1;
					int ep = ex + ey;
					int er = ex - ey;

					for (int y = 0; y < list.Count - 1; y++)
					{
						int x = list[y];
						int p = x + y;
						int r = x - y;

						if (
							ex == x ||
							ep == p ||
							er == r
							)
							return true;
					}
					if (queen <= list.Count)
					{
						answer++;
						return true;
					}
					return false;
				},
				CreateZeroThElement = list => -1,
				MoveToFirstOrNextElement = (list, x, setter) =>
				{
					if (queen <= ++x)
						return false;

					setter(x);
					return true;
				},
				ReleaseElement = (list, element) => { },
			}
			.Perform();

			ProcMain.WriteLog(queen + " ==> " + answer);
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
