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

			new Common.RecursiveSearch<int>()
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
	}
}
