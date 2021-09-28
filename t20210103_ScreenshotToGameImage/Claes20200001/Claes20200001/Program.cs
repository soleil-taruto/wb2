using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			Common.Pause();
		}

		private void Main4()
		{
			// -- choose one --

			Main5();
			//new Test0001().Test01();
			//new Test0002().Test01();
			//new Test0003().Test01();

			// --
		}

		private Bitmap 位置合わせ_Bmp;

		private void Main5()
		{
			if (!Directory.Exists(Consts.SCREENSHOTS_DIR))
				throw new Exception("no SCREENSHOTS_DIR");

			if (!File.Exists(Consts.位置合わせ_IMAGE_FILE))
				throw new Exception("no 位置合わせ_IMAGE_FILE");

			SCommon.DeletePath(Consts.OUTPUT_DIR);
			SCommon.CreateDir(Consts.OUTPUT_DIR);

			this.位置合わせ_Bmp = (Bitmap)Bitmap.FromFile(Consts.位置合わせ_IMAGE_FILE);
			try
			{
				foreach (string file in Directory.GetFiles(Consts.SCREENSHOTS_DIR))
					if (
						SCommon.EqualsIgnoreCase(Path.GetExtension(file), Consts.SCREENSHOT_EXT_01) ||
						SCommon.EqualsIgnoreCase(Path.GetExtension(file), Consts.SCREENSHOT_EXT_02)
						)
						this.Conv(file);
			}
			finally
			{
				try
				{
					this.位置合わせ_Bmp.Dispose();
					this.位置合わせ_Bmp = null;
				}
				catch
				{ }
			}
		}

		private void Conv(string ssFile)
		{
			Console.WriteLine("< " + ssFile); // cout

			using (Bitmap ssBmp = (Bitmap)Bitmap.FromFile(ssFile))
			{
				I2Point pt = this.Find_位置合わせ(ssBmp);

				Console.WriteLine("pt_1: " + pt); // cout

				pt.X += Consts.位置合わせ_X_Diff;
				pt.Y += Consts.位置合わせ_Y_Diff;

				Console.WriteLine("pt_2: " + pt); // cout

				using (Bitmap gameImage = this.GetRect(ssBmp, pt.X, pt.Y, Consts.GameImage_W, Consts.GameImage_H))
				{
					string outputFile = Path.Combine(Consts.OUTPUT_DIR, Path.GetFileNameWithoutExtension(ssFile) + ".png");

					Console.WriteLine("> " + outputFile); // cout

					if (File.Exists(outputFile)) // ? 出力ファイル名の重複 -- 2bs
						throw null;

					gameImage.Save(outputFile, ImageFormat.Png);
				}
			}
		}

		private I2Point Find_位置合わせ(Bitmap ssBmp)
		{
			return FindPattern(ssBmp, this.位置合わせ_Bmp);
		}

		private static I2Point FindPattern(Bitmap bmp, Bitmap pattern)
		{
			for (int x = 0; x + pattern.Width <= bmp.Width; x++)
				for (int y = 0; y + pattern.Height <= bmp.Height; y++)
					if (IsMatch(bmp, pattern, x, y))
						return new I2Point(x, y);

			throw new Exception("画像パターン見つからじ！");
		}

		private static bool IsMatch(Bitmap bmp, Bitmap pattern, int l, int t)
		{
			if ((SCommon.CRandom.GetUInt() & 0xffffu) == 0u)
				Console.WriteLine("IsMatch (Random-Display) -- " + l + ", " + t); // cout

			for (int c = 0; c < 20; c++) // ゆるい検査
			{
				int x = SCommon.CRandom.GetInt(pattern.Width);
				int y = SCommon.CRandom.GetInt(pattern.Height);

				if (bmp.GetPixel(l + x, t + y) != pattern.GetPixel(x, y))
					return false;
			}

			// ちゃんと検査する。
			for (int x = 0; x < pattern.Width; x++)
				for (int y = 0; y < pattern.Height; y++)
					if (bmp.GetPixel(l + x, t + y) != pattern.GetPixel(x, y))
						return false;

			return true;
		}

		private Bitmap GetRect(Bitmap src, int l, int t, int w, int h)
		{
			Bitmap canvas = new Bitmap(w, h);
			try
			{
				using (Graphics g = Graphics.FromImage(canvas))
				{
					g.DrawImage(src, new Rectangle(0, 0, w, h), new Rectangle(l, t, w, h), GraphicsUnit.Pixel);
				}
				return canvas;
			}
			catch
			{
				try
				{
					canvas.Dispose();
					canvas = null;
				}
				catch
				{ }

				throw;
			}
		}
	}
}
