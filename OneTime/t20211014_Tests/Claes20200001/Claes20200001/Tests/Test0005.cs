using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0005
	{
		public void Test01()
		{
			Test01_a(null);
			Test01_a(new string[] { });
			Test01_a(new string[] { null });
			Test01_a(new string[] { null, null });
			Test01_a(new string[] { null, null, null });
			Test01_a(new string[] { "" });
			Test01_a(new string[] { "", "" });
			Test01_a(new string[] { "", "", "" });
			Test01_a(new string[] { "ABC", });
			Test01_a(new string[] { "ABC", "DEF" });
			Test01_a(new string[] { "ABC", "DEF", "GHI" });
			Test01_a(new string[] { null, "xxxxx" });
			Test01_a(new string[] { "xxxxx", null });
			Test01_a(new string[] { null, "123", "456" });
			Test01_a(new string[] { "123", null, "456" });
			Test01_a(new string[] { "123", "456", null });

			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				Console.WriteLine(testcnt);

				int count = SCommon.CRandom.GetRange(1, 30);
				string[] strs = new string[count];

				for (int index = 0; index < count; index++)
				{
					Func<int, int, string> a_makeString = (minlen, maxlen) =>
						new string(Enumerable.Range(1, SCommon.CRandom.GetRange(minlen, maxlen))
							.Select(dummy => SCommon.CRandom.ChooseOne(SCommon.HALF.ToArray()))
							.ToArray());

					string str;

					switch (SCommon.CRandom.GetRange(1, 5))
					{
						case 1:
							str = null;
							break;

						case 2:
							str = a_makeString(0, 100);
							break;

						case 3:
							str = a_makeString(100, 300);
							break;

						case 4:
							str = a_makeString(300, 1000);
							break;

						case 5:
							str = a_makeString(1000, 3000);
							break;

						case 6:
							str = a_makeString(3000, 10000);
							break;

						case 7:
							str = a_makeString(10000, 33000);
							break;

						default:
							throw null;
					}
					strs[index] = str;
				}
				Test01_a(strs);
			}
			Console.WriteLine("OK!");
		}

		private void Test01_a(string[] strs)
		{
			string seriStr = SerializeStrings(strs);
			string[] strs2 = DeserializeStrings(seriStr);

			if (string.IsNullOrEmpty(seriStr))
				throw null;

			if (strs == null)
			{
				if (strs2 != null)
					throw null;
			}
			else
			{
				if (strs2 == null)
					throw null;

				if (SCommon.Comp(
					strs,
					strs2,
					(a, b) =>
					{
						if (a == null && b == null)
							return 0;

						if (a == null) // ? (a, b) == (null, not null) --> a < b
							return -1;

						if (b == null) // ? (a, b) == (not null, null) --> a > b
							return 1;

						return SCommon.Comp(a, b);
					}
					) != 0
					)
					throw null;
			}
		}

		private string SerializeStrings(string[] strs)
		{
			const char DUMMY_CHAR = '?';

			if (strs == null)
				strs = new string[] { "" };
			else
				strs = new string[] { "", "" }.Concat(strs).ToArray();

			strs = strs
				.Select(str => str == null ? "" : DUMMY_CHAR + str)
				.ToArray();

			byte[][] bStrs = strs
				.Select(str => Encoding.UTF8.GetBytes(str))
				.ToArray();

			StringBuilder buff = new StringBuilder();

			foreach (byte[] bStr in bStrs)
			{
				string sSize = ((uint)bStr.Length).ToString("x8");

				buff.Append("" + sSize.Length);
				buff.Append(sSize);

				foreach (byte bChr in bStr)
					buff.Append(bChr.ToString("x2"));
			}
			return buff.ToString();
		}

		private string[] DeserializeStrings(string seriStr)
		{
			List<byte[]> bStrs = new List<byte[]>();

			for (int index = 0; index < seriStr.Length; )
			{
				int szSize = int.Parse(seriStr.Substring(index, 1));
				index++;

				int size = (int)Convert.ToUInt32(seriStr.Substring(index, szSize), 16);
				index += szSize;

				byte[] bStr = new byte[size];

				for (int i = 0; i < size; i++)
				{
					byte bChr = (byte)Convert.ToUInt32(seriStr.Substring(index, 2), 16);
					index += 2;

					bStr[i] = bChr;
				}
				bStrs.Add(bStr);
			}
			string[] strs = bStrs
				.Select(bStr => Encoding.UTF8.GetString(bStr))
				.ToArray();

			strs = strs
				.Select(str => str.Length == 0 ? null : str.Substring(1))
				.ToArray();

			if (strs.Length < 2)
				strs = null;
			else
				strs = strs.Skip(2).ToArray();

			return strs;
		}
	}
}
