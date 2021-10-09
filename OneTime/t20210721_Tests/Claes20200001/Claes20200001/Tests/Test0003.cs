using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using System.IO;

namespace Charlotte.Tests
{
	public class Test0003
	{
		public void Test01()
		{
			// "X" < "x-" < "-x" < "X" ???

			Console.WriteLine("X".CompareTo("x-")); // 1
			Console.WriteLine("x-".CompareTo("-x")); // -1
			Console.WriteLine("-x".CompareTo("X")); // -1

			// --> "X" > "x-" < "-x" < "X" -- 問題無し

			Common.Pause();
		}

		public void Test02()
		{
			try
			{
				using (new Test02_a())
				{
					throw new Exception("using_Test02_a");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e); // Test02_a_Dispose のスタックトレースのみ -- using_Test02_a は捨てられる。
			}
			Common.Pause();
		}

		private class Test02_a : IDisposable
		{
			public void Dispose()
			{
				throw new Exception("Test02_a_Dispose");
			}
		}

		public void Test03()
		{
			string[] parts = SCommon.ParseEnclosed("このタグはテキストを<strong>強調</strong>する際に・・・", "<STRong>", "</strONG>", true);

			foreach (string part in parts)
				Console.WriteLine("part: " + part);

			Common.Pause();
		}

		public void Test04()
		{
			UInt16[] chrs_A = SCommon.GetJCharCodes().ToArray();
			UInt16[] chrs_B = SCommon.GetJChars().ToCharArray()
				.Select(chr => SCommon.ENCODING_SJIS.GetBytes(new string(new char[] { chr })))
				.Select(bytes => (UInt16)(bytes[0] << 8 | bytes[1]))
				.ToArray();

			if (chrs_A.Length != chrs_B.Length)
				throw null; // never

			int count = chrs_A.Length;

			for (int index = 0; index < count; index++)
			{
				if (chrs_A[index] != chrs_B[index])
				{
					Console.WriteLine(
						((int)chrs_A[index]).ToString("x") + " -> " +
						((int)chrs_B[index]).ToString("x") + " == " +
						SCommon.GetJCharCodes().Contains(chrs_B[index]));

					if (!SCommon.GetJCharCodes().Contains(chrs_B[index]))
						throw null; // 想定外
				}
			}
			Common.Pause();
		}

		public void Test04_2()
		{
			UInt16[] chrs_A = SCommon.GetJCharCodes().ToArray();
			UInt16[] chrs_B = SCommon.GetJChars().ToCharArray()
				.Select(chr => Encoding.UTF8.GetBytes(new string(new char[] { chr })))
				.Select(bytes => Encoding.UTF8.GetString(bytes)[0])
				.Select(chr => SCommon.ENCODING_SJIS.GetBytes(new string(new char[] { chr })))
				.Select(bytes => (UInt16)(bytes[0] << 8 | bytes[1]))
				.ToArray();

			if (chrs_A.Length != chrs_B.Length)
				throw null; // never

			int count = chrs_A.Length;

			for (int index = 0; index < count; index++)
			{
				if (chrs_A[index] != chrs_B[index])
				{
					Console.WriteLine(
						((int)chrs_A[index]).ToString("x") + " -> " +
						((int)chrs_B[index]).ToString("x") + " == " +
						SCommon.GetJCharCodes().Contains(chrs_B[index]));

					if (!SCommon.GetJCharCodes().Contains(chrs_B[index]))
						throw null; // 想定外
				}
			}
			Common.Pause();
		}

		public void Test05()
		{
			List<string> dest = new List<string>();

			foreach (UInt16 code in SCommon.GetJCharCodes())
			{
				char[] chrs = SCommon.ENCODING_SJIS.GetString(new byte[] { (byte)(code >> 8), (byte)(code & 0xff) }).ToArray();

				if (chrs.Length != 1)
					throw null;

				char chr = chrs[0];

				if ((int)chr < 1 || 65535 < (int)chr)
					throw null;

				// 逆変換
				{
					byte[] bytes = SCommon.ENCODING_SJIS.GetBytes(new string(new char[] { chr }));

					if (bytes.Length != 2)
						throw null;

					UInt16 revCode = (UInt16)(((int)bytes[0] << 8) | bytes[1]);

					if (revCode != code) // 対応しないのはある。
						if (!SCommon.GetJCharCodes().Contains(revCode)) // JChar に戻れば良しとする。
							throw null;
				}

				dest.Add("this.Add(" + (int)code + ", " + (int)chr + ");");
			}

			// 連結して行数を減らす。
			for (int c = 0; c < 3; c++)
			{
				for (int index = 0; index + 1 < dest.Count; index++)
				{
					dest[index] = dest[index] + " " + dest[index + 1];
					dest.RemoveAt(index + 1);
				}
			}

			File.WriteAllLines(@"C:\temp\SJIS_Unicode.txt", dest, Encoding.ASCII);

			// ----

			for (int code = 0xa1; code <= 0xdf; code++) // 半角カナ
			{
				char[] chrs = SCommon.ENCODING_SJIS.GetString(new byte[] { (byte)code }).ToArray();

				if (chrs.Length != 1)
					throw null;

				char chr = chrs[0];

				Console.WriteLine(code + " ==> " + (int)chr + " " + ((int)chr - code));

				// 半角カナ(SJIS) -> Unicode は 65216 足すだけで良いっぽい。

				if ((int)chr != code + 65216)
					throw null;

				// 逆変換
				{
					byte[] bytes = SCommon.ENCODING_SJIS.GetBytes(new string(new char[] { chr }));

					if (bytes.Length != 1)
						throw null;

					UInt16 revCode = (UInt16)bytes[0];

					if (revCode != code)
						throw null;
				}
			}
			Common.Pause();
		}

		public void Test05_2()
		{
			List<string> dest = new List<string>();

			foreach (UInt16 code in SCommon.GetJCharCodes())
			{
				char[] chrs = SCommon.ENCODING_SJIS.GetString(new byte[] { (byte)(code >> 8), (byte)(code & 0xff) }).ToArray();

				if (chrs.Length != 1)
					throw null;

				char chr = chrs[0];

				dest.Add(((int)code).ToString("x4"));
				dest.Add(((int)chr).ToString("x4"));
			}

			// 連結して行数を減らす。
			for (int c = 0; c < 6; c++)
			{
				for (int index = 0; index + 1 < dest.Count; index++)
				{
					dest[index] = dest[index] + dest[index + 1];
					dest.RemoveAt(index + 1);
				}
			}

			File.WriteAllLines(@"C:\temp\SJIS_Unicode.txt", dest, Encoding.ASCII);
		}
	}
}
