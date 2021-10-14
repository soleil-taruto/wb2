using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void Test01()
		{
			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				Test01_a();
			}
		}

		private void Test01_a()
		{
			bool[] humans = new bool[SCommon.CRandom.GetRange(1, 1000)]; // { (? not liar) ... }

			for (int index = 0; index < humans.Length; index++)
				humans[index] = SCommon.CRandom.GetInt(2) == 0;

			bool[] says = new bool[humans.Length]; // { (? say: next is not liar) ... }

			for (int index = 0; index < humans.Length; index++)
			{
				bool curr = humans[index];
				bool next = humans[(index + 1) % humans.Length];

				if (curr) // ? not liar
				{
					says[index] = next;
				}
				else // ? liar
				{
					says[index] = !next;
				}
			}

			int sayLiarCount = says.Where(v => !v).Count();

			if (sayLiarCount % 2 != 0) // ? 偶数ではない。
				throw null;

			Console.WriteLine(sayLiarCount + ", " + humans.Length);
		}
	}
}
