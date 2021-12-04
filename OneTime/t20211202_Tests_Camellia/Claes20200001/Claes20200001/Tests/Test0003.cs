using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Camellias;
using System.IO;

namespace Charlotte.Tests
{
	public class Test0003
	{
		public void Test01()
		{
			for (int size = 0; size < 100; size++)
				Test01_a(size);

			for (int testcnt = 0; testcnt < 300; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(100, 1000));

			for (int testcnt = 0; testcnt < 100; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(1000, 10000));

			for (int testcnt = 0; testcnt < 30; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(10000, 100000));

			for (int testcnt = 0; testcnt < 10; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(100000, 1000000));
		}

		private void Test01_a(int size)
		{
			byte[] rawKey = MakeRawKey();
			byte[] testData = MakeTestData(size);
			byte[] encData;
			byte[] decData;

			using (RingCipher transformer = new RingCipher(rawKey))
			{
				encData = transformer.Encrypt(testData);
				decData = transformer.Decrypt(encData);
			}

			ProcMain.WriteLog("K " + rawKey.Length);
			ProcMain.WriteLog("T " + testData.Length);
			ProcMain.WriteLog("E " + encData.Length);
			ProcMain.WriteLog("D " + decData.Length);

			PrintHead(rawKey);
			PrintHead(testData);
			PrintHead(encData);
			PrintHead(decData);

			byte[] encData2;
			byte[] decData2;

			using (WorkingDir wd = new WorkingDir())
			using (FileCipher transformer = new FileCipher(rawKey))
			{
				string file = wd.MakePath();
				File.WriteAllBytes(file, testData);
				transformer.Encrypt(file);
				encData2 = File.ReadAllBytes(file);
				transformer.Decrypt(file);
				decData2 = File.ReadAllBytes(file);
			}

			ProcMain.WriteLog("e " + encData2.Length);
			ProcMain.WriteLog("d " + decData2.Length);

			PrintHead(encData2);
			PrintHead(decData2);

			// 2bs
			if (testData.Length == encData.Length) // 平文と暗号文は少なくとも長さは違うはず
				throw null;

			// 2bs
			if (SCommon.Comp(testData, decData) != 0) // ? 平文と復号した平文の不一致
				throw null;

			// 暗号文はcRandPartが異なるため(ほぼ確実に)一致しない。

			if (encData.Length != encData2.Length) // ? 暗号文の長さの不一致
				throw null;

			if (SCommon.Comp(decData, decData2) != 0) // ? 平文の不一致
				throw null;
		}

		private static byte[] MakeRawKey()
		{
			int size = SCommon.CRandom.ChooseOne(new int[] { 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 128 });
			return SCommon.CRandom.GetBytes(size);
		}

		private static byte[] MakeTestData(int size)
		{
			return SCommon.CRandom.GetBytes(size);
		}

		private static void PrintHead(byte[] data)
		{
			const int HEAD_SIZE = 38;
			bool cutFlag;

			if (HEAD_SIZE < data.Length)
			{
				data = SCommon.GetSubBytes(data, 0, HEAD_SIZE);
				cutFlag = true;
			}
			else
				cutFlag = false;

			Console.WriteLine(SCommon.Hex.ToString(data) + (cutFlag ? "..." : ""));
		}
	}
}
