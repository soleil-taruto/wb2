using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0004
	{
		public void Test01()
		{
			//Console.WriteLine(Path.GetFullPath(null)); // 例外 -- 値を Null にすることはできません。
			//Console.WriteLine(Path.GetFullPath("")); // 例外 -- パスの形式が無効です。
			//Console.WriteLine(Path.GetFullPath(" ")); // 例外
			//Console.WriteLine(Path.GetFullPath("\t")); // 例外
			//Console.WriteLine(Path.GetFullPath(" \t")); // 例外
			//Console.WriteLine(Path.GetFullPath(" \t ")); // 例外
			//Console.WriteLine(Path.GetFullPath("\t ")); // 例外
			//Console.WriteLine(Path.GetFullPath("\r")); // 例外
			//Console.WriteLine(Path.GetFullPath("\n")); // 例外
			//Console.WriteLine(Path.GetFullPath("\r\n")); // 例外
			//Console.WriteLine(Path.GetFullPath("ttttt\tttttt")); // 例外 -- パスに無効な文字が含まれています。
			//Console.WriteLine(Path.GetFullPath("rrrrr\rrrrrr")); // 例外
			//Console.WriteLine(Path.GetFullPath("nnnnn\nnnnnn")); // 例外
			//Console.WriteLine(Path.GetFullPath("ttttt\nrrrrr\rrrrrrnnnnn\nxxxxx")); // 例外
			Console.WriteLine(Path.GetFullPath("　"));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug\　"
			Console.WriteLine(Path.GetFullPath("."));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug"
			Console.WriteLine(Path.GetFullPath("\\"));
			// --> @"C:\"
			Console.WriteLine(Path.GetFullPath(".\\"));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug\"
			Console.WriteLine(Path.GetFullPath(".\\."));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug"

			Common.Pause();
		}

		public void Test02()
		{
			//Console.WriteLine(SCommon.MakeFullPath(null)); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath(" ")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("\t")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath(" \t")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath(" \t ")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("\t ")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("\r")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("\n")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("\r\n")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("ttttt\tttttt")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("rrrrr\rrrrrr")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("nnnnn\nnnnnn")); // 例外
			//Console.WriteLine(SCommon.MakeFullPath("ttttt\nrrrrr\rrrrrrnnnnn\nxxxxx")); // 例外
			Console.WriteLine(SCommon.MakeFullPath("　"));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug\　"
			Console.WriteLine(SCommon.MakeFullPath("."));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug"
			Console.WriteLine(SCommon.MakeFullPath("\\"));
			// --> @"C:\"
			Console.WriteLine(SCommon.MakeFullPath(".\\"));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug"
			Console.WriteLine(SCommon.MakeFullPath(".\\."));
			// --> @"C:\Dev\wb2\OneTime\t20210721_Tests\Claes20200001\Claes20200001\bin\Debug"

			Common.Pause();
		}
	}
}
