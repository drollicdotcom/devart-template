/*
Copyright © 2006, Drollic
All rights reserved.
http://www.drollic.com

Redistribution of this software in source or binary forms, with or 
without modification, is expressly prohibited. You may not reverse-assemble, 
reverse-compile, or otherwise reverse-engineer this software in any way.

THIS SOFTWARE ("SOFTWARE") IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for ColorImage.
	/// </summary>
	public class ColorImage
	{           
		public Color [] singledata;
		private static Color StartingColor = Color.FromArgb(255, 255, 255);

		public int Width;
		public int Height;

        public ColorImage()
        {
        }

		public ColorImage(UnsafeBitmap img)
		{         
			DateTime start = DateTime.Now;

			this.Width = img.Width;
			this.Height = img.Height;
         
			this.singledata = new Color[this.Height * this.Width];

			for (int j=0; j < this.Height; j++)
			{
				for (int i=0; i < this.Width; i++)
				{
					this.singledata[j * this.Width + i] = img.GetPixel(i, j);
				}
			}
        }   


		public ColorImage(int width, int height)
		{   
			this.Width = width;
			this.Height = height;
         
			this.singledata = new Color[this.Height * this.Width];

			for (int j=0; j < this.Height; j++)
			{
				for (int i=0; i < this.Width; i++)
				{
					this.singledata[j * this.Width + i] = StartingColor;
				}
			} 
		}

		public virtual Color GetColor(int x, int y)
		{
			return this.singledata[y * this.Width + x]; //this.data[y][x];			
		}

		public virtual void SetColor(int x, int y, Color c)
		{
			this.singledata[y * this.Width + x] = c;
		}

		public ColorImage Crop(int xc, int yc, int w, int h)
		{
			int x0, y0;

			DateTime start = DateTime.Now;

			/* adjust region to fit in source image */
			x0 = xc - w/2;
			y0 = yc - h/2;
			if (x0 < 0) 
			{
				w += x0;
				x0 = 0;
			}

			if (x0 > this.Width) x0 = this.Width;
			if (y0 < 0) 
			{
				h += y0;
				y0 = 0;
			}
			if (y0 > this.Height) y0 = this.Height;
			if (x0 + w > this.Width) w = this.Width - x0; 
			if (y0 + h > this.Height) h = this.Height - y0; 

			// create cropped result
			ColorImage output = new ColorImage(w, h);
			int xCounter = 0, yCounter = 0;
			for (int outY=y0; outY < y0 + h; outY++)
			{
				xCounter = 0;
				for (int outX=x0; outX < x0 + w; outX++)
				{
					output.singledata[yCounter * output.Width + xCounter++] = this.singledata[outY * this.Width + outX];
				}
				yCounter++;
			}

			return output;
		}


		public Bitmap GenerateBitmap()
		{
			Bitmap img = new Bitmap(this.Width, this.Height);
			for (int y=0; y < this.Height; y++)
			{
				for (int x=0; x < this.Width; x++)
				{
					img.SetPixel(x, y, this.GetColor(x, y));
				}
			}

			return img;
		}


		public static int Blend(ref ColorImage inp, ColorImage stroke, Color rgb, int xc, int yc)
		{
			int xi, yi; 			/* lower left corner in input image */
			int xa, ya;				/* corresponding corner in alpha image */
			int wa, ha;				/* area in alpha image to be blended */

			wa = stroke.Width;
			xa = 0;
			xi = xc - wa/2;
			if (xi < 0) 
			{
				wa += xi;
				xa -= xi;
				xi = 0;
			}
			if (xi > inp.Width) 
				return 0;
			if (wa <= 0)
				return 0;
			if (xi + wa >= inp.Width)
				wa = inp.Width - xi;

			ha = stroke.Height;
			ya = 0;
			yi = yc - ha/2;
			if (yi < 0) 
			{
				ha += yi;
				ya -= yi;
				yi = 0;
			}
			if (yi > inp.Height) 
				return 0;
			if (ha <= 0)
				return 0;
			if (yi + ha >= inp.Height)
				ha = inp.Height - yi;

			//la = stroke->row*stroke->type;
			//li = in->row*in->type;

			//input = in->buffer + li*yi + xi*in->type;
			//alpha = stroke->buffer + la*ya + xa*stroke->type;
	
			for (int iny = yi, strokey = ya; strokey < ha; iny++, strokey++) 
			{
				for (int inx = xi, strokex = xa; strokex < wa; inx++, strokex++) 
				{
					int upperR = stroke.GetColor(strokex, strokey).R;

					double alpha = upperR / 255.0;
					double oneMinusAlpha = (1.0 - alpha);

					Color lowerC = inp.GetColor(inx, iny);

					int r = (int)((alpha * rgb.R) + (lowerC.R * oneMinusAlpha));
					int g = (int)((alpha * rgb.G) + (lowerC.G * oneMinusAlpha));
					int b = (int)((alpha * rgb.B) + (lowerC.B * oneMinusAlpha));
                  
					inp.SetColor(inx, iny, Color.FromArgb(r, g, b));
				}
			}

			return 1;
		}


		public static void BlendImages(ref ColorImage lower, ColorImage upper, Color c, int xc, int yc)
		{
			int xOffset = xc - upper.Width;
			int yOffset = yc - upper.Height;

			for (int y=0; y < upper.Height; y++)
			{
				for (int x=0; x < upper.Width; x++)
				{
					int lowerX = x + xOffset + 15;
					int lowerY = y + yOffset + 15;

					if ((lowerX > 0) && (lowerX < lower.Width) && (lowerY > 0) && (lowerY < lower.Height))
					{
						int upperR = upper.GetColor(x, y).R;

						double alpha = upperR / 255.0;
						double oneMinusAlpha = (1.0 - alpha);

						Color lowerC = lower.GetColor(lowerX, lowerY);

						int r = (int)((alpha * c.R) + (lowerC.R * oneMinusAlpha));
						int g = (int)((alpha * c.G) + (lowerC.G * oneMinusAlpha));
						int b = (int)((alpha * c.B) + (lowerC.B * oneMinusAlpha));
                  
						lower.SetColor(lowerX, lowerY, Color.FromArgb(r, g, b));
					}
				}
			}
		}
      


		public static ColorImage ScaleWidth(ColorImage input, int ow)
		{
			ColorImage output = new ColorImage(ow, input.Height);

			int u, x, y;
			int res_r, res_g, res_b, acc_r, acc_g, acc_b;
			int p, q;
			int iw = input.Width;
			int area = (ow * iw);

			for (y = 0; y < input.Height; y++) 
			{
				q = iw;
				p = ow;
				acc_r = 0;
				acc_g = 0;
				acc_b = 0;
				x = u = 0;

				while (x < ow) 
				{
					if (u+1 < iw) 
					{
						/*
						res_r = p * input.data[y][u].R + (ow - p) * input.data[y][u+1].R;
						res_g = p * input.data[y][u].G + (ow - p) * input.data[y][u+1].G;
						res_b = p * input.data[y][u].B + (ow - p) * input.data[y][u+1].B;
						*/
						Color cuy = input.GetColor(u, y);
						Color cuPlusOney = input.GetColor(u+1, y);
						res_r = p * cuy.R + (ow - p) * cuPlusOney.R;
						res_g = p * cuy.G + (ow - p) * cuPlusOney.G;
						res_b = p * cuy.B + (ow - p) * cuPlusOney.B;

					}
					else
					{
						/*
						res_r = ow * input.data[y][u].R;
						res_g = ow * input.data[y][u].G;
						res_b = ow * input.data[y][u].B;						
						*/
						Color cuy = input.GetColor(u, y);
						res_r = ow * cuy.R;
						res_g = ow * cuy.G;
						res_b = ow * cuy.B;						
					}

					if (p < q) 
					{
						acc_r += res_r * p;
						acc_g += res_g * p;
						acc_b += res_b * p;
						q -= p;
						p = ow;
						u++;
					} 
					else 
					{
						acc_r += res_r * q;
						acc_g += res_g * q;
						acc_b += res_b * q;
                  
						//output.data[y][x] = Color.FromArgb((byte)(acc_r / area), (byte)(acc_g / area), (byte)(acc_b / area));
						output.SetColor(x, y, Color.FromArgb((byte)(acc_r / area), (byte)(acc_g / area), (byte)(acc_b / area)));
						acc_r = 0;
						acc_g = 0;
						acc_b = 0;
						p -= q;
						q = iw;
						x++;
					}
				}
			}

			output.Width = ow;
			output.Height = input.Height;

			return output;
		}

		public static ColorImage ScaleHeight(ColorImage input, int oh)
		{
			ColorImage output = new ColorImage(input.Width, oh);

			int x, y, v;
			int res_r, acc_r;
			int res_g, acc_g;
			int res_b, acc_b;
			int p, q;
			int ih = input.Height;
			int area = (ih * oh);

			for (x = 0; x < input.Width; x++) 
			{
				q = ih;
				p = oh;
				acc_r = 0;
				acc_g = 0;
				acc_b = 0;
				y = v = 0;

				while (y < oh) 
				{
					if (v+1 < ih) 
					{
						/*
						res_r = p * input.data[v][x].R + (oh - p) * input.data[v][x].R;
						res_g = p * input.data[v][x].G + (oh - p) * input.data[v][x].G;
						res_b = p * input.data[v][x].B + (oh - p) * input.data[v][x].B;
						*/
						Color cxv = input.GetColor(x, v);
						res_r = p * cxv.R + (oh - p) * cxv.R;
						res_g = p * cxv.G + (oh - p) * cxv.G;
						res_b = p * cxv.B + (oh - p) * cxv.B;

					} 
					else 
					{
						/*
						res_r = oh * input.data[v][x].R;
						res_g = oh * input.data[v][x].G;
						res_b = oh * input.data[v][x].B;
						*/
						Color cxv = input.GetColor(x, v);
						res_r = oh * cxv.R;
						res_g = oh * cxv.G;
						res_b = oh * cxv.B;

					}
					if (p < q) 
					{
						acc_r += res_r * p;
						acc_g += res_g * p;
						acc_b += res_b * p;
						q -= p;
						p = oh;
						v++;
					} 
					else 
					{
						acc_r += res_r * q;
						acc_g += res_g * q;
						acc_b += res_b * q;
						//output.data[y][x]= Color.FromArgb((byte)(acc_r / area), (byte)(acc_g / area), (byte)(acc_b / area));
						output.SetColor(x, y, Color.FromArgb((byte)(acc_r / area), (byte)(acc_g / area), (byte)(acc_b / area)));
						acc_r = 0;
						acc_g = 0;
						acc_b = 0;
						p -= q;
						q = ih;
						y++;
					}
				}
			}

			output.Width = input.Width;
			output.Height = oh;

			return output;
		}


		public static ColorImage Rotate(ColorImage input, double thetaInRadians)
		{
			double q = thetaInRadians;
			int max = System.Math.Max(input.Width, input.Height);
			ColorImage output = new ColorImage(max + max, max + max);

			double cosX = System.Math.Cos(q);
			double sinX = System.Math.Sin(q);

			int origXCenter = input.Width / 2;
			int origYCenter = input.Height / 2;
			int xOffset = (int)(output.Width / 2);
			int yOffset = (int)(output.Height / 2);

			int xCenter = (int)((origXCenter * cosX) - (origYCenter * sinX));
			int yCenter = (int)((origXCenter * sinX) + (origYCenter * cosX));
			int xc = 0, yc = 0;

			for (int y=0; y < input.Height; y++)
			{
				for (int x=0; x < input.Width; x++)
				{
					xc = x - origXCenter;
					yc = y - origYCenter;
               
					int xPrime = (int)((xc * cosX) - (yc * sinX)) + xOffset;
					int yPrime = (int)((xc * sinX) + (yc * cosX)) + yOffset;

					if ((xPrime > 0) && (xPrime < output.singledata.Length) && (yPrime > 0) && (yPrime < output.singledata.Length))
					{
						output.SetColor(xPrime, yPrime, input.GetColor(x, y));
					}
				}
			}

			return output;
		}

	}
}