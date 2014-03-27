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
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;

using com.drollic.util;
using com.drollic.graphics.painting.native.wrapper;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for NativeMomentsPainter.
	/// </summary>
	public sealed class NativeMomentsPainter : Painter, IDisposable
	{
		private int windowSize;  // the S parameter				

        private bool renderingInProgress = false;


        public NativeMomentsPainter()
        {
            this.windowSize = 10;  // Default, safe window size
        }

        public override int GetPercentComplete()
        {
            return 0;
        }

        /*
		public NativeMomentsPainter(int windowSize, String strokeFilename)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = new Bitmap(strokeFilename);
		}

        public NativeMomentsPainter(int windowSize, Bitmap stroke)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = stroke;
		}
        */

        public override string  ToString()
        {
            return "Native Moments Painter - Cleaned Up";
        }

        /*
		private ArrayList paint()
		{
			return null;
		}

        public Hashtable GenerateTestImages(Bitmap inImage)
        {
            return null;
        }
         * */

		#region IPainter Members

        private void StatusUpdater(object state)
        {
            StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), NativeMomentPainterWrapper.GetPaitingPercentComplete());

            while (renderingInProgress)
            {
                Thread.Sleep(100);
                StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), NativeMomentPainterWrapper.GetPaitingPercentComplete());
            }

            StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), 100);
        }

        protected override Bitmap CreatePainting()
        {
            return null;
        }

        public Bitmap CreatePainting(int width, int height, Bitmap[] originals, Point[] locations)
        {
            return CreatePaintingWithLocations(width, height, originals, locations);
        }

        private Bitmap CreatePaintingWithLocations(int width, int height, Bitmap[] originals, Point[] locations)
        {
            renderingInProgress = true;    
           
            ThreadPool.QueueUserWorkItem(new WaitCallback(StatusUpdater));

            // Transform the original images to the fast and unsafe bitmaps we require
            List<UnsafeBitmap> unsafeOriginals = new List<UnsafeBitmap>();
            foreach (Bitmap original in originals)
            {
                unsafeOriginals.Add(new UnsafeBitmap(new Bitmap(original)));
            }                    

            try
            {
                using (UnsafeBitmap unsafeBrush = new UnsafeBitmap((Bitmap)Properties.Resources.brush1.Clone()))
                {                                    
                    using (UnsafeBitmap unsafePainting = NativeMomentPainterWrapper.GenerateCompositePainting(width, height, unsafeOriginals.ToArray(), locations, BrushCollection.Instance.AvailableBrushes, this.windowSize))
                    {                        
                        unsafePainting.UnlockBitmap();
                        return new Bitmap(unsafePainting.Bitmap);
                    }
                }
            }
            finally
            {
                renderingInProgress = false;

                // We're done with the unsafe originals, free them up
                foreach (UnsafeBitmap unsafeOriginal in unsafeOriginals)
                {
                    unsafeOriginal.Dispose();
                }
            }
        }

        /*
		public Bitmap CreatePainting(Bitmap originalImage)
		{
            using (UnsafeBitmap unsafeOriginal = new UnsafeBitmap(originalImage))
            {
                using (UnsafeBitmap unsafeBrush = new UnsafeBitmap(Properties.Resources.brush1.Clone()))
                {
                    using (UnsafeBitmap unsafePainting = NativeMomentPainterWrapper.GeneratePainting(unsafeOriginal, unsafeBrush, this.windowSize))
                    {
                        unsafePainting.UnlockBitmap();
                        return unsafePainting.Bitmap.Clone();
                    }
                }
            }
        }
         */

		#endregion

        #region IPainter Members

        public Bitmap Render(ArrayList strokes, Bitmap composition)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ArrayList GenerateStrokes(Bitmap original)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
