using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.SimpleDatabases;

namespace Charlotte.Tests
{
	public class Test0002
	{
		public void Test01()
		{
			SCommon.DeletePath(@"C:\temp\Data02");

			{
				SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data02", 100000L);

				for (int c = 0; c < 10000; c++)
				{
					List<string> row = new List<string>();

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt());

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt64());

					sd.Add(row.ToArray());
				}
			}

			ProcMain.WriteLog("P-0001");
			Common.Pause();

			{
				SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data02", 201000L);

				List<string[]> rows1 = sd.ReadToEnd().ToList();

				for (int c = 0; c < 1000; c++)
				{
					List<string> row = new List<string>();

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt());

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt64());

					string[] aRow = row.ToArray();
					rows1.Add(aRow);
					sd.Add(aRow);
				}

				string[][] rows2 = sd.ReadToEnd().ToArray();

				if (SCommon.Comp(rows1, rows2, (a, b) => SCommon.Comp(a, b, SCommon.Comp)) != 0) // ? rows1, rows2 不一致
					throw null; // bug
			}

			ProcMain.WriteLog("P-0002");
			Common.Pause();

			{
				SimpleDatabase sd = new SimpleDatabase(@"C:\temp\Data02", 403000L);

				List<string[]> rows1 = sd.ReadToEnd().ToList();

				for (int c = 0; c < 1500; c++)
				{
					List<string> row = new List<string>();

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt());

					for (int d = 0; d < 10; d++)
						row.Add("" + SCommon.CRandom.GetUInt64());

					string[] aRow = row.ToArray();
					rows1.Add(aRow);
					sd.Add(aRow);
				}

				string[][] rows2 = sd.ReadToEnd().ToArray();

				if (SCommon.Comp(rows1, rows2, (a, b) => SCommon.Comp(a, b, SCommon.Comp)) != 0) // ? rows1, rows2 不一致
					throw null; // bug
			}

			ProcMain.WriteLog("P-0003");
			Common.Pause();

			ProcMain.WriteLog("END");
		}
	}
}
