using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0003B
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

			protected override bool IsInvalid(List<int> list)
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
				return false;
			}

			protected override bool IsEnd(List<int> list)
			{
				if (this.Queen <= list.Count)
				{
					return true;
				}
				return false;
			}

			protected override void Ended(List<int> list)
			{
				this.Answer++;
			}

			protected override IEnumerable<int> E_GetElements(List<int> list)
			{
				return Enumerable.Range(0, this.Queen);
			}
		}

		public abstract class RecursiveSearch<T>
		{
			/// <summary>
			/// リストが間違っているか判定する。
			/// 途中までのリストである場合がある。
			/// 長さが2以上の場合 !IsInvalid(list.Take(list.Count - 1).AsList()) であったことが保証されるので
			/// リストの最後の要素についてチェックすれば良い。
			/// </summary>
			/// <param name="list">リスト</param>
			/// <returns>判定</returns>
			protected abstract bool IsInvalid(List<T> list);

			/// <summary>
			/// リストが完成しているか判定する。
			/// 完成した場合、これ以上リストを延長しない。
			/// </summary>
			/// <param name="list">リスト</param>
			/// <returns>判定</returns>
			protected abstract bool IsEnd(List<T> list);

			/// <summary>
			/// 完成したリストに対するリアクション
			/// </summary>
			/// <param name="list">リスト</param>
			protected abstract void Ended(List<T> list);

			/// <summary>
			/// 要素の値を列挙する。
			/// </summary>
			/// <param name="list">リスト</param>
			/// <returns>値の列挙</returns>
			protected abstract IEnumerable<T> E_GetElements(List<T> list);

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

					if (this.IsInvalid(list))
						goto next;

					if (this.IsEnd(list))
					{
						this.Ended(list);
						goto next;
					}
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
