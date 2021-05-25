using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void Test01()
		{
			Test01_a("AABBCCABCCBACCBBAA", "ABC");
			Test01_a("54321", "12345");
			Test01_a("", "");
		}

		private void Test01_a(string src, string expect)
		{
			string ans = new string(src.ToArray().Distinct((a, b) => (int)a - (int)b).ToArray());

			if (ans != expect)
				throw null; // BUG !!!
		}
	}
}
