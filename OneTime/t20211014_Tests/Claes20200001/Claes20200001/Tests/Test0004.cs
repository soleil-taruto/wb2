using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Tests
{
	public class Test0004
	{
		public void Test01()
		{
			XMLNode.LoadFromFile(@"C:\temp\1.xml").WriteToFile(@"C:\temp\2.xml");
			XMLNode.LoadFromFile(@"C:\temp\2.xml").WriteToFile(@"C:\temp\3.xml");
		}
	}
}
