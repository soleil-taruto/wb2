using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0006
	{
		public void Test01()
		{
			Test01_a(new string[0]);

			char[] TEST_CHARS = (SCommon.HALF + SCommon.GetJChars()).ToArray();

			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				// cout
				if (testcnt % 100 == 0)
					Console.WriteLine(testcnt);

				string[] testStrings = new string[SCommon.CRandom.GetRange(1, 100)];

				for (int index = 0; index < testStrings.Length; index++)
				{
					char[] testStringChrs = new char[SCommon.CRandom.GetRange(0, 100)];

					for (int ndx = 0; ndx < testStringChrs.Length; ndx++)
						testStringChrs[ndx] = SCommon.CRandom.ChooseOne(TEST_CHARS);

					testStrings[index] = new string(testStringChrs);
				}
				Test01_a(testStrings);
			}
			Console.WriteLine("OK!");
		}

		private void Test01_a(string[] testStrings)
		{
			string enc = SCommon.Serializer.I.Join(testStrings);
			string[] dec = SCommon.Serializer.I.Split(enc);

			if (!Regex.IsMatch(enc, "^[A-Za-z0-9+/]{2,}$")) // ? 書式エラー
				throw null;

			if (SCommon.Comp(testStrings, dec, SCommon.Comp) != 0) // ? 不一致
				throw null;
		}
	}
}
