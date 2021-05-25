using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte
{
	public static class Common
	{
		public static string ZPad(string str, int minlen, char padding = '0')
		{
			while (str.Length < minlen)
			{
				str = padding + str;
			}
			return str;
		}
	}
}
