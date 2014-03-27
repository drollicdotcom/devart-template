using System;
using System.Drawing;
using System.Collections;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// This dithering algorithm is designed to spread the error randomly in all directions.
	/// </summary>
	public class FloydSteinbergDither : IDitheringAlgorithm
	{
		/// <summary>
		/// This parameter is the largest length of pixels that can receive no stroke
		/// </summary>
		private double s;  

		/// <summary>
		/// This parameter performs a gamma-correction on the image, controlling the concentration of strokes near 
		/// the borders of the image
		/// </summary>
		private double p;

		public FloydSteinbergDither(double s, double p)
		{
			this.s = s;
			this.p = p;
		}

		public void ResetParameters(double s, double p)
		{
			this.s = s;
			this.p = p;
		}

		public Bitmap Dither(ColorImage inImage)
		{
			int width = inImage.Width;
			int height = inImage.Height;
			int [,] input = new int [height,width];
			Bitmap img = new Bitmap(width, height);
			int w, h;
			float error, val;
			float a = (float) ((s - 1) / System.Math.Pow(255.0, p));        

			int [,] inputPrime = new int [height,width];
         
			// Convert into collection of Color objects
			for (int y = 0; y < inImage.Height; y++) 
			{
				for (int x = 0; x < inImage.Width; x++) 
				{               
					int r = inImage.GetColor(x, y).R;
					input[y,x] = r;
				}
			}

			System.Random rand = new System.Random();	
		
			w = width;
			h = height;

			/* allocates working space */	
			float [] cur  = new float [w];
			float [] nxt  = new float [w];

			/* inicializa o buffer de proxima linha */
			for (int x = 0; x < w; x++)
				nxt[x] = System.Math.Max(1.0f /(a * (float)(System.Math.Pow(input[0, x], p)) + 1), 0.5f);

			int yClone = 0;
			for (int y=0; y < h-1; y++, yClone = y) 
			{
				/* next line becomes current line */
				nxt.CopyTo(cur, 0);				

				/* copies next line to local buffer */
				nxt[0] = System.Math.Max(1.0f /(a *(float)(System.Math.Pow(input[y+1, 0], p)) + 1), 0.5f);

				for (int x = 1; x < w; x++) 
				{
					nxt[x] = 1.0f / (a * (float)(System.Math.Pow(input[y+1, x], p)) + 1);
				}

				/* spread error */
				int xClone = 0;
				for (int x = 0; x < w-1; x++, xClone = x) 
				{
					val = cur[x] > 1.0f ? 1.0f : 0.0f;
					error = cur[x] - val;
					inputPrime[y, x] = val > 0.0 ? 0 : 255;
					switch (rand.Next() % 4) 
					{
						case 0:
							nxt[x + 1] += error/16.0f;
							if (x > 0)
								nxt[x - 1] += 3*error/16.0f;
							nxt[x]     += 5*error/16.0f;
							cur[x + 1] += 7*error/16.0f;
							break;
						case 1:
							nxt[x + 1] += 7*error/16.0f;
							if (x > 0)
								nxt[x - 1] += error/16.0f;
							nxt[x]     += 3*error/16.0f;
							cur[x + 1] += 5*error/16.0f;
							break;
						case 2:
							nxt[x + 1] += 5*error/16.0f;
							if (x > 0)
								nxt[x - 1] += 7*error/16.0f;
							nxt[x]     += error/16.0f;
							cur[x + 1] += 3*error/16.0f;
							break;
						case 3:
							nxt[x + 1] += 3*error/16.0f;
							if (x > 0)
								nxt[x - 1] += 5*error/16.0f;
							nxt[x]     += 7*error/16.0f;
							cur[x + 1] += error/16.0f;
							break;
					}
				}

				inputPrime[y, xClone] = cur[xClone] > 1.0 ? 0 : 255;
			}

			/* clip whole last line */
			for (int x = 0; x < w; x++)
				inputPrime[yClone, x] = nxt[x] > 1.0 ? 0 : 255;

			// Generate bitmap
			for (int y2=0; y2 < height; y2++)
				for (int x2=0; x2 < width; x2++)
					img.SetPixel(x2, y2, System.Drawing.Color.FromArgb(inputPrime[y2, x2], inputPrime[y2, x2], inputPrime[y2, x2]));         

			return img;
		}

	}
}
