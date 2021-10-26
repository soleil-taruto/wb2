using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using System.IO;

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

			Common.Pause();
		}

		public void Test02()
		{
			Test02_a(
				SCommon.Hex.ToBytes("efbbbf3132330d0a"), // BOM + "123" + 改行 (UTF-8)
				Encoding.UTF8
				);

			Test02_a(
				SCommon.Hex.ToBytes("fffe3100320033000d000a00"), // BOM + "123" + 改行 (UTF-16_LE)
				Encoding.Unicode
				);
			Test02_a(
				SCommon.Hex.ToBytes("fffe3100320033000d000a00"), // BOM + "123" + 改行 (UTF-16_LE)
				Encoding.BigEndianUnicode
				);
			Test02_a(
				SCommon.Hex.ToBytes("feff003100320033000d000a"), // BOM + "123" + 改行 (UTF-16_BE)
				Encoding.Unicode
				);
			Test02_a(
				SCommon.Hex.ToBytes("feff003100320033000d000a"), // BOM + "123" + 改行 (UTF-16_BE)
				Encoding.BigEndianUnicode
				);

			Test02_a(
				SCommon.Hex.ToBytes("fffe00003100000032000000330000000d0000000a000000"), // BOM + "123" + 改行 (UTF-32_LE)
				Encoding.UTF32
				);
			Test02_a(
				SCommon.Hex.ToBytes("0000feff0000003100000032000000330000000d0000000a"), // BOM + "123" + 改行 (UTF-32_BE)
				Encoding.UTF32
				);

			Common.Pause();
		}

		private void Test02_a(byte[] data, Encoding encoding)
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string file = wd.MakePath();
				File.WriteAllBytes(file, data);
				string str = File.ReadAllText(file, encoding);
				ShowStringChars(str, "ファイル経由あり");
			}

			{
				string str = encoding.GetString(data);
				ShowStringChars(str, "ファイル経由なし");
			}
		}

		private void ShowStringChars(string str, string title)
		{
			Console.WriteLine(title + " ⇒ " + string.Join(", ", str.ToCharArray().Select(v => "" + (int)v)));
		}
	}
}
