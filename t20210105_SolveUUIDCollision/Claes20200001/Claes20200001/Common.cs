using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte
{
	public static class Common
	{
		public static string EraseStartEnd(string str, string startPtn, string endPtn)
		{
			if (!str.StartsWith(startPtn))
				return null;

			str = str.Substring(startPtn.Length);

			if (!str.EndsWith(endPtn))
				return null;

			str = str.Substring(0, str.Length - endPtn.Length);
			return str;
		}

		/// <summary>
		/// パス文字列を比較する。
		/// 同じフォルダ内のローカル名が辞書順になるようにする。
		/// </summary>
		/// <param name="a">パス文字列_A</param>
		/// <param name="b">パス文字列_B</param>
		/// <returns>比較結果</returns>
		public static int CompPath(string a, string b)
		{
			a = a.Replace('\\', '\t');
			b = b.Replace('\\', '\t');

			return SCommon.CompIgnoreCase(a, b);
		}
	}
}
