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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace com.drollic.graphics.painting
{
    class FastColorImage : ColorImage
    {
        private int xOffset;
        private int yOffset;
        private int originalWidth;

        public FastColorImage()
        {
        }

        public FastColorImage(UnsafeBitmap img) : base(img)
        {
            
        }

        public override Color GetColor(int x, int y)
        {
            return this.singledata[((yOffset + y) * this.originalWidth) + (x + xOffset)];
        }

        public override void SetColor(int x, int y, Color data)
        {
            this.singledata[(yOffset + y) * this.Width + (x + xOffset)] = data;
        }

        public void FastCrop(int xc, int yc, int w, int h, ref FastColorImage result)
        {
            int x0, y0;

            /* adjust region to fit in source image */
            x0 = xc - w / 2;
            y0 = yc - h / 2;
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

            result.Width = w;
            result.Height = h;
            result.xOffset = x0;
            result.yOffset = y0;
            result.originalWidth = this.Width;
            result.singledata = this.singledata;
        }

    }
}
