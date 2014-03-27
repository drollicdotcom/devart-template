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
	public sealed class SegmentedNativeMomentsPainter : Painter, IDisposable
	{
		private int windowSize;  // the S parameter
				
		private Bitmap strokeBitmap = null;		

        //private bool renderingInProgress = false;

        private int width;
        private int height;
        private Bitmap[] originals;        

        /// <summary>
        /// Segmented painter. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="originals">These objects are not owned by this class</param>
        public SegmentedNativeMomentsPainter(int width, int height, Bitmap[] originals)
        {
            this.windowSize = 10;  // Default, safe window size
            this.strokeBitmap = Properties.Resources.brush1;  // Default brus
            this.width = width;
            this.height = height;
            this.originals = originals;            

        }

        /*
		public SegmentedNativeMomentsPainter(int windowSize, String strokeFilename)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = new Bitmap(strokeFilename);
		}

        public SegmentedNativeMomentsPainter(int windowSize, Bitmap stroke)
		{
			this.windowSize = windowSize;
			this.strokeBitmap = stroke;
		}
        */

        public override string  ToString()
        {
            return "Segmented Native Moments Painter";
        }

        /*
        private void StatusUpdater(object state)
        {
            //StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), SegmentedNativeMomentPainterWrapper.GetPaitingPercentComplete());

            while (renderingInProgress)
            {
                Thread.Sleep(100);
                //StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), SegmentedNativeMomentPainterWrapper.GetPaitingPercentComplete());
            }

            StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), 100);
        }
        */

        public override int GetPercentComplete()
        {
            return 0;
        }

        protected override Bitmap CreatePainting()
        {
            /*
            renderingInProgress = true;

            ThreadPool.QueueUserWorkItem(new WaitCallback(StatusUpdater));
            */

            // Transform the original images to the fast and unsafe bitmaps we require
            List<UnsafeBitmap> unsafeOriginals = new List<UnsafeBitmap>();
            foreach (Bitmap original in originals)
            {
                unsafeOriginals.Add(new UnsafeBitmap(new Bitmap(original)));
            }

            try
            {
                using (UnsafeBitmap unsafeBrush = new UnsafeBitmap(new Bitmap(Properties.Resources.brush1)))
                {
                    using (UnsafeBitmap unsafePainting = NativeMomentPainterWrapper.GenerateSegmentedCompositePainting(width, height, unsafeOriginals.ToArray(), BrushCollection.Instance.AvailableBrushes, this.windowSize))
                    {
                        unsafePainting.UnlockBitmap();
                        return new Bitmap(unsafePainting.Bitmap);
                    }
                }
            }
            finally
            {
                //renderingInProgress = false;

                // We're done with the unsafe originals, free them up
                foreach (UnsafeBitmap unsafeOriginal in unsafeOriginals)
                {
                    unsafeOriginal.Dispose();
                }

                // The originals were disposed of when the unsafe wrappers were 
                // disposed of.  So, simply null out this collection.
                this.originals = null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {            
        }

        #endregion
    }
}
