using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Camellias;

namespace Charlotte.Tests
{
	public class Test0004
	{
		private CsvFileWriter Writer;

		public void Test01()
		{
			Writer = new CsvFileWriter(Common.NextOutputPath() + ".csv");

			// ----

			Test01_a(16);
			Test01_a(24);
			Test01_a(32);
			Test01_a(64);
			Test01_a(96);
			Test01_a(128);

			// ----

			Writer.Dispose();
			Writer = null;
		}

		private void Test01_a(int keySize)
		{
			Test01_b(keySize, 0);
			Test01_b(keySize, 10);
			Test01_b(keySize, 30);
			Test01_b(keySize, 100);
			Test01_b(keySize, 300);
			Test01_b(keySize, 1000);
			Test01_b(keySize, 3000);
			Test01_b(keySize, 10000);
			Test01_b(keySize, 30000);
			Test01_b(keySize, 100000);
			Test01_b(keySize, 300000);
			Test01_b(keySize, 1000000);
			//Test01_b(keySize, 3000000);
			//Test01_b(keySize, 10000000);
			//Test01_b(keySize, 30000000);
			//Test01_b(keySize, 100000000);
			//Test01_b(keySize, 300000000);
		}

		private void Test01_b(int keySize, int dataSize)
		{
			for (int testcnt = 0; testcnt < 3; testcnt++)
			{
				MeasureProcessTime(keySize, dataSize);
			}
		}

		public void MeasureProcessTime(int keySize, int dataSize)
		{
			DateTime stTm = DateTime.Now;

			ProcMain.WriteLog("MPT " + keySize + ", " + dataSize);

			byte[] rawKey = SCommon.CRandom.GetBytes(keySize);
			byte[] testData = SCommon.CRandom.GetBytes(dataSize);
			byte[] encData;
			byte[] decData;
			long encTime;
			long decTime;
			long encTime2;
			long decTime2;

			using (RingCipher transformer = new RingCipher(rawKey))
			{
				// メモリ版・暗号化
				{
					DateTime t1 = DateTime.Now;
					encData = transformer.Encrypt(testData);
					DateTime t2 = DateTime.Now;
					encTime = (long)((t2 - t1).TotalMilliseconds);
				}

				// メモリ版・復号
				{
					DateTime t1 = DateTime.Now;
					decData = transformer.Decrypt(encData);
					DateTime t2 = DateTime.Now;
					decTime = (long)((t2 - t1).TotalMilliseconds);
				}
			}

			using (WorkingDir wd = new WorkingDir())
			using (FileCipher transformer = new FileCipher(rawKey))
			{
				string testDataFile = wd.MakePath();
				string encDataFile = wd.MakePath();

				File.WriteAllBytes(testDataFile, testData);
				File.WriteAllBytes(encDataFile, encData);

				// ファイル版・暗号化
				{
					DateTime t1 = DateTime.Now;
					transformer.Encrypt(testDataFile);
					DateTime t2 = DateTime.Now;
					encTime2 = (long)((t2 - t1).TotalMilliseconds);
				}

				// ファイル版・復号
				{
					DateTime t1 = DateTime.Now;
					transformer.Decrypt(encDataFile);
					DateTime t2 = DateTime.Now;
					decTime2 = (long)((t2 - t1).TotalMilliseconds);
				}
			}

			ProcMain.WriteLog(encData.Length);
			ProcMain.WriteLog(decData.Length);
			ProcMain.WriteLog(encTime);
			ProcMain.WriteLog(decTime);
			ProcMain.WriteLog(encTime2);
			ProcMain.WriteLog(decTime2);

			Writer.WriteRow(new string[]
			{
				"" + keySize,
				"" + dataSize,
				"" + encData.Length,
				"" + encTime,
				"" + decTime,
				"" + encTime2,
				"" + decTime2,
			});

			DateTime edTm = DateTime.Now;

			ProcMain.WriteLog((edTm - stTm).TotalMilliseconds);
		}
	}
}
