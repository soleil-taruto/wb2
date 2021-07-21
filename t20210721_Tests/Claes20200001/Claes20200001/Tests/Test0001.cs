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
			a2("");
			a2("A");
			a2("AB");
			a2("ABC");
			a2("ABCD");
			a2("ABCDE");
			a2("A:BCDE");
			a2("A:B:CDE");
			a2("A:B:C:DE");
			a2("A:B:C:D:E");
			a2(":");
			a2("::");
			a2(":::");
		}

		private void Test_SCommon_LinesToText()
		{
			throw new NotImplementedException();
		}

		private void Test_SCommon_IndexOf()
		{
			throw new NotImplementedException();
		}

		private void Test_SCommon_IndexOfIgnoreCase()
		{
			throw new NotImplementedException();
		}

		private void Test_SCommon_Join()
		{
			throw new NotImplementedException();
		}

		private void Test_SCommon_SplittableJoin()
		{
			throw new NotImplementedException();
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
