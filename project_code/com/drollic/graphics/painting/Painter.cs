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
using System.Security.Permissions;
using System.Drawing.Imaging;
using System.Threading;

using com.drollic.util;

namespace com.drollic.graphics.painting
{
	/// <summary>
	/// Summary description for IPainter.
	/// </summary>
	public abstract class Painter : IDisposable
	{
        protected bool generateMovie = false;

        protected static readonly String artist = "Created with the Drollic Dreamer, http://www.drollic.com";

        protected bool renderingInProgress = false;

        protected abstract Bitmap CreatePainting();

        public abstract int GetPercentComplete();

        public List<Bitmap> introMovieFrames = null;

        private String movieFilename = null;


        public String MovieFilename
        {
            set
            {
                this.movieFilename = value;
            }
            get
            {
                return this.movieFilename;
            }
        }

        public bool GenerateMovie
        {
            set
            {
                this.generateMovie = value;
            }
            get
            {
                return this.generateMovie;
            }
        }

        public List<Bitmap> IntroMovieFrames
        {
            set
            {
                this.introMovieFrames = value;
            }
        }

        private void StatusUpdater(object state)
        {
            StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), GetPercentComplete());

            while (renderingInProgress)
            {
                Thread.Sleep(100);
                StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), GetPercentComplete());
            }

            StatusManager.Instance.SetTaskPercentCompletion(this.GetHashCode(), 100);
        }
        

        public Bitmap Paint()
        {
            Bitmap painting = null;

            try
            {
                // Mark rendering as in progress, which will be used by the status
                // updater thread to determine if it should continue running.
                renderingInProgress = true;

                // Get the status reporter going
                ThreadPool.QueueUserWorkItem(new WaitCallback(StatusUpdater));

                // Create the painting
                painting = CreatePainting();


                // Attempt to set the metadata on the painting.  We have to do this
                // in a crazy way since there's no constructor for the PropertyItem
                // class.
                int[] propIds = Properties.Resources.propholder.PropertyIdList;
                foreach (int id in propIds)
                {
                    PropertyItem item = Properties.Resources.propholder.GetPropertyItem(id);
                    if (item.Type == 2)
                    {
                        try
                        {
                            // Now set the metadata on the painting                
                            byte[] bArtist = new Byte[artist.Length];

                            // copy description into byte array
                            for (int i = 0; i < artist.Length; i++) bArtist[i] = (byte)artist[i];

                            item = Properties.Resources.propholder.GetPropertyItem(id);
                            item.Type = 2;    // Identifies data as null terminated ASCII string
                            item.Len = artist.Length;
                            item.Value = bArtist;
                            painting.SetPropertyItem(item);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                renderingInProgress = false;
            }

            return painting;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Nothing to be done here
        }

        #endregion
    }
}
