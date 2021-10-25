using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0005
	{
		public void Test01()
		{
			Test01_a(new string[] { });
			Test01_a(new string[] { "" });
			Test01_a(new string[] { "ABC" });
			Test01_a(new string[] { "ABC", "" });
			Test01_a(new string[] { "", "123" });
			Test01_a(new string[] { "ABC", "123" });
			Test01_a(new string[] { "ABC", "123", "abcdef" });
		}

		private void Test01_a(string[] strs)
		{
			string enc = SCommon.Serializer.I.Join(strs);
			string[] dec = SCommon.Serializer.I.Split(enc);

			if (SCommon.Comp(strs, dec, SCommon.Comp) != 0)
				throw null;
		}
	}
}
