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
			// -- choose one --

			MakeButtons_20200001_DoremyRockman();
			//MakeButtons_20200002_SSAGame();

			// --
		}

		private void MakeButtons_20200001_DoremyRockman()
		{
			I4Color color = new I4Color(255, 64, 64, 255);

			MakeButtons(2400, color, "ゲームスタート", 60);
			MakeButtons(2400, color, "コンテニュー", 180);
			MakeButtons(2400, color, "設定", 675);
			MakeButtons(2400, color, "終了", 675);
		}

		private void MakeButtons_20200002_SSAGame()
		{
			I4Color color = new I4Color(233, 255, 33, 255);

			MakeButtons(2400, color, "ゲームスタート", 60);
			MakeButtons(2400, color, "コンテニュー", 180);
			MakeButtons(2400, color, "設定", 675);
			MakeButtons(2400, color, "終了", 675);
		}

		private void MakeButtons(int w, I4Color frameColor, string text, int text_x)
		{
			//int w = 2400;
			int h = 480;
			int frame = 60;

			I4Color backColor = new I4Color(255, 255, 255, 0);
			I4Color textColor = new I4Color(255, 255, 255, 255);

			Canvas canvas = new Canvas(w, h);

			canvas.Fill(backColor);
			canvas.DrawCircle(new D2Point(0 + h / 2, h / 2), h / 2, frameColor);
			canvas.DrawCircle(new D2Point(w - h / 2, h / 2), h / 2, frameColor);
			canvas.FillRect(new I4Rect(h / 2, frame * 0, w - h, h - frame * 0), frameColor);
			canvas.FillRect(new I4Rect(h / 2, frame * 1, w - h, h - frame * 2), backColor);

			{
				Func<I4Color, I4Color> a_fill = col =>
				{
					col.R /= 2;
					col.G /= 2;
					col.B /= 2;

					return col;
				};

				canvas.FillRect(new I4Rect(1 * w / 2, 0 * h / 2, w / 2, h / 2), a_fill);
				canvas.FillRect(new I4Rect(0 * w / 2, 1 * h / 2, w / 2, h / 2), a_fill);
			}

			string name = text;

			{
				int p = text.IndexOf(':');

				if (p != -1)
				{
					name = text.Substring(0, p);
					text = text.Substring(p + 1);
				}
			}

			canvas = canvas.DrawString(text, 180, "メイリオ", FontStyle.Regular, textColor, h / 2 + text_x, 70);
			canvas = canvas.Expand(w / 12, h / 12);
			canvas.Save(Path.Combine(Common.GetOutputDir(), name + ".png"));
		}
	}
}
