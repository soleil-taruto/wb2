using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0002
	{
		public void Test01()
		{
			Test_FileCopy();

			// ----

			Console.WriteLine("OK!");
			Common.Pause();
		}

		private void Test_FileCopy()
		{
			Test_FileCopy_Test01(0);
			Test_FileCopy_Test01(1);
			Test_FileCopy_Test01(100);
			Test_FileCopy_Test01(100000); // 100 KB
			Test_FileCopy_Test01(100000000); // 100 MB

			for (int testCnt = 0; testCnt < 1000; testCnt++)
			{
				Test_FileCopy_Test01(SCommon.CRandom.GetInt(100000000)); // < 100 MB
			}
		}

		private void Test_FileCopy_Test01(int testDataSize)
		{
			Console.WriteLine("Test_FileCopy_Test01: " + testDataSize); // cout

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

				Test_FileCopy_Test02(rFile, wFile);

				// 検査
				{
					if (SCommon.Comp(testData, File.ReadAllBytes(rFile)) != 0)
						throw null;

					if (SCommon.Comp(testData, File.ReadAllBytes(wFile)) != 0)
						throw null;
				}
			}

			Console.WriteLine("Test_FileCopy_Test01_OK"); // cout
		}

		private void Test_FileCopy_Test02(string rFile, string wFile)
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
	}
}
