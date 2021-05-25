using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte
{
	public static class Consts
	{
		public const string SCREENSHOTS_DIR = @"C:\temp";
		public const string SCREENSHOT_EXT_01 = ".bmp";
		public const string SCREENSHOT_EXT_02 = ".png"; // .pngで保存したスクショの再利用を想定

		public const string OUTPUT_DIR = @"C:\108";

		public const string 位置合わせ_IMAGE_FILE = @"..\..\..\..\res\位置合わせ.bmp";

		public const int 位置合わせ_X_Diff = -2;
		public const int 位置合わせ_Y_Diff = 22;

		public const int GameImage_W = 960;
		public const int GameImage_H = 540;
	}
}
