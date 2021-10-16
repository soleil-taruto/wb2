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
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				Test01_a();
				ProcMain.WriteLog("Test01 OK " + testcnt);
			}
			ProcMain.WriteLog("Test01 OK!");
		}

		private void Test01_a()
		{
			bool[] humans = new bool[SCommon.CRandom.GetRange(1, 1000)]; // { (? not liar) ... }

			for (int index = 0; index < humans.Length; index++)
				humans[index] = SCommon.CRandom.GetInt(2) == 0; // ランダムに設定

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

		public void Test02()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				Test02_a();
				ProcMain.WriteLog("Test02 OK " + testcnt);
			}
			ProcMain.WriteLog("Test02 OK!");
		}

		private void Test02_a()
		{
			bool[] humans = new bool[100]; // { (? not liar) ... }

			do
			{
				for (int index = 0; index < humans.Length; index++)
					humans[index] = SCommon.CRandom.GetInt(2) == 0; // ランダムに設定
			}
			while (0 <= humans.Select(v => v ? 1 : -1).Sum()); // ? 同数 || 正直者の方が多い -- 嘘つきの方が多くなければならない。

			int[] askedCounts = new int[humans.Length];

			Func<int, int, bool> ask = (from, dest) =>
			{
				askedCounts[from]++;

				if (3 < askedCounts[from]) // 質問回数制限
					throw null;

				if (33 < askedCounts.Where(v => 1 <= v).Count()) // 質問者数制限
					throw null;

				if (humans[from]) // ? not liar
				{
					return humans[dest];
				}
				else // ? liar
				{
					return !humans[dest];
				}
			};

			bool[] answer = Test02_FindAnswer(ask);

			if (SCommon.Comp(humans, answer, (a, b) => (a ? 1 : 0) - (b ? 1 : 0)) != 0) // ? 不正解
				throw null;
		}

		private bool[] Test02_FindAnswer(Func<int, int, bool> ask)
		{
			bool[] answer = new bool[100];

			answer[0] = true; // 正直者と仮定

			for (int index = 3; index <= 99; index += 3)
			{
				answer[index] = answer[index - 3] ^ ask(index, index - 3) ^ true;
				answer[index - 2] = answer[index] ^ ask(index, index - 2) ^ true;
				answer[index - 1] = answer[index] ^ ask(index, index - 1) ^ true;
			}
			if (0 <= answer.Select(v => v ? 1 : -1).Sum()) // ? 同数 || 正直者の方が多い -> 仮定が間違っているので反転
				answer = answer.Select(v => !v).ToArray();

			return answer;
		}
	}
}
