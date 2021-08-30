using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
