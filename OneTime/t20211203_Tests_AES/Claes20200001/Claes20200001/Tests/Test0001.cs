using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Charlotte.Commons;
using Charlotte.SubCommons;

namespace Charlotte.Tests
{
	/// <summary>
	/// 主として AESCipher.cs のテストを行う。
	/// </summary>
	public class Test0001
	{
		private const string TEST_VECTOR_FILE = @"..\..\..\..\dat\testvector\t_aes128.txt";

		private class TestCase
		{
			public byte[] Key;
			public byte[] Plain;
			public byte[] Cipher;
		}

		public void Test01()
		{
			string[] lines = File.ReadAllLines(TEST_VECTOR_FILE, Encoding.ASCII);
			byte[] currKey = null;
			byte[] currPlain = null;

			List<TestCase> testCases = new List<TestCase>();

			foreach (string line in lines)
			{
				int colon = line.IndexOf(':');

				if (colon == -1)
					continue;

				string strData = line.Substring(colon + 1);
				strData = strData.Replace(" ", "");
				byte[] data = SCommon.Hex.ToBytes(strData);

				if (line[0] == 'K')
				{
					currKey = data;
				}
				else if (line[0] == 'P')
				{
					currPlain = data;
				}
				else if (line[0] == 'C')
				{
					testCases.Add(new TestCase()
					{
						Key = currKey,
						Plain = currPlain,
						Cipher = data,
					});
				}
			}

			byte[] block = new byte[16];

			foreach (TestCase testCase in testCases)
			{
				ProcMain.WriteLog("K " + SCommon.Hex.ToString(testCase.Key));
				ProcMain.WriteLog("P " + SCommon.Hex.ToString(testCase.Plain));
				ProcMain.WriteLog("C " + SCommon.Hex.ToString(testCase.Cipher));

				using (AESCipher transformer = new AESCipher(testCase.Key))
				{
					transformer.EncryptBlock(testCase.Plain, block);

					if (SCommon.Comp(testCase.Cipher, block) != 0)
						throw null; // テスト失敗：ブロック暗号化 出力 不一致

					transformer.DecryptBlock(testCase.Cipher, block);

					if (SCommon.Comp(testCase.Plain, block) != 0)
						throw null; // テスト失敗：ブロック復号 出力 不一致
				}
			}

			ProcMain.WriteLog("OK!");
		}
	}
}
