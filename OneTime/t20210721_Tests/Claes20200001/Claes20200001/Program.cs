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

			//new Test0001().Test01();
			//new Test0002().Test01(); // ファイルコピー(ランダム)
			//new Test0002().Test02(); // ファイルコピー(時間)
			//new Test0003().Test01();
			//new Test0003().Test02();
			//new Test0003().Test03();
			//new Test0003().Test04();
			//new Test0003().Test04_2();
			//new Test0003().Test05();
			//new Test0003().Test05_2();
			//new Test0003().Test05_3();
			//new Test0003().Test05_4();
			//new Test0003().Test05_5();
			//new Test0003().Test06();
			//new Test0003().Test06_2();
			//new Test0004().Test01();
			//new Test0004().Test02();
			//new Test0005().Test01();
			//new Test0005().Test02();
			//new Test0006().Test01();
			new Test0007().Test01();

			// --
		}
	}
}
