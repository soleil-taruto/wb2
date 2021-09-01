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
			// -- choose one --

			Main4();
			//new Test0001().Test01();
			//new Test0002().Test01();
			//new Test0003().Test01();

			// --

			//Common.Pause();
		}

		private void Main4()
		{
			for (int testcnt = 0; testcnt < 1000000; testcnt++)
			{
				ProcMain.WriteLog("testcnt: " + testcnt);

				Test01();
			}
		}

		//private const int COLOR_NUM = 4;
		private const int COLOR_NUM = 100;

		private void Test01()
		{
			int[] colors = new int[COLOR_NUM];

			for (int index = 0; index < COLOR_NUM; index++)
				colors[index] = SCommon.CRandom.GetInt(COLOR_NUM);

			if (!TryGuess(colors)) // ? 正答者在らず。
				throw null; // 失敗
		}

		private bool TryGuess(int[] colors)
		{
			for (int answererIndex = 0; answererIndex < COLOR_NUM; answererIndex++)
			{
				List<int> otherColors = new List<int>(); // 自分以外の色

				for (int index = 0; index < COLOR_NUM; index++)
					if (index != answererIndex)
						otherColors.Add(colors[index]);

				int myColor = colors[answererIndex]; // 自分の色
				int answeredColor = TryGuess(otherColors.ToArray(), answererIndex); // 回答者の回答

				if (myColor == answeredColor) // ? 正答した。
					return true;
			}
			return false; // 正答者在らず。
		}

		private int TryGuess(int[] otherColors, int answererIndex)
		{
			return (COLOR_NUM - otherColors.Sum() % COLOR_NUM + answererIndex) % COLOR_NUM;
		}
	}
}
