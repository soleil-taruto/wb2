using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0003
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

			Test01_Search search = new Test01_Search()
			{
				Queen = queen,
			};
			search.Perform();
			int answer = search.Answer;

			ProcMain.WriteLog(queen + " ==> " + answer);
		}

		private class Test01_Search : RecursiveSearch<int>
		{
			public int Queen;
			public int Answer = 0;

			protected override bool IsEnd(List<int> list)
			{
				// 間違っているかチェック
				// -- 最後の要素についてのみチェックすれば良い。

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

				// 完成しているかチェック

				if (this.Queen <= list.Count)
				{
					this.Answer++;
					return true;
				}

				// 継続

				return false;
			}

			protected override IEnumerable<int> E_GetElements(List<int> list)
			{
				return Enumerable.Range(0, this.Queen);
			}
		}

		/// <summary>
		/// 深さ優先探索によるリストの生成
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		public abstract class RecursiveSearch<T>
		{
			/// <summary>
			/// これ以上リストを延長する必要が無いか判定する。
			/// 或いは ( 途中までのリストが間違っている || リストが完成している ) を返す。
			/// </summary>
			/// <param name="list">リスト</param>
			/// <returns>これ以上リストを延長する必要が無いか</returns>
			protected abstract bool IsEnd(List<T> list);

			/// <summary>
			/// 要素の値を列挙する。
			/// </summary>
			/// <param name="list">リスト</param>
			/// <returns>要素の値の列挙</returns>
			protected abstract IEnumerable<T> E_GetElements(List<T> list);

			/// <summary>
			/// 探索実行
			/// </summary>
			public void Perform()
			{
				List<IEnumerator<T>> ites = new List<IEnumerator<T>>();
				List<T> list = new List<T>();
				int index = -1;

			forward:
				ites.Add(this.E_GetElements(list).GetEnumerator());
				list.Add(default(T));
				index++;

			next:
				if (ites[index].MoveNext())
				{
					list[index] = ites[index].Current;

					if (this.IsEnd(list))
						goto next;

					goto forward;
				}
				ites.RemoveAt(index);
				list.RemoveAt(index);
				index--;

				if (0 <= index)
					goto next;
			}
		}
	}
}
