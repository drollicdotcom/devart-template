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
    /// Summary description for CompositeNativeMomentsPainter.
	/// </summary>
	public sealed class CompositeNativeMomentsPainter : Painter
	{
		private int windowSize;  // the S parameter					

        private int width;

        private int height;

        private Bitmap[] originals;

        private Point[] locations;


        /// <summary>
        /// Composite painter, native code.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="originals">These objects are not owned by this class</param>
        /// <param name="locations"></param>
        public CompositeNativeMomentsPainter(int width, int height, Bitmap[] originals, Point[] locations)
        {
            this.windowSize = 15;  // Default, safe window size
            this.width = width;
            this.height = height;
            this.originals = originals;
            this.locations = locations;
        }

        public override string  ToString()
        {
            return "Composite Native Moments Painter";
        }

		private ArrayList paint()
		{
			return null;
		}

        public Hashtable GenerateTestImages(Bitmap inImage)
        {
            return null;
        }

		#region IPainter Members

        public override int GetPercentComplete()
        {
            return CompositeNativeMomentPainterWrapper.GetPercentComplete();
        }


        protected override Bitmap CreatePainting()
        {            
            // Transform the original images to the fast and unsafe bitmaps we require
            List<UnsafeBitmap> unsafeOriginals = new List<UnsafeBitmap>();
            foreach (Bitmap original in this.originals)
            {
                unsafeOriginals.Add(new UnsafeBitmap(new Bitmap(original)));
            }

            List<UnsafeBitmap> unsafeMovieIntroFrames = new List<UnsafeBitmap>();
            if (this.introMovieFrames != null)
            {
                foreach (Bitmap originalIntroFrame in this.introMovieFrames)
                {
                    unsafeMovieIntroFrames.Add(new UnsafeBitmap(new Bitmap(originalIntroFrame)));
                }
            }

            try
            {
                using (UnsafeBitmap unsafePainting = CompositeNativeMomentPainterWrapper.CreatePainting(width, height, unsafeOriginals.ToArray(), this.locations, BrushCollection.Instance.AvailableBrushes, this.windowSize, this.generateMovie, unsafeMovieIntroFrames.ToArray(), this.MovieFilename))
                {
                    unsafePainting.UnlockBitmap();
                    return new Bitmap(unsafePainting.Bitmap);
                }
            }
            finally
            {
                // We're done with the unsafe originals, free them up.  This will also
                // dispose of the originals
                foreach (UnsafeBitmap unsafeOriginal in unsafeOriginals)
                {
                    unsafeOriginal.Dispose();
                }

                // Dispose of the unsafe movie title frame, if it was created
                foreach (UnsafeBitmap unsafeIntroFrame in unsafeMovieIntroFrames)
                {
                    unsafeIntroFrame.Dispose();
                }

                // The originals were disposed of when the unsafe wrappers were 
                // disposed of.  So, simply null out this collection.
                this.originals = null;
            }
        }

		#endregion
    }
}
