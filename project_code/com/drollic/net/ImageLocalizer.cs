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
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Security.Permissions;

using com.drollic.util;

namespace com.drollic.net
{
    public sealed class LocalizationStatus
    {
        public String uri;
        public int bytesRead;
        public int totalBytes;
        public DateTime started;
    }

    /// <summary>
    /// Summary description for ImageLocalizer.
    /// </summary>
    public sealed class ImageLocalizer
    {
        //private static ImageLocalizer instance = new ImageLocalizer();

        private static int MaxDownloadsAllowed = 3;

        public delegate void ImageLocalizedDelegate(Bitmap image);
        public event ImageLocalizedDelegate ImageLocalizedEvent;

        Dictionary<String, LocalizationStatus> status = new Dictionary<string, LocalizationStatus>();

        private List<DownloadedImage> downloadedImages = new List<DownloadedImage>();
        private int totalDownloaded;
        private int totalDesired;
        private int activeDownloads;
        private Queue<String> urls;

        AutoResetEvent[] allWaitHandles = new AutoResetEvent[MaxDownloadsAllowed];
        Queue<AutoResetEvent> freeWaitHandles = new Queue<AutoResetEvent>();

        public ImageLocalizer()
        {
        }

        public ImageLocalizer(Queue<String> urls, int total)
        {
            this.totalDesired = total;
            this.totalDownloaded = 0;
            this.urls = urls;
            this.activeDownloads = 0;

            for (int i = 0; i < MaxDownloadsAllowed; i++)
            {
                this.allWaitHandles[i] = new AutoResetEvent(false);
                this.freeWaitHandles.Enqueue(this.allWaitHandles[i]);
            }
        }


        private AutoResetEvent getFreeWaitHandle()
        {
            lock (this.freeWaitHandles)
            {
                try
                {
                    return this.freeWaitHandles.Dequeue();
                }
                catch (InvalidOperationException)
                {
                    // This exception is thrown if the queue is empty
                    return null;
                }
            }
        }


        private void ExecuteDownload(object state)
        {
            String url = (String)state;
            AutoResetEvent waitHandle = null;
            int getHandleCount = 0;

            try
            {
                // First, attempt to obtain a free wait handle. 
                while ((waitHandle == null) && (getHandleCount < 5))
                {
                    waitHandle = getFreeWaitHandle();
                    getHandleCount++;
                    Thread.Sleep(100);
                }

                // Simply return if we were unable to obtain one
                if (waitHandle == null)
                    return;

                using (WebDownload dl = new WebDownload(url, new DownloadProgressHandler(dl_ProgressCallback)))
                {
                    if (dl.Download() == WebDownload.WebDownloadResult.Success)
                    {
                        CreateDownloadedImage(dl.DownloadedData, url);
                    }
                }

                //StatusManager.Instance.RemoveTask(url.GetHashCode());

                lock (this.status)
                {
                    this.status.Remove(url);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                activeDownloads--;

                if (waitHandle != null)
                {
                    lock (this.freeWaitHandles)
                    {
                        this.freeWaitHandles.Enqueue(waitHandle);
                        waitHandle.Set();
                    }
                }
            }
        }

        public List<DownloadedImage> Download()
        {
            while ((totalDownloaded < totalDesired) && (this.urls.Count > 0))
            {
                //System.Console.WriteLine("ImageLocalizer url count: " + this.urls.Count);

                ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteDownload), this.urls.Dequeue());

                activeDownloads++;

                if (activeDownloads >= MaxDownloadsAllowed)
                {
                    //System.Console.WriteLine("Waiting on all handles...");
                    WaitHandle.WaitAny(allWaitHandles);
                }
            }

            // Wait for remaining threads to complete their downloads
            while (activeDownloads > 0)
            {
                //System.Console.WriteLine("Waiting for remaining downloads to complete...");
                WaitHandle.WaitAny(allWaitHandles);
            }

            return this.downloadedImages;
        }


        void dl_FailedCallback()
        {
        }

        void dl_ProgressCallback(String uri, int bytesRead, int totalBytes)
        {
            lock (this.status)
            {
                if (this.status.ContainsKey(uri))
                {
                    this.status[uri].bytesRead = bytesRead;
                }
                else
                {
                    this.status[uri] = new LocalizationStatus();
                    this.status[uri].totalBytes = totalBytes;
                    this.status[uri].bytesRead = bytesRead;
                }
            }


            // Update percent completion status      
            if ((totalBytes > 0) && (bytesRead > 0))
            {
                StatusManager.Instance.SetTaskPercentCompletion(uri.GetHashCode(), (int)(((float)bytesRead / (float)totalBytes) * 100.0));
            }
        }

        void dl_CompleteCallback(byte[] dataDownloaded)
        {
            MemoryStream stream = new MemoryStream(dataDownloaded);
            Bitmap img = (Bitmap)Image.FromStream(stream);

            if (this.ImageLocalizedEvent != null)
                this.ImageLocalizedEvent(img);
        }

        private void CreateDownloadedImage(byte[] dataDownloaded, String url)
        {
            lock (this.downloadedImages)
            {
                // We need this check because we may be actively downloading more
                // images then was requested.  This is because not all downloads
                // will really complete.
                if (this.downloadedImages.Count < this.totalDesired)
                {
                    MemoryStream stream = new MemoryStream(dataDownloaded);
                    Bitmap img = (Bitmap)Image.FromStream(stream);
                    DownloadedImage dlImage = new DownloadedImage(img, url);

                    this.downloadedImages.Add(dlImage);
                    this.totalDownloaded++;
                }
            }
        }

        public static Image Localize(String url)
        {
            Image img = null;
            try
            {
                url = url.Replace("%2520", "%20");
                Stream ImageStream = new WebClient().OpenRead(url);
                img = Image.FromStream(ImageStream);
            }
            catch
            {
                //Console.WriteLine("Error downloading image...");
            }
            finally { }

            return img;
        }
    }
}
