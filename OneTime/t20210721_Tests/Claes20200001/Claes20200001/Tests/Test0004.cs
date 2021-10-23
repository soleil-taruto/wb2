using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Charlotte.Tests
{
	public class Test0004
	{
		public void Test01()
		{
			//Console.WriteLine(Path.GetFullPath(null)); // 例外
			//Console.WriteLine(Path.GetFullPath("")); // 例外
			//Console.WriteLine(Path.GetFullPath(" ")); // 例外
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
	}
}
