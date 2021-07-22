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
		}

		private static string[] TEST_VECTORS = new string[]
		{
			"",
			"A",
			"AB",
			"ABC",
			":",
			"::",
			":::",
			"A:B",
			"A:B:C",
			"Aa:::",
			":Bb::",
			"::Cc:",
			":::Dd",
			"Aa:::",
			"Aa:Bb::",
			"Aa::Cc:",
			"Aa:::Dd",
			":Bb::",
			":Bb:Cc:",
			":Bb::Dd",
			"::Cc:",
			"::Cc:Dd",
			"Aa:Bb:Cc:",
			"Aa:Bb::Dd",
			"Aa::Cc:Dd",
			":Bb:Cc:Dd",
			"Aa:Bb:Cc:Dd",
		};

		private static string[] TEST_VECTORS_02 = ":A:B:C:D:Aa:Bb:Cc:Dd".Split(':');

		private void Test_SCommon_Serializer_Join()
		{
			Action<string[]> a = src =>
			{
				string dest = SCommon.Serializer.I.Join(src);
				Console.WriteLine(dest);

				string[] src2 = SCommon.Serializer.I.Split(dest);

				if (SCommon.Comp(src, src2, SCommon.Comp) != 0)
					throw null;
			};

			Action<string> a2 = line =>
			{
				a(line.Split(':'));
			};

			a(new string[0]);

			foreach (string tv in TEST_VECTORS)
				a2(tv);
		}

		private void Test_SCommon_LinesToText()
		{
			Action<string[]> a = src =>
			{
				string dest = SCommon.LinesToText(src);
				Console.WriteLine(dest);

				string[] src2 = SCommon.TextToLines(dest);

				if (SCommon.Comp(src, src2, SCommon.Comp) != 0)
					throw null;
			};

			Action<string> a2 = line =>
			{
				a(line.Split(':'));
			};

			a(new string[0]);

			foreach (string tv in TEST_VECTORS)
				a2(tv);
		}

		private void Test_SCommon_IndexOf()
		{
			Action<string[], string> a = (src, token) =>
			{
				int index = SCommon.IndexOf(src, token);
				Console.WriteLine(index);
			};

			Action<string, string> a2 = (line, token) =>
			{
				a(line.Split(':'), token);
			};

			foreach (string tv2 in TEST_VECTORS_02)
			{
				a(new string[0], tv2);

				foreach (string tv in TEST_VECTORS)
					a2(tv, tv2);
			}
		}

		private void Test_SCommon_IndexOfIgnoreCase()
		{
			Action<string[], string> a = (src, token) =>
			{
				int index = SCommon.IndexOfIgnoreCase(src, token);
				Console.WriteLine(index);
			};

			Action<string, string> a2 = (line, token) =>
			{
				a(line.Split(':'), token);
			};

			foreach (string tv2 in TEST_VECTORS_02)
			{
				a(new string[0], tv2);

				foreach (string tv in TEST_VECTORS)
					a2(tv, tv2);
			}
		}

		private void Test_SCommon_Join()
		{
			Action<string[]> a = src =>
			{
				byte[] dest = SCommon.Join(src.Select(token => Encoding.UTF8.GetBytes(token)).ToArray());
				Console.WriteLine(SCommon.Hex.ToString(dest));
			};

			Action<string> a2 = line =>
			{
				a(line.Split(':'));
			};

			a(new string[0]);

			foreach (string tv in TEST_VECTORS)
				a2(tv);
		}

		private void Test_SCommon_SplittableJoin()
		{
			Action<string[]> a = src =>
			{
				byte[] dest = SCommon.SplittableJoin(src.Select(token => Encoding.UTF8.GetBytes(token)).ToArray());
				Console.WriteLine(SCommon.Hex.ToString(dest));
			};

			Action<string> a2 = line =>
			{
				a(line.Split(':'));
			};

			a(new string[0]);

			foreach (string tv in TEST_VECTORS)
				a2(tv);
		}

		private void Test_CsvFileWriter_WriteCells()
		{
			throw new NotImplementedException();
		}

		private void Test_CsvFileWriter_WriteRow()
		{
			throw new NotImplementedException();
		}

		private void Test_CsvFileWriter_WriteRows()
		{
			throw new NotImplementedException();
		}
	}
}
