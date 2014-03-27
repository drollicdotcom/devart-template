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

namespace com.drollic.graphics
{
    public unsafe class UnsafeBitmap : IDisposable
    {
        Bitmap bitmap;
        Object bitmapLock = new Object();
        bool locked = false;
        bool disposed = false;

        // three elements used for MakeGreyUnsafe
        int width;
        BitmapData bitmapData = null;
        Byte* pBase = null;

        /// <summary>
        /// UnsafeBitmap Constructor.  This object takes ownership of the Bitmap
        /// passed it.  It will be disposed of when this object is disposed of.
        /// </summary>
        /// <param name="original"></param>
        public UnsafeBitmap(Bitmap original)
        {
            System.Diagnostics.Debug.Assert((original != null), "Bitmap is null");

            // We take ownership of this source Bitmap
            this.bitmap = original;

            LockBitmap();
        }

        private UnsafeBitmap(int width, int height)
        {
            this.bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }

        private Point PixelSize
        {
            get
            {
                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF bounds = bitmap.GetBounds(ref unit);

                return new Point((int)bounds.Width, (int)bounds.Height);
            }
        }

        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
             (int)boundsF.Y,
             (int)boundsF.Width,
             (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }
            bitmapData =
             bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            this.locked = true;

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }

        public int Width
        {
            get
            {
                return bitmap.Width;
            }
        }

        public int Height
        {
            get
            {
                return bitmap.Height;
            }
        }

        public RectangleF GetBounds(ref GraphicsUnit aPixel)
        {            
            return bitmap.GetBounds(ref aPixel);
        }

        public Color GetPixel(int x, int y)
        {
            PixelData data = *PixelAt(x, y);
            return Color.FromArgb(data.red, data.green, data.blue);            
        }

        public PixelData GetPixelData(int x, int y)
        {
            return *(PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }

        public void SetPixel(int x, int y, PixelData colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = colour;
        }


        public void UnlockBitmap()
        {
            lock (bitmapLock)
            {
                if (!this.disposed)
                {
                    this.bitmap.UnlockBits(bitmapData);
                    this.bitmapData = null;
                    this.pBase = null;
                    this.locked = false;
                }
            }
        }


        public PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }


        public void Dispose()
        {
            lock (bitmapLock)
            {
                if (!this.disposed)
                {
                    if (this.locked)
                    {
                        UnlockBitmap();
                    }

                    this.bitmap.Dispose();

                    this.disposed = true;
                }
            }
        }
    }


    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
    }
}
