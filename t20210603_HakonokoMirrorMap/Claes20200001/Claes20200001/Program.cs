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

		private static I3Color[] ReplaceColorPairArray = new I3Color[]
		{
			new I3Color(  63,  72, 207 ), // 青い箱 1
			new I3Color(  63,  72, 205 ), // 青い箱 3
			new I3Color(  63,  72, 208 ), // 青い箱 4
			new I3Color(  63,  72, 204 ), // 青い箱 6
			new I3Color(  63,  72, 209 ), // 青い箱 7
			new I3Color(  63,  72, 211 ), // 青い箱 9
			new I3Color(  34, 177,  79 ), // 緑の箱 T1
			new I3Color(  34, 177,  85 ), // 緑の箱 H3
			new I3Color(  34, 177,  80 ), // 緑の箱 T4
			new I3Color(  34, 177,  84 ), // 緑の箱 H6
			new I3Color(  34, 177,  81 ), // 緑の箱 T7
			new I3Color(  34, 177,  91 ), // 緑の箱 H9
			new I3Color(  34, 177,  87 ), // 緑の箱 H1
			new I3Color(  34, 177,  77 ), // 緑の箱 T3
			new I3Color(  34, 177,  88 ), // 緑の箱 H4
			new I3Color(  34, 177,  76 ), // 緑の箱 T6
			new I3Color(  34, 177,  89 ), // 緑の箱 H7
			new I3Color(  34, 177,  83 ), // 緑の箱 T9
			new I3Color(  34, 177,  78 ), // 緑の箱 T2
			new I3Color(  34, 177,  86 ), // 緑の箱 H2
			new I3Color(  34, 177,  82 ), // 緑の箱 T8
			new I3Color(  34, 177,  90 ), // 緑の箱 H8
			new I3Color( 255, 174, 201 ), // 赤い箱 L
			new I3Color( 255, 174, 204 ), // 赤い箱 R
			new I3Color( 255, 174, 202 ), // 赤い箱 LS
			new I3Color( 255, 174, 205 ), // 赤い箱 RS
			new I3Color( 255, 174, 203 ), // 赤い箱 LF
			new I3Color( 255, 174, 206 ), // 赤い箱 RF
		};

		private class ReplaceColorPairInfo
		{
			public I3Color From;
			public I3Color To;
		}

		private static ReplaceColorPairInfo[] ReplaceColorPairs;

		private void Main4()
		{
			ReplaceColorPairs = Enumerable.Range(0, ReplaceColorPairArray.Length)
				.Select(index => new ReplaceColorPairInfo()
				{
					From = ReplaceColorPairArray[index],
					To = ReplaceColorPairArray[index ^ 1],
				})
				.ToArray();

			MirrorMap(@"C:\Dev\Elsa3\e20210501_Hakonoko\dat\res\Map\0007.bmp", -1);
			//MirrorMap(@"C:\Dev\Elsa3\e20210501_Hakonoko\dat\res\Map\0009.bmp", 110); // 不実施
		}

		private void MirrorMap(string file, int mirH)
		{
			int w;
			int h;

			I3Color[,] bmp = Common.ReadBmpFile(File.ReadAllBytes(file), out w, out h);

			int mirT = 0;
			int mirL = 0;
			int mirW = w;
			//int mirH = h;

			if (mirH == -1)
				mirH = h;

			MirrorMap(bmp, new I4Rect(mirT, mirL, mirW, mirH));

			byte[] fileData = Common.WriteBmpFile(bmp, w, h);

#if !true
			File.WriteAllBytes(file, fileData);
#else // test
			File.WriteAllBytes(Common.NextOutputPath() + ".bmp", fileData);
#endif
		}

		private void MirrorMap(I3Color[,] bmp, I4Rect mirRect)
		{
			for (int x = 0; x < mirRect.W / 2; x++)
			{
				for (int y = 0; y < mirRect.H; y++)
				{
					SCommon.Swap(
						ref bmp[
							mirRect.L + x,
							mirRect.T + y
							],
						ref bmp[
							mirRect.L + mirRect.W - 1 - x,
							mirRect.T + y
							]
						);
				}
			}
			for (int x = 0; x < mirRect.W; x++)
			{
				for (int y = 0; y < mirRect.H; y++)
				{
					int index = SCommon.IndexOf(ReplaceColorPairs, pair => pair.From.IsSame(bmp[x, y]));

					if (index != -1)
					{
						bmp[x, y] = ReplaceColorPairs[index].To;
					}
				}
			}
		}
	}
}
