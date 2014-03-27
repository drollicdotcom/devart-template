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
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace com.drollic.graphics
{
	/// <summary>
	/// Summary description for ImageProcessing.
	/// </summary>
	public sealed class ImageTools
	{
        public static Point PixelSize(UnsafeBitmap bitmap)
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            return new Point((int)bounds.Width, (int)bounds.Height);
        }

        public static double[] ExtractGreyValues(UnsafeBitmap bitmap)
        {
            Point size = PixelSize(bitmap);
            double[] data = new double[size.Y * size.X];

            int count = 0;
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    double dvalue = (0.299 * c.R) + (0.587 * c.G) + (0.114 * c.B);
                    data[count++] = dvalue;
                }
            }

            return data;
        }


        /// <summary>
        /// This method will generate an image from the provided data.  It will 
        /// also zero out any values that fall below the lowBoundary parameter.
        /// This is a good way to eliminate a lot of noise.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="lowBoundary"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap generateImage(int[] data, int lowBoundary, int width, int height)
        {
            int count = 0;
            int value = 0;
            Bitmap result = null;

            using (UnsafeBitmap bitmap = new UnsafeBitmap(new Bitmap(width, height)))
            {
                PixelData pdata;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        value = data[count++];

                        if (value < lowBoundary)
                        {
                            value = 0;
                        }

                        pdata.red = pdata.green = pdata.blue = (byte)value;
                        bitmap.SetPixel(x, y, pdata);
                    }
                }

                bitmap.UnlockBitmap();
                result = new Bitmap(bitmap.Bitmap);
            }

            return result;
        }
		


        public static double[] rotate90Clockwise(double[] data, int imageWidth)
        {
            int len = data.Length;
            int count = 0;
            double[] tData = new double[len];

            for (int i = (len - imageWidth); i < len; i++)
                for (int j = i; j >= 0; j -= imageWidth)
                    tData[count++] = data[j];

            return tData;
        }

        public static double[] rotate90CounterClockwise(double[] data, int imageWidth)
        {
            int len = data.Length;
            double[] tData = new double[len];
            int count = 0;

            for (int ii = (imageWidth - 1); ii >= 0; ii--)
                for (int jj = ii; jj < len; jj += imageWidth)
                    tData[count++] = data[jj];

            return tData;
        }


		public static double ColorDistance(Color bgr1, Color bgr2)
		{
			int dr = bgr1.R - bgr2.R;
			int dg = bgr1.G - bgr2.G;
			int db = bgr1.B - bgr2.B;
			
			return dr * dr + dg * dg + db * db;
		}

		public static Bitmap GenWorkingImage(Bitmap image)
		{
			return new Bitmap(image);
		}

		public static Bitmap GenWorkingImage(Bitmap image, int x, int y, int width, int height)
		{
			Bitmap img = new Bitmap(image);

			int x2 = System.Math.Min(x+width, img.Width-1);
			int y2 = System.Math.Min(y+height, img.Height-1);

			for (int i=x; i < x2; i++)
			{
				img.SetPixel(i, y, System.Drawing.Color.Lime);
				img.SetPixel(i, y2, System.Drawing.Color.Lime);
			}
			for (int j=y; j < y2; j++)
			{
				img.SetPixel(x, j, System.Drawing.Color.Lime);
				img.SetPixel(x2, j, System.Drawing.Color.Lime);
			}

			return img;
		}


        public static Bitmap ScaleByPercent(Bitmap imgPhoto, int percent)
        {
            Bitmap bmPhoto = null;
            Graphics grPhoto = null;

            try
            {
                float nPercent = ((float)percent / 100.0f);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;
                int sourceX = 0;
                int sourceY = 0;

                int destX = 0;
                int destY = 0;
                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                // Sanity check.  If we've reduced size to zero, return null
                if ((destWidth > 0) && (destHeight > 0))
                {
                    bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
                    bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                    grPhoto = Graphics.FromImage(bmPhoto);
                    grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

                    grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);                    
                }
            }
            catch (Exception)
            {
                bmPhoto = null;
            }
            finally
            {
                if (grPhoto != null)
                {
                    grPhoto.Dispose();
                }
            }

            return bmPhoto;
        }

        public static Bitmap ScaleBySize(Bitmap imgPhoto, int destWidth, int destHeight)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        public enum AnchorPosition
        {
            Top,
            Bottom,
            Left,
            Right,
            Center
        };

        private static Bitmap ScaleWithCrop(Image imgPhoto, int Width, int Height, AnchorPosition Anchor)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (Anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)
                            (Height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)
                            ((Height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (Anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int)
                          (Width - (sourceWidth * nPercent));
                        break;
                    default:
                        destX = (int)
                          ((Width - (sourceWidth * nPercent)) / 2);
                        break;
                }
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width,
                    Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                    imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }


        /// <summary>
        /// This method will resize an image to roughly the desired size, but while maintaining
        /// aspect ratio.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxImageWidth"></param>
        /// <param name="maxImageHeight"></param>
        /// <returns></returns>
		public static Bitmap ResizeImage(Bitmap image, int maxImageWidth, int maxImageHeight)
		{
			int maxht = maxImageHeight;
			int maxwt = maxImageWidth;
			double maxaspect = (double)maxht / (double)maxwt;

			int imght = image.Height;
			int imgwt = image.Width;

			Bitmap resized = image;

			if (image.Width > maxwt || image.Height > maxht) 
			{
				double theAspect = (double)image.Height / (double)image.Width;

				if (theAspect > maxaspect) 
				{
					// Image has taller aspect ratio than max rectangle
					imght = maxht;
					imgwt = (int)(maxht/theAspect + 0.5);
				}
				else
				{
					// Image has wider aspect ratio than max rectangle
					imght = (int)((maxwt*theAspect) + 0.5);
					imgwt = maxwt;
				}

				resized = (Bitmap)image.GetThumbnailImage(imgwt, imght, null, System.IntPtr.Zero);
			}

			return resized;
		}

        private static Bitmap ResizeImageIgnoreAspect(Bitmap image, int width, int height)
        {
            return (Bitmap)image.GetThumbnailImage(width, height, null, System.IntPtr.Zero);
        }
	}
}
