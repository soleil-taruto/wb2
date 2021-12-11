using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0007
	{
		public void Test01()
		{
			Test01_a(new byte[0]);

			for (int testcnt = 0; testcnt < 1000; testcnt++)
			{
				Test01_a(SCommon.CRandom.GetBytes(SCommon.CRandom.GetRange(1, 1000)));
			}

			Console.WriteLine("OK!");
			Common.Pause();
		}

		private void Test01_a(byte[] testData)
		{
			string base64 = SCommon.Base64.I.Encode(testData);
			string base64url = SCommon.Base64.I.Encode(testData)
				.Replace("=", "")
				.Replace('+', '-')
				.Replace('/', '_'); // Base64 Encode -> Base64 URL Encode

			Console.WriteLine(base64); // cout
			Console.WriteLine(base64url); // cout

			byte[] decBase64 = SCommon.Base64.I.Decode(base64);
			byte[] decBase64url = SCommon.Base64.I.Decode(base64url);

			if (SCommon.Comp(testData, decBase64) != 0)
				throw null;

			if (SCommon.Comp(testData, decBase64url) != 0)
				throw null;
		}
	}
}
