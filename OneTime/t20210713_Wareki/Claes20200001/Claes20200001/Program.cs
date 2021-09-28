using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Charlotte.Commons;
using Charlotte.Tests;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ProcMain.DEBUG)
			{
				Main3();
			}
			else
			{
				Main4();
			}
			Common.OpenOutputDirIfCreated();
		}

		private void Main3()
		{
			Main4();
			//Common.Pause();
		}

		private void Main4()
		{
			// -- choose one --

			//Test01();
			//Test02();
			Test03();
			//new Test0001().Test01();
			//new Test0002().Test01();
			//new Test0003().Test01();

			// --
		}

		private void Test01()
		{
			string file = Common.NextOutputPath() + ".txt";

			using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
			{
				for (int y = 1; y <= 9999; y++)
				{
					for (int m = 1; m <= 12; m++)
					{
						for (int d = 1; d <= 31; d++)
						{
							SimpleDate date = new SimpleDate(y, m, d);
							writer.WriteLine(date + " -> " + date.GetWareki());
						}
					}
				}
			}
		}

		private void Test02()
		{
			for (; ; )
			{
				int y = SCommon.CRandom.GetRange(1, 9999);
				int m = SCommon.CRandom.GetRange(1, 12);
				int d = SCommon.CRandom.GetRange(1, 31);
				SimpleDate date = new SimpleDate(y, m, d);
				Console.WriteLine(date + " -> " + date.GetWareki());
			}
		}

		private void Test03()
		{
			string file = Common.NextOutputPath() + ".txt";

			using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
			{
				for (int y = 1; y <= 9999; y++)
				{
					for (int m = 1; m <= 12; m++)
					{
						for (int d = 1; d <= 31; d++)
						{
							SimpleDate date = new SimpleDate(y, m, d);
							writer.WriteLine(Common.HankakuToZenkaku(date + " ⇒ " + date.GetWareki()));
						}
					}
				}
			}
		}
	}
}
