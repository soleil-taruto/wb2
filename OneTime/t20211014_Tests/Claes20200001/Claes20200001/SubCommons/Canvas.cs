using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Charlotte.Commons;

namespace Charlotte.SubCommons
{
	public class Canvas
	{
		private I4Color[,] Dots;
		public int W { get; private set; }
		public int H { get; private set; }

		public Canvas(int w, int h)
		{
			this.Dots = new I4Color[w, h];
			this.W = w;
			this.H = h;
		}

		public I4Color this[int x, int y]
		{
			get
			{
				return this.Dots[x, y];
			}

			set
			{
				this.Dots[x, y] = value;
			}
		}

		public static Canvas LoadFromFile(string imageFile)
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
		/// 指定した矩形領域に文字列を描画する。
		/// 新しいキャンパスを返し、このインスタンスは変更しない。
		/// </summary>
		/// <param name="text">文字列</param>
		/// <param name="fontSize">フォントサイズ</param>
		/// <param name="fontName">フォント名</param>
		/// <param name="fontStyle">フォントスタイル</param>
		/// <param name="color">色</param>
		/// <param name="rect">描画したい領域</param>
		/// <param name="blurLv">ぼかし量</param>
		/// <returns>新しいキャンパス</returns>
		public void DrawString(string text, int fontSize, string fontName, FontStyle fontStyle, I3Color color, I4Rect rect, int blurLv)
		{
			int w;
			int h;

			using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
			{
				SizeF size = g.MeasureString(text, new Font(fontName, fontSize, fontStyle));

				w = (int)size.Width;
				h = (int)size.Height;
			}
			Canvas canvas = new Canvas(w, h);

			canvas.Fill(new I4Color(0, 0, 0, 255));
			canvas = canvas.DrawString(text, fontSize, fontName, fontStyle, new I4Color(255, 255, 255, 255), 0, 0);
			canvas = canvas.DS_SetMargin(v => v.R == 0, blurLv, new I4Color(0, 0, 0, 255));
			canvas.DS_Blur(blurLv, color);
			canvas = canvas.Expand(rect.W, rect.H);

			this.DrawImage(canvas, rect.L, rect.T, true);
		}

		private Canvas DS_SetMargin(Predicate<I4Color> matchOuter, int margin, I4Color outerColor)
		{
			int x1 = int.MaxValue;
			int y1 = int.MaxValue;
			int x2 = -1;
			int y2 = -1;

			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					if (!matchOuter(this[x, y]))
					{
						x1 = Math.Min(x1, x);
						y1 = Math.Min(y1, y);
						x2 = Math.Max(x2, x);
						y2 = Math.Max(y2, y);
					}
				}
			}
			if (x2 == -1)
				throw new Exception("中身無し");

			int l = x1;
			int t = y1;
			int w = x2 - x1 + 1;
			int h = y2 - y1 + 1;

			Canvas dest = new Canvas(w + margin * 2, h + margin * 2);

			dest.Fill(outerColor);

			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					dest[margin + x, margin + y] = this[l + x, t + y];
				}
			}
			return dest;
		}

		private void DS_Blur(int blurLv, I3Color color)
		{
			double[, ,] map = new double[2, this.W, this.H];
			int r = 0;

			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					map[0, x, y] = this[x, y].R / 255.0;
				}
			}
			for (int c = 0; c < blurLv; c++)
			{
				int w = 1 - r;

				for (int x = 0; x < this.W; x++)
				{
					for (int y = 0; y < this.H; y++)
					{
						double d = 0.0;
						int dc = 0;

						for (int xc = -1; xc <= 1; xc++)
						{
							for (int yc = -1; yc <= 1; yc++)
							{
								int sx = x + xc;
								int sy = y + yc;

								if (
									0 <= sx && sx < this.W &&
									0 <= sy && sy < this.H
									)
								{
									d += map[r, sx, sy];
									dc++;
								}
							}
						}
						map[w, x, y] = d / dc;
					}
				}
				r = w;
			}
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					this[x, y] = new I4Color(color.R, color.G, color.B, SCommon.ToInt(map[r, x, y] * 255.0));
				}
			}
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
		/// <param name="fontStyle">フォントスタイル</param>
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
		/// 指定位置に画像を描画する。
		/// </summary>
		/// <param name="src">描画する画像</param>
		/// <param name="l">描画する領域の左上座標_X-軸</param>
		/// <param name="t">描画する領域の左上座標_Y-軸</param>
		/// <param name="allowAlpha">透過率を考慮するか</param>
		public void DrawImage(Canvas src, int l, int t, bool allowAlpha = false)
		{
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

		public Canvas Expand(int w, int h)
		{
			//const int SAMPLING = 4;
			//const int SAMPLING = 8;
			//const int SAMPLING = 16;
			const int SAMPLING = 24;

			return Expand(w, h, SAMPLING);
		}

		public Canvas Expand(int w, int h, int sampling)
		{
			return Expand(w, h, sampling, sampling);
		}

		/// <summary>
		/// 目的のサイズに拡大・縮小する。
		/// 新しいキャンパスを返し、このインスタンスは変更しない。
		/// サンプリング回数：
		/// -- 出力先の１ドットの１辺につき何回サンプリングするか
		/// </summary>
		/// <param name="w">目的の幅</param>
		/// <param name="h">目的の高さ</param>
		/// <param name="xSampling">サンプリング回数(横方向)</param>
		/// <param name="ySampling">サンプリング回数(縦方向)</param>
		/// <returns>新しいキャンパス</returns>
		public Canvas Expand(int w, int h, int xSampling, int ySampling)
		{
			Canvas dest = new Canvas(w, h);

			for (int x = 0; x < w; x++)
			{
				for (int y = 0; y < h; y++)
				{
					int r = 0;
					int g = 0;
					int b = 0;
					int a = 0;

					for (int xc = 0; xc < xSampling; xc++)
					{
						for (int yc = 0; yc < ySampling; yc++)
						{
							double xd = x + (xc + 0.5) / xSampling;
							double yd = y + (yc + 0.5) / ySampling;
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
						a = SCommon.ToInt(a / (xSampling * ySampling));
					}
					dest[x, y] = new I4Color(r, g, b, a);
				}
			}
			return dest;
		}

		/// <summary>
		/// 指定された色でキャンバス全体を塗りつぶす。
		/// </summary>
		/// <param name="color">塗りつぶす色</param>
		public void Fill(I4Color color)
		{
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					this[x, y] = color;
				}
			}
		}

		// ====
		// ====
		// ====

		public void FillRect(I4Color color, I4Rect rect)
		{
			for (int x = rect.L; x < rect.R; x++)
			{
				for (int y = rect.T; y < rect.B; y++)
				{
					this[x, y] = color;
				}
			}
		}

		public void FillCircle(I4Color color, I2Point pt, int r)
		{
			int x1 = pt.X - r;
			int x2 = pt.X + r;
			int y1 = pt.Y - r;
			int y2 = pt.Y + r;

			x1 = Math.Max(x1, 0);
			x2 = Math.Min(x2, this.W - 1);
			y1 = Math.Max(y1, 0);
			y2 = Math.Min(y2, this.H - 1);

			const double R_MARGIN = 0.2;

			for (int x = x1; x <= x2; x++)
			{
				for (int y = y1; y <= y2; y++)
				{
					double d = Common.GetDistance(new D2Point(x - pt.X, y - pt.Y));

					if (d < r + R_MARGIN)
					{
						this[x, y] = color;
					}
				}
			}
		}
	}
}
