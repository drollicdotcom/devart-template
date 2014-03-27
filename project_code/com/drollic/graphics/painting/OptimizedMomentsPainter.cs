using System;
//using System.IO;
using System.Drawing;
using System.Collections;
using System.Security.Permissions;

using com.drollic.util;
using com.drollic.graphics.painting.native.wrapper;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description
	/// </summary>
	public class OptimizedMomentsPainter : IPainter
	{
		private int windowSize;  // the S parameter
				
		private Bitmap strokeBitmap = null;

		private IDitheringAlgorithm dither = null;

		private static double d02 = 150*150;

        public OptimizedMomentsPainter()
        {
            this.windowSize = 15;  // Default, safe window size
            this.strokeBitmap = Properties.Resources.brush1;  // Default brush
        }

		public OptimizedMomentsPainter(int windowSize, String strokeFilename)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = new Bitmap(strokeFilename);
		}

        public OptimizedMomentsPainter(int windowSize, Bitmap stroke)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = stroke;
		}

        public override string  ToString()
        {
            return "Optimized Moments Painter - C#";
        }

		private ArrayList paint()
		{
			return null;
		}

		private ArrayList paint(Bitmap inImage, int winSize)
		{
			DateTime start = DateTime.Now;

			int S;			
			ArrayList list = null;
			int count;
			int factor, size, level;

			S = winSize; //_DEFS;

			System.Console.WriteLine("Generating stroke area image (optimized)");

            DateTime startAnalysis = DateTime.Now;

            // Managed calculation of stroke area image
			ColorImage areaImg = GenerateStrokeArea(inImage, S);

			// End Managed calculation

            // Unmanaged calculation of stroke area image  
            /*
            DateTime startAnalysis = DateTime.Now;
			Color[] colors = StrokeAreaGenerator.GenerateFast1(new UnsafeBitmap(inImage), S);

			ColorImage areaImg = new ColorImage(inImage.Width, inImage.Height);
			for (int y=0; y < inImage.Height; y++)
				for (int x=0; x < inImage.Width; x++)
					areaImg.singledata[y * inImage.Width + x] = colors[y * inImage.Width + x];
             */
            // End unmanaged calculation

            TimeSpan durationAnalysis = DateTime.Now - startAnalysis;
            StatusManager.Instance.StatusMessage("Analysis duration: " + durationAnalysis.TotalMilliseconds + "ms");
         

            //StatusReporter.Instance.SubmitWorkingImage(ImageTools.GenWorkingImage(areaImg));

			count = 0;
			list = new ArrayList();

			factor = 1;
			level = 1;
			size = System.Math.Max(inImage.Width, inImage.Height);
            DateTime startStrokeGen = DateTime.Now;
			while (size > 4*S) 
			{
				System.Console.WriteLine("Processing level " + factor);				
				count += GenerateStrokesWithRawScaling(inImage, areaImg, list, S, factor, level);
				size /= 2;
				factor *= 2;
				level++;
			}
            TimeSpan durationStrokeGen = DateTime.Now - startStrokeGen;
            StatusManager.Instance.StatusMessage("Stroke generation duration: " + durationStrokeGen.TotalMilliseconds + "ms");
         
			list.Sort();

			TimeSpan duration = DateTime.Now - start;
            StatusManager.Instance.StatusMessage("Duration of analysis, stroke gen and sorting: " + duration.TotalMilliseconds + "ms");

			return list;
		}


		private int GenerateStrokesWithRawScaling(Bitmap input, ColorImage area, ArrayList list, int S, int factor, int level)
		{			
			int x, y;

			System.Console.WriteLine("Generating stroke positions image.");

			/*
			Bitmap areascaled = new Bitmap(area.Width / factor, area.Height / factor);
			Graphics g = Graphics.FromImage(areascaled);   
			g.ScaleTransform(1.0f / (float)factor, 1.0f / (float)factor);   
			g.DrawImage(area, 0, 0, area.Width, area.Height);   
			areascaled.Save("areascaled-" + factor.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
			ColorImage img = new ColorImage(areascaled);
			*/
			
			ColorImage scaledWidth = ColorImage.ScaleWidth(area, area.Width / factor);
			ColorImage img = ColorImage.ScaleHeight(scaledWidth, area.Height / factor);
		
			this.dither = new FloydSteinbergDither(4*S/System.Math.Sqrt(level), 2.0f/level);

			UnsafeBitmap dithered = new UnsafeBitmap(this.dither.Dither(img));

			//StatusReporter.Instance.SubmitWorkingImage(ImageTools.GenWorkingImage(dithered));
			//dithered.Save("dithered-" + factor.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
   
			Bitmap inscaled = new Bitmap(input.Width / factor, input.Height / factor);
			Graphics g2 = Graphics.FromImage(inscaled);   
			g2.ScaleTransform(1.0f / (float)factor, 1.0f / (float)factor);   
			g2.DrawImage(input, 0, 0, input.Width, input.Height);   
			//inscaled.Save("inputscaled-" + factor.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

			System.Console.WriteLine("Collecting strokes.");

			int maxY = dithered.Height; 
			int maxX = dithered.Width;

			ColorImage inScaled = new ColorImage(new UnsafeBitmap(inscaled));  // should be &inputscaled
			for (y = 0; y < maxY; y++) 
			{
				for (x = 0; x < maxX; x++) 
				{
					if (dithered.GetPixel(x, y).R == 0)
					{            
						//StatusReporter.Instance.SubmitWorkingImage(ImageTools.GenWorkingImage(dithered, x, y, S, S));
						Stroke s = GenerateStroke(inScaled, x, y, S, factor, level);
						list.Add(s);				
					}			
				}
			}	

			return list.Count;
		}


		private int GenerateStrokes(Bitmap input, Bitmap area, ArrayList list, int S, int factor, int level)
		{			
			int x, y;

			System.Console.WriteLine("Generating stroke positions image.");

			Bitmap areascaled = new Bitmap(area.Width / factor, area.Height / factor);
			Graphics g = Graphics.FromImage(areascaled);   
			g.ScaleTransform(1.0f / (float)factor, 1.0f / (float)factor);   
			g.DrawImage(area, 0, 0, area.Width, area.Height);   
			areascaled.Save("areascaled-" + factor.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
  
			ColorImage img = new ColorImage(new UnsafeBitmap(areascaled));

			this.dither = new FloydSteinbergDither(4*S/System.Math.Sqrt(level), 2.0f/level);

            UnsafeBitmap dithered = new UnsafeBitmap(this.dither.Dither(img));

			//StatusReporter.Instance.SubmitWorkingImage(ImageTools.GenWorkingImage(dithered));
			//dithered.Save("dithered-" + factor.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
   
			Bitmap inscaled = new Bitmap(input.Width / factor, input.Height / factor);
			Graphics g2 = Graphics.FromImage(inscaled);   
			g2.ScaleTransform(1.0f / (float)factor, 1.0f / (float)factor);   
			g2.DrawImage(input, 0, 0, input.Width, input.Height);   
			//inscaled.Save("inputscaled-" + factor.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

			System.Console.WriteLine("Collecting strokes.");

			int maxY = dithered.Height; 
			int maxX = dithered.Width;

			ColorImage inScaled = new ColorImage(new UnsafeBitmap(inscaled));  // should be &inputscaled
			for (y = 0; y < maxY; y++) 
			{
				for (x = 0; x < maxX; x++) 
				{
					if (dithered.GetPixel(x, y).R == 0)
					{            
						//StatusReporter.Instance.SubmitWorkingImage(ImageTools.GenWorkingImage(dithered, x, y, S, S));
						Stroke s = GenerateStroke(inScaled, x, y, S, factor, level);
						list.Add(s);				
					}			
				}
			}	

			return list.Count;
		}


		public double MomentI(Color bgrxy, Color bgr)
		{
			double d2, r2;

			// RSM: This shouldn't always be assumed.  We should square the d0 value
			// once at startup... whatever it may be!
			//double d02 = 150*150;

			d2 = ImageTools.ColorDistance(bgrxy, bgr);

			if (d2 >= OptimizedMomentsPainter.d02)
				return 0.0;

			r2 = d2 / d02;

			return ((1 - r2) * (1 - r2));
		}


		double Moment00(ColorImage input, Color bgr, StreamWriter writer)
		{
			double m = 0.0;
			int lineCount = 0;

			DateTime start = DateTime.Now;

			for (int i = 0; i < input.Height; i++) 
			{
				for (int j = 0; j < input.Width; j++) 
				{
					Color c = input.GetColor(j, i);

					m += MomentI (c, bgr);

					//String* s1 = String::Concat("  c=", c.R.ToString(), String::Concat(",", c.G.ToString(), ",", c.B.ToString()));
					//writer->WriteLine(String::Concat(lineCount.ToString(), s1, ": m=", m.ToString("000.00")));

					lineCount++;
				}
			}

			//TimeSpan duration = DateTime.Now - start;
			//if (duration.TotalMilliseconds > 20)
			//	System.Console.WriteLine("Moment00 completed in " + duration.TotalMilliseconds + "ms");

			return m;
		}


		public ColorImage GenerateStrokeArea(Bitmap input, int s)
		{
            FastColorImage piece = new FastColorImage();
			double m;
			ArrayList list = new ArrayList();
			double pixVal;

			DateTime start = DateTime.Now;

			//Bitmap img = new Bitmap(input.Width, input.Height);
			ColorImage cImg = new ColorImage(input.Width, input.Height);

			StreamWriter writer = null; //new StreamWriter("new.txt");

            FastColorImage inMem = new FastColorImage(new UnsafeBitmap(input));					

			for (int i = 0; i < input.Height; i++) 
			{				
				for (int j = 0; j < input.Width; j++) 
				{						
					inMem.FastCrop(j, i, s, s, ref piece);			         
                     
					System.Drawing.Color topColor = inMem.GetColor(j, i);

					m = Moment00(piece, topColor, writer);	                 

					pixVal = (255.0 * m / (piece.Width * piece.Height));
					
					cImg.SetColor(j, i, Color.FromArgb((int)pixVal, (int)pixVal, (int)pixVal));
				}
			}

			//img.Save("area.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
   
			TimeSpan duration = DateTime.Now - start;
			System.Console.WriteLine("Stroke area computed in " + duration.TotalMilliseconds + "ms");

			return cImg;
		}

		public int ComputeMoments(ColorImage input, Color bgr, int S, ref double m00, ref double m01, ref double m10, ref double m11, ref double m02, ref double m20)
		{
			double Ival;

			m00 = m01 = m10 = m11 = m02 = m20 = 0;

			for (int y = 1; y <= S; y++) 
			{
				for (int x = 1; x <= S; x++) 
				{
					Ival = 0.0; 
					if ((x < input.Width) && (y < input.Height))
					{
						Color c = input.GetColor(x, y);
						Ival = MomentI(c, bgr) * 255.0;
					}

					m00 += Ival;
					m01 += y * Ival;
					m10 += x * Ival;
					m11 += y * x * Ival;
					m02 += y * y * Ival;
					m20 += x * x * Ival;
				}
			}

			return 1;
		}


		public Stroke GenerateStroke(ColorImage input, int x, int y, int s, int factor, int level) 
		{
			ArrayList list = new ArrayList();
			double m00, m01, m10, m11, m02, m20;
			double a, b, c;
			double tempval;
			double dw, dxc, dyc;
			int xc, yc;
			double theta;
			float w, l;

			m00 = m01 = m10 = m11 = m02 = m20 = 0;

			System.Drawing.Color firstColor = input.GetColor(x, y); //System::Drawing::Color::FromArgb((int)color[0], (int)color[1], (int)color[2]);

			ColorImage cropImage = input.Crop(x, y, s, s);

			ComputeMoments(cropImage, firstColor, s, ref m00, ref m01, ref m10, ref m11, ref m02, ref m20);

			dxc = m10 / m00;
			dyc = m01 / m00;
			a = (m20 / m00) - (double)((dxc)*(dxc));
			b = 2 * (m11 / m00 - (double)((dxc)*(dyc)));
			c = (m02 / m00) - (double)((dyc)*(dyc));
			theta = System.Math.Atan2(b, (a-c)) / 2;
			tempval = System.Math.Sqrt(b*b + (a-c)*(a-c));
			dw = System.Math.Sqrt(6 * (a+c - tempval));
			w = (float)(System.Math.Sqrt(6 * (a+c - tempval)));
			l = (float)(System.Math.Sqrt(6 * (a+c + tempval)));
			xc = (int)(x + System.Math.Round(dxc - s/2)); 
			yc = (int)(y + System.Math.Round(dyc - s/2)); 

			Stroke stroke = new Stroke();
			stroke.xc = factor*xc;
			stroke.yc = factor*yc;
			stroke.w = factor*w;
			stroke.l = factor*l;
			stroke.theta = (float) theta;
			stroke.red = firstColor.R;
			stroke.green = firstColor.G;
			stroke.blue = firstColor.B;
			stroke.level = level;

			return stroke;
		}

		#region IPainter Members


		public ArrayList GenerateStrokes(Bitmap originalImage)
		{
			ArrayList strokes = paint(originalImage, this.windowSize);			
			return strokes;
		}

        public Bitmap RenderX(ArrayList strokes, Bitmap paintingBitmap)
        {
            ColorImage strokeImage = new ColorImage(new UnsafeBitmap(strokeBitmap));
            int strokeCount = 0;

            Graphics gr = Graphics.FromImage(paintingBitmap);
            gr.Clear(System.Drawing.Color.White);
            ColorImage painting = new ColorImage(new UnsafeBitmap(paintingBitmap));

            double OneEightyOverPi = 180.0f / System.Math.PI;

            Hashtable scaledStrokeHash = new Hashtable();
            Hashtable scaledWidthHash = new Hashtable();

            TimeSpan durationGDI = TimeSpan.Zero;
            TimeSpan durationBlending = TimeSpan.Zero;
            TimeSpan durationScaling = TimeSpan.Zero;

            DateTime start = DateTime.Now;

            foreach (Stroke stroke in strokes)
            {
                double radians = stroke.theta * -1.0;
                //int max = System.Math.Max((int)stroke.l, (int)stroke.w);

                if ((float.IsNaN(stroke.l)) || (float.IsNaN(stroke.w)))
                    continue;

                int scaledWidth = (int)stroke.l;
                int scaledHeight = (int)stroke.w;

                if ((scaledWidth == 0) || (scaledHeight == 0))
                    continue;

                DateTime startScaling = DateTime.Now;
                Point p = new Point(scaledWidth, scaledHeight);
                Bitmap newStrokeBmp = null;
                if (scaledStrokeHash.ContainsKey(p))
                {
                    newStrokeBmp = (Bitmap)scaledStrokeHash[p];
                }
                else
                {

                    ColorImage temp1;
                    if (scaledWidthHash.ContainsKey(scaledWidth))
                    {
                        temp1 = (ColorImage)scaledWidthHash[scaledWidth];
                    }
                    else
                    {
                        temp1 = ColorImage.ScaleWidth(strokeImage, scaledWidth);
                        scaledWidthHash[scaledWidth] = temp1;
                    }

                    temp1 = ColorImage.ScaleWidth(strokeImage, scaledWidth);
                    ColorImage scaledImage = ColorImage.ScaleHeight(temp1, scaledHeight);
                    newStrokeBmp = scaledImage.GenerateBitmap();
                    scaledStrokeHash[p] = newStrokeBmp;
                }
                durationScaling += DateTime.Now - startScaling;

                DateTime startGDI = DateTime.Now;
                Bitmap sbmp = new Bitmap(newStrokeBmp.Width, newStrokeBmp.Height);
                Graphics g = Graphics.FromImage(sbmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;// HighQualityBicubic;		
                g.TranslateTransform(-newStrokeBmp.Width / 2, -newStrokeBmp.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);
                g.RotateTransform((float)(radians * OneEightyOverPi), System.Drawing.Drawing2D.MatrixOrder.Append);
                g.TranslateTransform(newStrokeBmp.Width / 2, newStrokeBmp.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);
                g.DrawImage(newStrokeBmp, 0, 0, newStrokeBmp.Width, newStrokeBmp.Height);
                //sbmp.Save("newstroke" + stroke.theta.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                sbmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                Color c = Color.FromArgb(stroke.red, stroke.green, stroke.blue);
                durationGDI += DateTime.Now - startGDI;

                DateTime startBlending = DateTime.Now;
                ColorImage.Blend(ref painting, new ColorImage(new UnsafeBitmap(sbmp)), c, stroke.xc, stroke.yc);
                durationBlending += DateTime.Now - startBlending;

                //painting.GenerateBitmap().Save("intermediate-" + strokeCount + "n.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                strokeCount++;

                //StatusReporter.Instance.SetPercentComplete((int)(((float)strokeCount / (float)(strokes.Count)) * 100.0));
            }

            /*
            foreach (Point p in h.Keys)
            {
                Console.WriteLine("Stroke size (" + p.X + "," + p.Y + ") = " + (int)h[p]);
            }
            */

            TimeSpan duration = DateTime.Now - start;
            StatusManager.Instance.StatusMessage("Total GDI duration: " + durationGDI.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Total Scaling duration: " + durationScaling.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Total Blending duration: " + durationBlending.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Rendering duration: " + duration.TotalMilliseconds + "ms");


            //bmp.Save("newpainting.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);	

            return painting.GenerateBitmap(); ;
        }

		public Bitmap Render(ArrayList strokes, Bitmap paintingBitmap)
		{			
			ColorImage strokeImage = new ColorImage(new UnsafeBitmap(strokeBitmap));
			int strokeCount = 0;		
			
			Graphics gr = Graphics.FromImage(paintingBitmap);
			gr.Clear(System.Drawing.Color.White);
			ColorImage painting = new ColorImage(new UnsafeBitmap(paintingBitmap)); 

			double OneEightyOverPi = 180.0f / System.Math.PI;

			Hashtable scaledStrokeHash = new Hashtable();
			Hashtable scaledWidthHash = new Hashtable();

            TimeSpan durationGDI = TimeSpan.Zero;
            TimeSpan durationBlending = TimeSpan.Zero;
            TimeSpan durationScaling = TimeSpan.Zero;

            DateTime start = DateTime.Now;

			foreach (Stroke stroke in strokes)
			{
				double radians = stroke.theta * -1.0;
				//int max = System.Math.Max((int)stroke.l, (int)stroke.w);

				if ((float.IsNaN(stroke.l)) || (float.IsNaN(stroke.w)))
					continue;
			
				int scaledWidth = (int)stroke.l;
				int scaledHeight = (int)stroke.w;

				if ((scaledWidth == 0) || (scaledHeight == 0))
					continue;				

                DateTime startScaling = DateTime.Now;
                Point p = new Point(scaledWidth, scaledHeight);
				Bitmap newStrokeBmp = null;
				if (scaledStrokeHash.ContainsKey(p))
				{
					newStrokeBmp = (Bitmap)scaledStrokeHash[p];
				}
				else
				{
					
					ColorImage temp1;
					if (scaledWidthHash.ContainsKey(scaledWidth))
					{
						temp1 = (ColorImage)scaledWidthHash[scaledWidth];						
					}
					else
					{
						temp1 = ColorImage.ScaleWidth(strokeImage, scaledWidth);
						scaledWidthHash[scaledWidth] = temp1;
					}
										
					temp1 = ColorImage.ScaleWidth(strokeImage, scaledWidth);
					ColorImage scaledImage = ColorImage.ScaleHeight(temp1, scaledHeight);
					newStrokeBmp = scaledImage.GenerateBitmap();
					scaledStrokeHash[p] = newStrokeBmp;
				}
                durationScaling += DateTime.Now - startScaling;

                DateTime startGDI = DateTime.Now;                
				Bitmap sbmp = new Bitmap(newStrokeBmp.Width, newStrokeBmp.Height);
				Graphics g = Graphics.FromImage(sbmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;// HighQualityBicubic;		
				g.TranslateTransform(- newStrokeBmp.Width / 2, - newStrokeBmp.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);				
				g.RotateTransform((float)(radians * OneEightyOverPi), System.Drawing.Drawing2D.MatrixOrder.Append);
				g.TranslateTransform(newStrokeBmp.Width / 2, newStrokeBmp.Height / 2, System.Drawing.Drawing2D.MatrixOrder.Append);								
				g.DrawImage(newStrokeBmp, 0, 0, newStrokeBmp.Width, newStrokeBmp.Height);   
				//sbmp.Save("newstroke" + stroke.theta.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
				sbmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
				Color c = Color.FromArgb(stroke.red, stroke.green, stroke.blue);
                durationGDI += DateTime.Now - startGDI;

                DateTime startBlending = DateTime.Now;
				ColorImage.Blend(ref painting, new ColorImage(new UnsafeBitmap(sbmp)), c, stroke.xc, stroke.yc);
                durationBlending += DateTime.Now - startBlending;

				//painting.GenerateBitmap().Save("intermediate-" + strokeCount + "n.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
				
				strokeCount++;

				//StatusReporter.Instance.SetPercentComplete((int)(((float)strokeCount / (float)(strokes.Count)) * 100.0));
			}

			/*
			foreach (Point p in h.Keys)
			{
				Console.WriteLine("Stroke size (" + p.X + "," + p.Y + ") = " + (int)h[p]);
			}
			*/

            TimeSpan duration = DateTime.Now - start;
            StatusManager.Instance.StatusMessage("Total GDI duration: " + durationGDI.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Total Scaling duration: " + durationScaling.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Total Blending duration: " + durationBlending.TotalMilliseconds + "ms");
            StatusManager.Instance.StatusMessage("Rendering duration: " + duration.TotalMilliseconds + "ms");


			//bmp.Save("newpainting.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);	

			return painting.GenerateBitmap();;
		}

		public Bitmap CreatePainting(Bitmap originalImage)
		{
			ArrayList strokes = paint(originalImage, this.windowSize);			
			return Render(strokes, new Bitmap(originalImage.Width, originalImage.Height));
		}

		#endregion
	}
}
