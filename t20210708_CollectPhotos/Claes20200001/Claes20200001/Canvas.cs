using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Charlotte.Commons;

namespace Charlotte
{
	public class Canvas
	{
		private I4Color[,] DotTable;
		public int W { get; private set; }
		public int H { get; private set; }

		public Canvas(int w, int h)
		{
			this.DotTable = new I4Color[w, h];
			this.W = w;
			this.H = h;
		}

		public I4Color this[int x, int y]
		{
			get
			{
				return this.DotTable[x, y];
			}

			set
			{
				this.DotTable[x, y] = value;
			}
		}

		public static Canvas Load(string imageFile)
		{
			using (Bitmap bmp = (Bitmap)Bitmap.FromFile(imageFile))
			{
				return Load(bmp);
			}
		}

		public static Canvas Load(Bitmap bmp)
		{
			Canvas canvas = new Canvas(bmp.Width, bmp.Height);

			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					Color color = bmp.GetPixel(x, y);

					canvas[x, y] = new I4Color(
						color.R,
						color.G,
						color.B,
						color.A
						);
				}
			}
			return canvas;
		}

		public void Save(string pngFile)
		{
			this.ToBitmap().Save(pngFile);
		}

		/// <summary>
		/// Jpegとして保存します。
		/// </summary>
		/// <param name="jpegFile">保存先ファイル名</param>
		/// <param name="qualityLevel">Jpegのクオリティ(0～100)</param>
		public void SaveAsJpeg(string jpegFile, int qualityLevel)
		{
			ImageCodecInfo ici = ImageCodecInfo.GetImageEncoders().First(v => v.FormatID == ImageFormat.Jpeg.Guid);
			EncoderParameter ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityLevel);
			EncoderParameters eps = new EncoderParameters(1);
			eps.Param[0] = ep;
			this.ToBitmap().Save(jpegFile, ici, eps);
		}

		public Bitmap ToBitmap()
		{
			Bitmap bmp = new Bitmap(this.W, this.H);

			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					bmp.SetPixel(x, y, this[x, y].ToColor());
				}
			}
			return bmp;
		}

		/// <summary>
		/// 目的のサイズに拡大・縮小する。
		/// 新しいキャンパスを返し、このインスタンスは変更しない。
		/// </summary>
		/// <param name="w">目的の幅</param>
		/// <param name="h">目的の高さ</param>
		/// <returns>新しいキャンパス</returns>
		public Canvas Expand(int w, int h)
		{
			//const int SAMPLING = 4;
			//const int SAMPLING = 8;
			//const int SAMPLING = 16;
			const int SAMPLING = 24;

			Canvas dest = new Canvas(w, h);

			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					int r = 0;
					int g = 0;
					int b = 0;
					int a = 0;

					for (int xc = 0; xc < SAMPLING; xc++)
					{
						for (int yc = 0; yc < SAMPLING; yc++)
						{
							double xd = x + (xc + 0.5) / SAMPLING;
							double yd = y + (yc + 0.5) / SAMPLING;
							double xs = (xd * this.W) / w;
							double ys = (yd * this.H) / h;
							int ixs = (int)xs;
							int iys = (int)ys;

							I4Color sDot = this[ixs, iys];

							r += sDot.A * sDot.R;
							g += sDot.A * sDot.G;
							b += sDot.A * sDot.B;
							a += sDot.A;
						}
					}
					if (1 <= a)
					{
						r = SCommon.ToInt(r / a);
						g = SCommon.ToInt(g / a);
						b = SCommon.ToInt(b / a);
						a = SCommon.ToInt(a / (SAMPLING * SAMPLING));
					}
					dest[x, y] = new I4Color(r, g, b, a);
				}
			}
			return dest;
		}

		/// <summary>
		/// 文字列を描画する。
		/// 新しいキャンパスを返し、このインスタンスは変更しない。
		/// フォントサイズ：
		/// -- 文字の幅(ピクセル数) =~ 文字の高さ(ピクセル数) =~ フォントサイズ * 1.333
		/// 描画位置：
		/// -- 描画領域の左上
		/// </summary>
		/// <param name="text">文字列</param>
		/// <param name="fontSize">フォントサイズ</param>
		/// <param name="fontName">フォント名</param>
		/// <param name="color">色</param>
		/// <param name="x">描画位置_X-軸</param>
		/// <param name="y">描画位置_Y-軸</param>
		/// <returns>新しいキャンパス</returns>
		public Canvas DrawString(string text, int fontSize, string fontName, FontStyle fontStyle, I4Color color, int x, int y)
		{
			Bitmap bmp = this.ToBitmap();

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.DrawString(text, new Font(fontName, fontSize, fontStyle), new SolidBrush(color.ToColor()), new Point(x, y));
			}
			return Load(bmp);
		}

		/// <summary>
		/// キャンパス全体を塗り潰す。
		/// 透過率を考慮しない。
		/// </summary>
		/// <param name="color">塗り潰す色</param>
		public void Fill(I4Color color)
		{
			this.FillRect(new I4Rect(0, 0, this.W, this.H), color);
		}

		/// <summary>
		/// キャンパス全体を塗り潰す。
		/// </summary>
		/// <param name="a_fill">塗り潰しアクション</param>
		public void Fill(Func<I4Color, I4Color> a_fill)
		{
			this.FillRect(new I4Rect(0, 0, this.W, this.H), a_fill);
		}

		/// <summary>
		/// 矩形領域を塗り潰す。
		/// 透過率を考慮しない。
		/// </summary>
		/// <param name="rect">塗り潰す領域</param>
		/// <param name="color">塗り潰す色</param>
		public void FillRect(I4Rect rect, I4Color color)
		{
			for (int x = rect.L; x < rect.R; x++)
			{
				for (int y = rect.T; y < rect.B; y++)
				{
					this[x, y] = color;
				}
			}
		}

		/// <summary>
		/// 矩形領域を塗り潰す。
		/// </summary>
		/// <param name="rect">塗り潰す領域</param>
		/// <param name="a_fill">塗り潰しアクション</param>
		public void FillRect(I4Rect rect, Func<I4Color, I4Color> a_fill)
		{
			for (int x = rect.L; x < rect.R; x++)
			{
				for (int y = rect.T; y < rect.B; y++)
				{
					I4Color color = this[x, y];
					color = a_fill(color);
					this[x, y] = color;
				}
			}
		}

		/// <summary>
		/// 円を描画する。
		/// 中心：
		/// -- (100.0, 200.0) を指定した場合、座標 (100, 200) にあるピクセルの左上を指す。
		/// </summary>
		/// <param name="center">中心</param>
		/// <param name="r">半径</param>
		/// <param name="color">色</param>
		public void DrawCircle(D2Point center, double r, I4Color color)
		{
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					double xd = x + 0.5;
					double yd = y + 0.5;

					if (Common.GetDistance(new D2Point(xd, yd) - center) <= r)
					{
						this[x, y] = color;
					}
				}
			}
		}

		/// <summary>
		/// 矩形領域を切り取る。
		/// 新しいキャンパスを返し、このインスタンスは変更しない。
		/// </summary>
		/// <param name="rect">切り取る領域</param>
		/// <returns>新しいキャンパス</returns>
		public Canvas Cut(I4Rect rect)
		{
			if (
				rect.L < 0 ||
				rect.T < 0 ||
				this.W < rect.R ||
				this.H < rect.B
				)
				throw new Exception("Bad rect");

			Canvas dest = new Canvas(rect.W, rect.H);

			for (int x = 0; x < rect.W; x++)
			{
				for (int y = 0; y < rect.H; y++)
				{
					dest[x, y] = this[rect.L + x, rect.T + y];
				}
			}
			return dest;
		}

		/// <summary>
		/// このキャンパスのコピーを作成する。
		/// </summary>
		/// <returns>このキャンパスのコピー</returns>
		public Canvas Copy()
		{
			return this.Cut(new I4Rect(0, 0, this.W, this.H));
		}

		/// <summary>
		/// 指定位置に画像を描画する。
		/// </summary>
		/// <param name="src">描画する画像</param>
		/// <param name="l">描画する領域の左上座標_X-軸</param>
		/// <param name="t">描画する領域の左上座標_Y-軸</param>
		/// <param name="allowAlpha">透過率を考慮するか</param>
		public void DrawImage(Canvas src, int l, int t, bool allowAlpha = false)
		{
			if (
				l < 0 ||
				t < 0 ||
				this.W < l + src.W ||
				this.H < t + src.H
				)
				throw new Exception("Bad rect");

			for (int x = 0; x < src.W; x++)
			{
				for (int y = 0; y < src.H; y++)
				{
					if (allowAlpha) // 透過率を考慮する。
					{
						D4Color dCol = this[l + x, t + y].ToD4Color();
						D4Color sCol = src[x, y].ToD4Color();

						double da = dCol.A * (1.0 - sCol.A);
						double sa = sCol.A;
						double xa = da + sa;

						D4Color xCol = new D4Color(
							(dCol.R * da + sCol.R * sa) / xa,
							(dCol.G * da + sCol.G * sa) / xa,
							(dCol.B * da + sCol.B * sa) / xa,
							xa
							);

						this[l + x, t + y] = xCol.ToI4Color();
					}
					else // 透過率を考慮しない。
					{
						this[l + x, t + y] = src[x, y];
					}
				}
			}
		}
	}
}
