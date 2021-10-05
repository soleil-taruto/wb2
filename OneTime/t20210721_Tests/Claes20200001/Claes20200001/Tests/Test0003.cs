using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

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
	}
}
