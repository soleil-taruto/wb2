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

			MakeTextPanels_20210904();

			// --
		}

		private void MakeTextPanels_20210904()
		{
			MakeTextPanel("BRAND-NAME", 960, 180, 4, "Impact", 480, FontStyle.Regular, 100, -30);
			MakeTextPanel("CERULEAN.CHARLOTTE", 960, 180, 4, "Bahnschrift Condensed", 380, FontStyle.Bold, -15, 140);
		}

		private void MakeTextPanel(string text, int w, int h, int 描画時の倍率, string fontName, int fontSize, FontStyle fontStyle, int 描画位置_L, int 描画位置_T)
		{
			int ew = w * 描画時の倍率;
			int eh = h * 描画時の倍率;

			Canvas canvas = new Canvas(ew, eh);

			canvas.Fill(new I4Color(0, 0, 0, 255));
			canvas = canvas.DrawString(text, fontSize, fontName, fontStyle, new I4Color(255, 255, 255, 255), 描画位置_L, 描画位置_T);
			canvas = canvas.Expand(w, h);

			WhiteLevelToAlpha(canvas); // 出力を確認する時はここをコメントアウト

			canvas.Save(Common.NextOutputPath() + ".png");
		}

		private void WhiteLevelToAlpha(Canvas canvas)
		{
			for (int x = 0; x < canvas.W; x++)
			{
				for (int y = 0; y < canvas.H; y++)
				{
					I4Color dot = canvas[x, y];

					I4Color dotNew = new I4Color(
						255,
						255,
						255,
						dot.R // 白さを透過率とする。代表_R
						);

					canvas[x, y] = dotNew;
				}
			}
		}
	}
}
