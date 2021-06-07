using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class ExtensionsTest
	{
		public void Test01()
		{
			Test01_a("", "");
			Test01_a("abc", "abc");
			Test01_a("54321", "12345");
			Test01_a("AAAAAAAAA", "A");
			Test01_a("BABABABABA", "AB");
			Test01_a("ABCCCCCCCDE", "ABCDE");
			Test01_a("EDCCCCCCCBA", "ABCDE");
			Test01_a("AAAAABCDEEEEE", "ABCDE");
			Test01_a("EEEEEDCBAAAAA", "ABCDE");
			Test01_a("AABBCCABCCBACCBBAA", "ABC");
		}

		private void Test01_a(string src, string assume)
		{
			string ans = new string(src.ToArray().DistinctOrderBy((a, b) => (int)a - (int)b).ToArray());

			if (ans != assume)
				throw null; // BUG !!!
		}

		public void Test02()
		{
			ProcMain.WriteLog("Test02 Start");

			Test02_Main(0, 1);
			Test02_Param(
				3000,
				new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 30, 100 },
				new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 30, 100 }
				);
			Test02_Param(
				100,
				new[] { 10, 100, 1000, 10000 },
				new[] { 10, 100, 1000, 10000 }
				);

			ProcMain.WriteLog("Test02 OK!");
		}

		private void Test02_Param(int testCount, int[] arrLengthMaxs, int[] valueLimitMaxs)
		{
			ProcMain.WriteLog("testCount: " + testCount);

			for (int testcnt = 0; testcnt < testCount; testcnt++)
			{
				foreach (int arrLengthMax in arrLengthMaxs)
				{
					foreach (int valueLimitMax in valueLimitMaxs)
					{
						int arrLength = SCommon.CRandom.GetRange(1, arrLengthMax);
						int valueLimit = SCommon.CRandom.GetRange(1, valueLimitMax);

						Test02_Main(arrLength, valueLimit);
					}
				}
			}
			ProcMain.WriteLog("done");
		}

		private void Test02_Main(int arrLength, int valueLimit)
		{
			int[] src = Enumerable.Range(1, arrLength).Select(dummy => SCommon.CRandom.GetInt(valueLimit)).ToArray();
			int[] ans = src.DistinctOrderBy(SCommon.Comp).ToArray();
			int[] assume = Test02_GetAssumeValue(src);

			if (SCommon.Comp(ans, assume, SCommon.Comp) != 0) // ? 不一致
				throw null; // BUG !!!
		}

		private int[] Test02_GetAssumeValue(int[] src)
		{
			if (src.Length == 0)
				return new int[0];

			bool[] valueExists = new bool[src.Max() + 1];

			foreach (int value in src)
				valueExists[value] = true;

			return Enumerable.Range(0, valueExists.Length).Where(value => valueExists[value]).ToArray();
		}
	}
}
