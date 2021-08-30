using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	/// <summary>
	/// ファイルコピー, SCommon.ReadToEnd
	/// </summary>
	public class Test0002
	{
		public void Test01()
		{
			Test01a(0);
			Test01a(1);
			Test01a(100);
			Test01a(100000); // 100 KB
			Test01a(100000000); // 100 MB

			for (int testCnt = 0; testCnt < 1000; testCnt++)
			{
				Test01a(SCommon.CRandom.GetInt(100000000)); // < 100 MB
			}

			ProcMain.WriteLog("OK!");
			Common.Pause();
		}

		private void Test01a(int testDataSize)
		{
			ProcMain.WriteLog("Test01a: " + testDataSize);

			byte[] testData = SCommon.CRandom.GetBytes(testDataSize);

			using (WorkingDir wd = new WorkingDir())
			{
				string rFile = wd.MakePath();
				string wFile = wd.MakePath();

				File.WriteAllBytes(rFile, testData);

				// 検査
				{
					if (SCommon.Comp(testData, File.ReadAllBytes(rFile)) != 0)
						throw null;

					if (File.Exists(wFile))
						throw null;
				}

				Test01b(rFile, wFile);

				// 検査
				{
					if (SCommon.Comp(testData, File.ReadAllBytes(rFile)) != 0)
						throw null;

					if (SCommon.Comp(testData, File.ReadAllBytes(wFile)) != 0)
						throw null;
				}
			}

			ProcMain.WriteLog("Test01a_OK");
		}

		private void Test01b(string rFile, string wFile)
		{
			using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
			using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
			{
#if true
				SCommon.ReadToEnd(reader.Read, writer.Write);
#else // same
				SCommon.ReadToEnd(
					(buff, offset, count) => reader.Read(buff, offset, count),
					(buff, offset, count) => writer.Write(buff, offset, count)
					);
#endif
			}
		}

		public void Test02()
		{
			DateTime stTm = DateTime.Now;

			Test01a(100000000); // 100 MB



			DateTime edTm = DateTime.Now;

			ProcMain.WriteLog((edTm - stTm).TotalMilliseconds + " ミリ秒");
			Common.Pause();
		}
	}
}
