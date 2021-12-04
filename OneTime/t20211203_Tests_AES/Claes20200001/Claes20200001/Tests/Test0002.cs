using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.SubCommons;

namespace Charlotte.Tests
{
	/// <summary>
	/// 主として RingCipher.cs のテストを行う。
	/// </summary>
	public class Test0002
	{
		public void Test01()
		{
			for (int size = 0; size < 100; size++)
				for (int testcnt = 0; testcnt < 100; testcnt++)
					Test01_a(size);

			for (int testcnt = 0; testcnt < 300; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(100, 3000));

			for (int testcnt = 0; testcnt < 100; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(3000, 100000));

			for (int testcnt = 0; testcnt < 30; testcnt++)
				Test01_a(SCommon.CRandom.GetRange(100000, 3000000));

			ProcMain.WriteLog("OK!");
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

			if (testData.Length == encData.Length) // 平文と暗号文は少なくとも長さは違うはず
				throw null;

			if (SCommon.Comp(testData, decData) != 0) // ? 平文と復号した平文の不一致
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

		public void Test02()
		{
			for (int testcnt = 0; testcnt < 100; testcnt++)
			{
				Test02_b();
				Test02_c();
			}
			ProcMain.WriteLog("OK!");
		}

		private void Test02_b()
		{
			byte[] rawKey = SCommon.CRandom.GetBytes(48);
			byte[] testData = SCommon.CRandom.GetBytes(10);
			byte[] encData;
			byte[] decData;

			using (RingCipher transformer = new RingCipher(rawKey))
			{
				encData = transformer.Encrypt(testData);
			}

			PrintHead(rawKey);
			PrintHead(testData);
			PrintHead(encData);

			// 自力で復号
			{
				byte[] data = SCommon.GetSubBytes(encData, 0, encData.Length);

				if (data.Length != 160) // 平文_10 + padding_6 + cRandPart_64 + hash_64 + cRandPart_16 --> 160
					throw null;

				byte[] data_01 = SCommon.GetSubBytes(data, 16 * 0, 16);
				byte[] data_02 = SCommon.GetSubBytes(data, 16 * 1, 16);
				byte[] data_03 = SCommon.GetSubBytes(data, 16 * 2, 16);
				byte[] data_04 = SCommon.GetSubBytes(data, 16 * 3, 16);
				byte[] data_05 = SCommon.GetSubBytes(data, 16 * 4, 16);
				byte[] data_06 = SCommon.GetSubBytes(data, 16 * 5, 16);
				byte[] data_07 = SCommon.GetSubBytes(data, 16 * 6, 16);
				byte[] data_08 = SCommon.GetSubBytes(data, 16 * 7, 16);
				byte[] data_09 = SCommon.GetSubBytes(data, 16 * 8, 16);
				byte[] data_10 = SCommon.GetSubBytes(data, 16 * 9, 16);

				data = null; // もう使わん

				byte[] tmp = new byte[16];

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 32, 16)))
				{
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_10[index]);
				}

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 0, 32)))
				{
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_10[index]);
				}

				byte[] data_A = new byte[80];
				byte[] hash_B = new byte[64];
				byte[] data_C = new byte[16];

				Array.Copy(data_01, 0, data_A, 16 * 0, 16); // 平文_10 + padding_6
				Array.Copy(data_02, 0, data_A, 16 * 1, 16); // cRandPart_64(1)
				Array.Copy(data_03, 0, data_A, 16 * 2, 16); // cRandPart_64(2)
				Array.Copy(data_04, 0, data_A, 16 * 3, 16); // cRandPart_64(3)
				Array.Copy(data_05, 0, data_A, 16 * 4, 16); // cRandPart_64(4)
				Array.Copy(data_06, 0, hash_B, 16 * 0, 16); // hash_64(1)
				Array.Copy(data_07, 0, hash_B, 16 * 1, 16); // hash_64(2)
				Array.Copy(data_08, 0, hash_B, 16 * 2, 16); // hash_64(3)
				Array.Copy(data_09, 0, hash_B, 16 * 3, 16); // hash_64(4)
				Array.Copy(data_10, 0, data_C, 16 * 0, 16); // cRandPart_16

				data_C = null; // もう使わん

				byte[] hash_A = SCommon.GetSHA512(data_A);

				if (SCommon.Comp(hash_A, hash_B) != 0) // ? ハッシュの不一致
					throw null;

				data_A = null; // もう使わん
				hash_A = null; // もう使わん
				hash_B = null; // もう使わん

				int size = data_01[15] & 0x0f;

				if (size != 5) // パディング長 - 1
					throw null;

				decData = SCommon.GetSubBytes(data_01, 0, 10);
			}

			PrintHead(decData);

			if (SCommon.Comp(testData, decData) != 0) // ? 平文と復号した平文の不一致
				throw null;

			ProcMain.WriteLog("OK");
		}

		private void Test02_c()
		{
			byte[] rawKey = SCommon.CRandom.GetBytes(104);
			byte[] testData = SCommon.CRandom.GetBytes(50);
			byte[] encData;
			byte[] decData;

			using (RingCipher transformer = new RingCipher(rawKey))
			{
				encData = transformer.Encrypt(testData);
			}

			PrintHead(rawKey);
			PrintHead(testData);
			PrintHead(encData);

			// 自力で復号
			{
				byte[] data = SCommon.GetSubBytes(encData, 0, encData.Length);

				if (data.Length != 208) // 平文_50 + padding_14 + cRandPart_64 + hash_64 + cRandPart_16 --> 208
					throw null;

				byte[] data_01 = SCommon.GetSubBytes(data, 16 * 0, 16);
				byte[] data_02 = SCommon.GetSubBytes(data, 16 * 1, 16);
				byte[] data_03 = SCommon.GetSubBytes(data, 16 * 2, 16);
				byte[] data_04 = SCommon.GetSubBytes(data, 16 * 3, 16);
				byte[] data_05 = SCommon.GetSubBytes(data, 16 * 4, 16);
				byte[] data_06 = SCommon.GetSubBytes(data, 16 * 5, 16);
				byte[] data_07 = SCommon.GetSubBytes(data, 16 * 6, 16);
				byte[] data_08 = SCommon.GetSubBytes(data, 16 * 7, 16);
				byte[] data_09 = SCommon.GetSubBytes(data, 16 * 8, 16);
				byte[] data_10 = SCommon.GetSubBytes(data, 16 * 9, 16);
				byte[] data_11 = SCommon.GetSubBytes(data, 16 * 10, 16);
				byte[] data_12 = SCommon.GetSubBytes(data, 16 * 11, 16);
				byte[] data_13 = SCommon.GetSubBytes(data, 16 * 12, 16);

				data = null; // もう使わん

				byte[] tmp = new byte[16];

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 88, 16)))
				{
					transformer.DecryptBlock(data_13, tmp); for (int index = 0; index < 16; index++) data_13[index] = (byte)(tmp[index] ^ data_12[index]);
					transformer.DecryptBlock(data_12, tmp); for (int index = 0; index < 16; index++) data_12[index] = (byte)(tmp[index] ^ data_11[index]);
					transformer.DecryptBlock(data_11, tmp); for (int index = 0; index < 16; index++) data_11[index] = (byte)(tmp[index] ^ data_10[index]);
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_13[index]);
				}

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 64, 24)))
				{
					transformer.DecryptBlock(data_13, tmp); for (int index = 0; index < 16; index++) data_13[index] = (byte)(tmp[index] ^ data_12[index]);
					transformer.DecryptBlock(data_12, tmp); for (int index = 0; index < 16; index++) data_12[index] = (byte)(tmp[index] ^ data_11[index]);
					transformer.DecryptBlock(data_11, tmp); for (int index = 0; index < 16; index++) data_11[index] = (byte)(tmp[index] ^ data_10[index]);
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_13[index]);
				}

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 32, 32)))
				{
					transformer.DecryptBlock(data_13, tmp); for (int index = 0; index < 16; index++) data_13[index] = (byte)(tmp[index] ^ data_12[index]);
					transformer.DecryptBlock(data_12, tmp); for (int index = 0; index < 16; index++) data_12[index] = (byte)(tmp[index] ^ data_11[index]);
					transformer.DecryptBlock(data_11, tmp); for (int index = 0; index < 16; index++) data_11[index] = (byte)(tmp[index] ^ data_10[index]);
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_13[index]);
				}

				using (AESCipher transformer = new AESCipher(SCommon.GetSubBytes(rawKey, 0, 32)))
				{
					transformer.DecryptBlock(data_13, tmp); for (int index = 0; index < 16; index++) data_13[index] = (byte)(tmp[index] ^ data_12[index]);
					transformer.DecryptBlock(data_12, tmp); for (int index = 0; index < 16; index++) data_12[index] = (byte)(tmp[index] ^ data_11[index]);
					transformer.DecryptBlock(data_11, tmp); for (int index = 0; index < 16; index++) data_11[index] = (byte)(tmp[index] ^ data_10[index]);
					transformer.DecryptBlock(data_10, tmp); for (int index = 0; index < 16; index++) data_10[index] = (byte)(tmp[index] ^ data_09[index]);
					transformer.DecryptBlock(data_09, tmp); for (int index = 0; index < 16; index++) data_09[index] = (byte)(tmp[index] ^ data_08[index]);
					transformer.DecryptBlock(data_08, tmp); for (int index = 0; index < 16; index++) data_08[index] = (byte)(tmp[index] ^ data_07[index]);
					transformer.DecryptBlock(data_07, tmp); for (int index = 0; index < 16; index++) data_07[index] = (byte)(tmp[index] ^ data_06[index]);
					transformer.DecryptBlock(data_06, tmp); for (int index = 0; index < 16; index++) data_06[index] = (byte)(tmp[index] ^ data_05[index]);
					transformer.DecryptBlock(data_05, tmp); for (int index = 0; index < 16; index++) data_05[index] = (byte)(tmp[index] ^ data_04[index]);
					transformer.DecryptBlock(data_04, tmp); for (int index = 0; index < 16; index++) data_04[index] = (byte)(tmp[index] ^ data_03[index]);
					transformer.DecryptBlock(data_03, tmp); for (int index = 0; index < 16; index++) data_03[index] = (byte)(tmp[index] ^ data_02[index]);
					transformer.DecryptBlock(data_02, tmp); for (int index = 0; index < 16; index++) data_02[index] = (byte)(tmp[index] ^ data_01[index]);
					transformer.DecryptBlock(data_01, tmp); for (int index = 0; index < 16; index++) data_01[index] = (byte)(tmp[index] ^ data_13[index]);
				}

				byte[] data_A = new byte[128];
				byte[] hash_B = new byte[64];
				byte[] data_C = new byte[16];

				Array.Copy(data_01, 0, data_A, 16 * 0, 16); // 平文_50(1)
				Array.Copy(data_02, 0, data_A, 16 * 1, 16); // 平文_50(2)
				Array.Copy(data_03, 0, data_A, 16 * 2, 16); // 平文_50(3)
				Array.Copy(data_04, 0, data_A, 16 * 3, 16); // 平文_50(4) + padding_14
				Array.Copy(data_05, 0, data_A, 16 * 4, 16); // cRandPart_64(1)
				Array.Copy(data_06, 0, data_A, 16 * 5, 16); // cRandPart_64(2)
				Array.Copy(data_07, 0, data_A, 16 * 6, 16); // cRandPart_64(3)
				Array.Copy(data_08, 0, data_A, 16 * 7, 16); // cRandPart_64(4)
				Array.Copy(data_09, 0, hash_B, 16 * 0, 16); // hash_64(1)
				Array.Copy(data_10, 0, hash_B, 16 * 1, 16); // hash_64(2)
				Array.Copy(data_11, 0, hash_B, 16 * 2, 16); // hash_64(3)
				Array.Copy(data_12, 0, hash_B, 16 * 3, 16); // hash_64(4)
				Array.Copy(data_13, 0, data_C, 16 * 0, 16); // cRandPart_16

				data_C = null; // もう使わん

				byte[] hash_A = SCommon.GetSHA512(data_A);

				if (SCommon.Comp(hash_A, hash_B) != 0) // ? ハッシュの不一致
					throw null;

				data_A = null; // もう使わん
				hash_A = null; // もう使わん
				hash_B = null; // もう使わん

				int size = data_04[15] & 0x0f;

				if (size != 13) // パディング長 - 1
					throw null;

				decData = SCommon.Join(new byte[][]
				{
					data_01,
					data_02,
					data_03,
					SCommon.GetSubBytes(data_04, 0, 2),
				});
			}

			PrintHead(decData);

			if (SCommon.Comp(testData, decData) != 0) // ? 平文と復号した平文の不一致
				throw null;

			ProcMain.WriteLog("OK");
		}
	}
}
