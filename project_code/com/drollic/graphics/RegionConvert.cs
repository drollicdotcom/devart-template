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
using System.Drawing.Drawing2D;
using System.Security.Permissions;

namespace com.drollic.graphics
{
	public sealed class RegionConvert 
	{
		public static Region ConvertFromTransparentBitmap(UnsafeBitmap imageRegion, Color transparentColor) 
		{
			// First we get the dimensions of our image
			GraphicsUnit aPixel = GraphicsUnit.Pixel;
			RectangleF imageBoundsF = imageRegion.GetBounds(ref aPixel);
			int imageWidth = Convert.ToInt32(imageBoundsF.Width);
			int imageHeight = Convert.ToInt32(imageBoundsF.Height);

			// This will be the path for our Region
			GraphicsPath regionPath = new GraphicsPath();

			// We loop over every line in our image, and every pixel per line
			for (int intY = 0; intY < imageHeight; intY++) 
			{
				for (int intX = 0; intX < imageWidth; intX++) 
				{	
					if (imageRegion.GetPixel(intX, intY) != transparentColor) 
					{
						// We have to see this pixel!
						regionPath.AddRectangle(new Rectangle(intX, intY, 1, 1));
					}
				}
			}
			
			Region formRegion = new Region(regionPath);
			regionPath.Dispose();
			return formRegion;
		} /* ConvertFromTransparentBitmap */
	} /* RegionConvert */
}
