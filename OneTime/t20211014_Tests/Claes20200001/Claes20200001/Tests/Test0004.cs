using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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

		public void Test03()
		{
			Canvas canvas = new Canvas(600, 400);

			canvas.Fill(new I4Color(255, 200, 100, 255));
			canvas.DrawString("CANVAS", 500, "Impact", FontStyle.Bold, new I3Color(0, 128, 255), new I4Rect(100, 100, 400, 200), 5);

			canvas.Save(Common.NextOutputPath() + ".png");

			// ----

			canvas = new Canvas(1000, 500);

			canvas.Fill(new I4Color(0, 0, 0, 0));
			canvas.FillRect(new I4Color(127, 127, 127, 255), new I4Rect(0, 100, 1000, 300));
			canvas.FillRect(new I4Color(127, 127, 127, 255), new I4Rect(100, 0, 800, 500));
			canvas.FillCircle(new I4Color(127, 127, 127, 255), new I2Point(100, 100), 100);
			canvas.FillCircle(new I4Color(127, 127, 127, 255), new I2Point(900, 100), 100);
			canvas.FillCircle(new I4Color(127, 127, 127, 255), new I2Point(900, 399), 100);
			canvas.FillCircle(new I4Color(127, 127, 127, 255), new I2Point(100, 399), 100);

			canvas.FillRect(new I4Color(255, 255, 127, 255), new I4Rect(50, 100, 900, 300));
			canvas.FillRect(new I4Color(255, 255, 127, 255), new I4Rect(100, 50, 800, 400));
			canvas.FillCircle(new I4Color(255, 255, 127, 255), new I2Point(100, 100), 50);
			canvas.FillCircle(new I4Color(255, 255, 127, 255), new I2Point(900, 100), 50);
			canvas.FillCircle(new I4Color(255, 255, 127, 255), new I2Point(900, 399), 50);
			canvas.FillCircle(new I4Color(255, 255, 127, 255), new I2Point(100, 399), 50);

			canvas.DrawString("CANVAS", 300, "Impact", FontStyle.Bold, new I3Color(0, 128, 255), new I4Rect(100, 100, 800, 300), 5);

			canvas = canvas.Expand(200, 100);

			canvas.Save(Common.NextOutputPath() + ".png");
		}
	}
}
