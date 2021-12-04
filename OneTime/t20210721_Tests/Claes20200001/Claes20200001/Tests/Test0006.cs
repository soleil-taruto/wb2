using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0006
	{
		public void Test01()
		{
			for (int testcnt = 0; testcnt < 10000; testcnt++)
			{
				Test01_a(false);
				Test01_a(true);
			}

			Console.WriteLine("OK!");
			Common.Pause();
		}

		private void Test01_a(bool allowZeroByte)
		{
			int testDataSize = SCommon.CRandom.GetRange(0, 10000);
			byte[] testData = SCommon.CRandom.GetBytes(testDataSize);

			int p = 0;
			SCommon.Read_d reader = (buff, offset, count) =>
			{
				int s = Math.Min(SCommon.CRandom.GetRange(allowZeroByte ? 0 : 1, count * 2), count);
				Array.Copy(testData, p, buff, offset, s);
				p += s;
				return s;
			};

			byte[] ret = ReadFixedSize(reader, allowZeroByte, testDataSize);

			if (SCommon.Comp(ret, testData) != 0)
				throw null;
		}

		// ---- ★★★ ReadFixedSize ここから ----

		public static byte[] ReadFixedSize(SCommon.Read_d reader, bool allowZeroByte, int size)
		{
			byte[] buff = new byte[size];
			ReadFixedSize(reader, allowZeroByte, buff);
			return buff;
		}

		public static void ReadFixedSize(SCommon.Read_d reader, bool allowZeroByte, byte[] buff, int offset = 0)
		{
			ReadFixedSize(reader, allowZeroByte, buff, offset, buff.Length - offset);
		}

		public static void ReadFixedSize(SCommon.Read_d reader, bool allowZeroByte, byte[] buff, int offset, int count)
		{
			while (1 <= count)
			{
				int readSize = reader(buff, offset, count);

				if (readSize < (allowZeroByte ? 0 : 1) || count < readSize)
					throw new Exception("Bad readSize: " + readSize);

				offset += readSize;
				count -= readSize;
			}
		}

		// ---- ★★★ ReadFixedSize ここまで ----
	}
}
