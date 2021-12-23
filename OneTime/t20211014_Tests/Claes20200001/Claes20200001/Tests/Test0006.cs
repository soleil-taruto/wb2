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
			Test01_a(new string[] { "" });
			Test01_a(new string[] { "", "" });
			Test01_a(new string[] { "", "", "" });
			Test01_a(new string[] { "ABC" });
			Test01_a(new string[] { "ABC", "DEF" });
			Test01_a(new string[] { "ABC", "DEF", "GHI" });

			char[] TEST_CHARS = (SCommon.HALF + SCommon.GetJChars()).ToArray();

			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
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

			// cout
			{
				string enc4Prn = enc.Length < 78 ? enc : enc.Substring(0, 30) + " ... " + enc.Substring(enc.Length - 30);

				Console.WriteLine("< " + testStrings.Length);
				Console.WriteLine("* " + enc4Prn);
				Console.WriteLine("> " + dec.Length);
			}

			if (!Regex.IsMatch(enc, "^[A-Za-z0-9+/]{2,}$")) // ? 書式エラー
				throw null;

			if (SCommon.Comp(testStrings, dec, SCommon.Comp) != 0) // ? 不一致
				throw null;
		}
	}
}
