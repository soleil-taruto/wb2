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
			Test_SCommon_Serializer_Join();
			Test_SCommon_LinesToText();
			Test_SCommon_IndexOf();
			Test_SCommon_IndexOfIgnoreCase();
			Test_SCommon_Join();
			Test_SCommon_SplittableJoin();
			Test_CsvFileWriter_WriteCells();
			Test_CsvFileWriter_WriteRow();
			Test_CsvFileWriter_WriteRows();

			// ----

			Console.WriteLine("OK!");
			Common.Pause();
		}

		private static class TestVector
		{
			private static char[] ALLOW_CHARS = "12345ABCDEabcde".ToCharArray();
			private const int STRING_LEN_MAX = 10;
			private const int STRING_COUNT_MAX = 10;
			private const int STRING_LIST_COUNT_MAX = 10;

			public static char GetChar()
			{
				return SCommon.CRandom.ChooseOne(ALLOW_CHARS);
			}

			public static string GetString()
			{
				return new string(Enumerable.Range(0, SCommon.CRandom.GetInt(STRING_LEN_MAX)).Select(dummy => GetChar()).ToArray());
			}

			public static string[] GetStrings()
			{
				return Enumerable.Range(0, SCommon.CRandom.GetInt(STRING_COUNT_MAX)).Select(dummy => GetString()).ToArray();
			}

			public static string[][] GetStringTable()
			{
				return Enumerable.Range(0, SCommon.CRandom.GetInt(STRING_LIST_COUNT_MAX)).Select(dummy => GetStrings()).ToArray();
			}

			public const int TEST_COUNT = 1000;
		}

		private void Test_SCommon_Serializer_Join()
		{
			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				string[] src = TestVector.GetStrings();
				string dest = SCommon.Serializer.I.Join(src);
				string[] src2 = SCommon.Serializer.I.Split(dest);

				if (SCommon.Comp(src, src2, SCommon.Comp) != 0)
					throw null;
			}
		}

		private void Test_SCommon_LinesToText()
		{
			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				string[] src = TestVector.GetStrings();
				string dest = SCommon.LinesToText(src);
				string[] src2 = SCommon.TextToLines(dest);

				if (SCommon.Comp(src, src2, SCommon.Comp) != 0)
					throw null;
			}
		}

		private void Test_SCommon_IndexOf()
		{
			Func<string[], string, int> testFunc = (tokens, token) =>
			{
				for (int index = 0; index < tokens.Length; index++)
					if (tokens[index] == token)
						return index;

				return -1;
			};

			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				string[] tokens = TestVector.GetStrings();
				string token = TestVector.GetString();

				int ans = SCommon.IndexOf(tokens, token);
				int ans2 = testFunc(tokens, token);

				if (ans != ans2)
					throw null;
			}
		}

		private void Test_SCommon_IndexOfIgnoreCase()
		{
			Func<string[], string, int> testFunc = (tokens, token) =>
			{
				for (int index = 0; index < tokens.Length; index++)
					if (tokens[index].ToLower() == token.ToLower())
						return index;

				return -1;
			};

			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				string[] tokens = TestVector.GetStrings();
				string token = TestVector.GetString();

				int ans = SCommon.IndexOfIgnoreCase(tokens, token);
				int ans2 = testFunc(tokens, token);

				if (ans != ans2)
					throw null;
			}
		}

		private void Test_SCommon_Join()
		{
			Func<byte[][], byte[]> testFunc = src =>
			{
				return SCommon.Hex.ToBytes(string.Join("", src.Select(v => SCommon.Hex.ToString(v))));
			};

			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				byte[][] src = TestVector.GetStrings().Select(v => Encoding.ASCII.GetBytes(v)).ToArray();

				byte[] ans = SCommon.Join(src);
				byte[] ans2 = testFunc(src);

				if (SCommon.Comp(ans, ans2, SCommon.Comp) != 0) // ? 不一致
					throw null;
			}
		}

		private void Test_SCommon_SplittableJoin()
		{
			for (int count = 0; count < TestVector.TEST_COUNT; count++)
			{
				byte[][] src = TestVector.GetStrings().Select(v => Encoding.ASCII.GetBytes(v)).ToArray();
				byte[] dest = SCommon.SplittableJoin(src);
				byte[][] src2 = SCommon.Split(dest);

				if (SCommon.Comp(src, src2, SCommon.Comp) != 0) // ? 不一致
					throw null;
			}
		}

		private void Test_CsvFileWriter_WriteCells()
		{
			using (WorkingDir wd = new WorkingDir())
			using (CsvFileWriter writer = new CsvFileWriter(wd.MakePath() + ".csv"))
			{
				for (int index = 0; index < TestVector.TEST_COUNT; index++)
					writer.WriteCells(TestVector.GetStrings());
			}
		}

		private void Test_CsvFileWriter_WriteRow()
		{
			using (WorkingDir wd = new WorkingDir())
			using (CsvFileWriter writer = new CsvFileWriter(wd.MakePath() + ".csv"))
			{
				for (int index = 0; index < TestVector.TEST_COUNT; index++)
					writer.WriteRow(TestVector.GetStrings());
			}
		}

		private void Test_CsvFileWriter_WriteRows()
		{
			using (WorkingDir wd = new WorkingDir())
			using (CsvFileWriter writer = new CsvFileWriter(wd.MakePath() + ".csv"))
			{
				for (int index = 0; index < TestVector.TEST_COUNT; index++)
					writer.WriteRows(TestVector.GetStringTable());
			}
		}
	}
}
