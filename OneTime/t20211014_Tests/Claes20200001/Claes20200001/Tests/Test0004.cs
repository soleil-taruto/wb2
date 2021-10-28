using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.SubCommons;

namespace Charlotte.Tests
{
	public class Test0004
	{
		public void Test01()
		{
			XMLNode.LoadFromFile(@"C:\temp\1.xml").WriteToFile(@"C:\temp\2.xml");
			XMLNode.LoadFromFile(@"C:\temp\2.xml").WriteToFile(@"C:\temp\3.xml");
		}

		public void Test02()
		{
			JsonNode.LoadFromFile(@"C:\temp\1.json").WriteToFile(@"C:\temp\2.json");
			JsonNode.ShortMode = true;
			JsonNode.LoadFromFile(@"C:\temp\2.json").WriteToFile(@"C:\temp\3.json");
			JsonNode.ShortMode = false;
			JsonNode.LoadFromFile(@"C:\temp\3.json").WriteToFile(@"C:\temp\4.json");
		}
	}
}
