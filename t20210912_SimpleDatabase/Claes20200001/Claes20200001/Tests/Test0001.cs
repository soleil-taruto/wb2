using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Charlotte.Commons;
using Charlotte.SimpleDatabases;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void MakeTestVector01()
		{
			SCommon.DeletePath(@"C:\temp\Data01");

			SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data01", 100000000L);

			ProcMain.WriteLog("MakeTestVector01_ST");

			for (int c = 0; c < 1000000; c++)
			{
				if (c % 50000 == 0)
					ProcMain.WriteLog("MakeTestVector01_" + c);

				List<string> row = new List<string>();

				for (int d = 0; d < 10; d++)
					row.Add("" + SCommon.CRandom.GetUInt());

				for (int d = 0; d < 10; d++)
					row.Add("" + SCommon.CRandom.GetUInt64());

				sd.Add(row.ToArray());
			}

			ProcMain.WriteLog("MakeTestVector01_ED");
		}

		public void Test01()
		{
			SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data01", 100000000L);

			ProcMain.WriteLog("Test01_ST");

			foreach (string[] row in sd.ReadToEnd())
			{
				// noop
			}

			ProcMain.WriteLog("Test01_ED");
		}

		public void Test02()
		{
			SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data01", 100000000L);
			string endPtn = "" + SCommon.CRandom.GetInt(1000000);
			int count = 0;

			ProcMain.WriteLog("Test02_ST");

			sd.Remove(row =>
			{
				if (row[0].EndsWith(endPtn))
				{
					count++;
					return true;
				}
				return false;
			});

			ProcMain.WriteLog("DELETED " + count);

			for (int c = 0; c < count; c++)
			{
				List<string> row = new List<string>();

				for (int d = 0; d < 10; d++)
					row.Add("" + SCommon.CRandom.GetUInt());

				for (int d = 0; d < 10; d++)
					row.Add("" + SCommon.CRandom.GetUInt64());

				sd.Add(row.ToArray());
			}

			ProcMain.WriteLog("Test02_ED");
		}

		public void Test03()
		{
			SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data01", 100000000L);
			string endPtn = "" + SCommon.CRandom.GetInt(1000000);
			int count = 0;

			ProcMain.WriteLog("Test03_ST");

			sd.Edit(row =>
			{
				if (row[0].EndsWith(endPtn))
				{
					count++;

					// 新しい行 -> row
					{
						List<string> dest = new List<string>();

						for (int d = 0; d < 10; d++)
							dest.Add("" + SCommon.CRandom.GetUInt());

						for (int d = 0; d < 10; d++)
							dest.Add("" + SCommon.CRandom.GetUInt64());

						row = dest.ToArray();
					}

					return row;
				}
				return null;
			});

			ProcMain.WriteLog("Test03_ED");
		}
	}
}
