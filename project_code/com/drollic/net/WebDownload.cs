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
using System.Net;
using System.IO;
using System.Threading;
using System.Security.Permissions;

using com.drollic.util;

namespace com.drollic.net
{
	public delegate void DownloadProgressHandler(String uri, int bytesRead, int totalBytes);
    public delegate void DownloadCompleteHandler(byte[] dataDownloaded);
    public delegate void DownloadFailedHandler();

	// The RequestState class passes data across async calls.
	public sealed class DownloadInfo : IDisposable
	{
		const int BufferSize = 1024;
		public byte[] BufferRead;

		public bool useFastBuffers;
		public byte[] dataBufferFast;
		public System.Collections.ArrayList dataBufferSlow;

		public int dataLength;
		public int bytesProcessed;

		public WebRequest Request;
		public Stream ResponseStream;
		public DownloadProgressHandler ProgressCallback;

        public bool connectionFailed;

		public DownloadInfo()
		{
			BufferRead = new byte[BufferSize];
			Request = null;
			dataLength = -1;
			bytesProcessed = 0;
            connectionFailed = false;
			useFastBuffers = true;
		}

        public void Dispose()
        {
            if (this.ResponseStream != null)
            {
                this.Request.Abort();
                //this.ResponseStream.Close();
                this.ResponseStream.Dispose();
            }

            this.BufferRead = null;
            this.Request = null;
            this.dataBufferFast = null;
            this.dataBufferSlow = null;
            this.ResponseStream = null;
            this.ProgressCallback = null;
        }
    }

	// ClientGetAsync issues the async request.
	public sealed class WebDownload : IDisposable
	{
        public enum WebDownloadResult
        {
            Success,
            Failure
        }

        public const int MaxDownloadSizeInBytes = 400 * 1024;

        //public event DownloadCompleteHandler CompleteCallback;
        //public event DownloadFailedHandler FailedCallback;
		public ManualResetEvent allDone = new ManualResetEvent(false);
        
        private System.Threading.Timer downloadTimer;

        private byte[] allDownloadedData;

        private DateTime downloadStartTime;

        private bool downloadAborted = false;

        // State object
        private DownloadInfo info = new DownloadInfo();

        private bool disposed = false;

		private const int BUFFER_SIZE = 1024;

        private DownloadProgressHandler progressCB = null;

        private Object downloadAbortLock = new Object();


        public WebDownload(String url, DownloadProgressHandler callback)
        {            
            this.url = url;
            this.progressCB = callback;
            info.ProgressCallback += this.progressCB;    
        
            StatusManager.Instance.SetTaskPercentCompletion(url.GetHashCode(), 0);
        }


        public byte[] DownloadedData
        {
            get
            {
                return this.allDownloadedData;
            }
        }


        private String url;
        public String Url
        {
            get
            {
                return this.url;
            }
        }

        public void CheckDownloadStatus(Object stateInfo)
        {
            TimeSpan duration = DateTime.Now - this.downloadStartTime;

            if (duration.TotalSeconds > 30)
            {
                Abort();
            }
        }


        private void Abort()
        {
            //System.Console.WriteLine("Aborting donwload on " + this.url);

            lock (this.downloadAbortLock)
            {
                downloadAborted = true;

                allDone.Set();
            }
        }

        public WebDownloadResult Download()
		{
            try
            {
                //System.Console.WriteLine("Attempting to download " + this.url);

                // Ensure flag set correctly.			
                allDone.Reset();

                // Get the URI from the command line.
                Uri httpSite = new Uri(url);

                // Create the request object.
                WebRequest req = WebRequest.Create(httpSite);

                // Put the request into the state object so it can be passed around.
                info.Request = req;

                TimerCallback timerDelegate = new TimerCallback(CheckDownloadStatus);
                downloadStartTime = DateTime.Now;
                this.downloadTimer = new Timer(timerDelegate, null, 1000, 3000);

                // Issue the async request.
                IAsyncResult r = (IAsyncResult)req.BeginGetResponse(new AsyncCallback(ResponseCallback), info);

                // Wait until the ManualResetEvent is set so that the application
                // does not exit until after the callback is called.
                allDone.WaitOne();

                // Check for failure
                if (downloadAborted || info.connectionFailed)
                {
                    //System.Console.WriteLine("Download aborted on " + this.url);
                    return WebDownloadResult.Failure;
                }

                // Pass back the downloaded information.
                if (info.useFastBuffers)
                    allDownloadedData = info.dataBufferFast;
                else
                {
                    byte[] data = new byte[info.dataBufferSlow.Count];
                    for (int b = 0; b < info.dataBufferSlow.Count; b++)
                        data[b] = (byte)info.dataBufferSlow[b];
                    allDownloadedData = data;
                }

                //CompleteCallback(allDownloadedData);

                return WebDownloadResult.Success;
            }
            catch (Exception)
            {
                return WebDownloadResult.Failure;
            }
		}

		private void ResponseCallback(IAsyncResult ar)
		{
            if (this.disposed || this.downloadAborted)
                return;

            // Get the DownloadInfo object from the async result were
            // we're storing all of the temporary data and the download
            // buffer.
            DownloadInfo info = (DownloadInfo)ar.AsyncState;

            try
            {
                // Get the WebRequest from RequestState.
                WebRequest req = info.Request;

                // Call EndGetResponse, which produces the WebResponse object
                // that came from the request issued above.
                WebResponse resp = req.EndGetResponse(ar);

                // Find the data size from the headers.
                string strContentLength = resp.Headers["Content-Length"];
                if (strContentLength != null)
                {
                    info.dataLength = Convert.ToInt32(strContentLength);
                    //System.Console.WriteLine("Size: " + info.dataLength + " for " + this.url);

                    if (info.dataLength > MaxDownloadSizeInBytes)
                    {
                        //System.Console.WriteLine("Download is too big, aborting");
                        Abort();
                        return;
                    }

                    info.dataBufferFast = new byte[info.dataLength];
                }
                else
                {
                    info.useFastBuffers = false;
                    info.dataBufferSlow = new System.Collections.ArrayList(BUFFER_SIZE);
                }

                //  Start reading data from the response stream.
                Stream ResponseStream = resp.GetResponseStream();

                // Store the response stream in RequestState to read
                // the stream asynchronously.
                info.ResponseStream = ResponseStream;

                //  Pass do.BufferRead to BeginRead.
                IAsyncResult iarRead = ResponseStream.BeginRead(info.BufferRead,
                    0,
                    BUFFER_SIZE,
                    new AsyncCallback(ReadCallBack),
                    info);                
            }
            catch (WebException)
            {
                info.connectionFailed = true;
            }
            catch (Exception)
            {
                info.connectionFailed = true;
            }
            finally
            {
                if (info.connectionFailed)
                    Abort();
            }
		}

		private void ReadCallBack(IAsyncResult asyncResult)
		{
            lock (this.downloadAbortLock)
            {
                if (this.disposed || this.downloadAborted)
                    return;

                // Get the DownloadInfo object from AsyncResult.
                DownloadInfo info = (DownloadInfo)asyncResult.AsyncState;
                if (info == null)
                    Abort();

                // Retrieve the ResponseStream that was set in RespCallback.
                Stream responseStream = info.ResponseStream;
                if (responseStream == null)
                    Abort();

                // Read info.BufferRead to verify that it contains data.
                int bytesRead = responseStream.EndRead(asyncResult);
                if (bytesRead > 0)
                {
                    if (info.useFastBuffers)
                    {
                        System.Array.Copy(info.BufferRead, 0,
                            info.dataBufferFast, info.bytesProcessed,
                            bytesRead);
                    }
                    else
                    {
                        for (int b = 0; b < bytesRead; b++)
                            info.dataBufferSlow.Add(info.BufferRead[b]);
                    }
                    info.bytesProcessed += bytesRead;

                    // Perhaps we didn't originally get the content size, so we're 
                    // just reading data with no end in sight.  If that's the casre,
                    // abort the download if we cross the max download size.
                    if (info.bytesProcessed > MaxDownloadSizeInBytes)
                    {
                        //System.Console.WriteLine("Download has become too big, aborting");
                        Abort();
                        return;
                    }

                    // If a registered progress-callback, inform it of our
                    // download progress so far.
                    if ((info.ProgressCallback != null) && (!this.downloadAborted))
                        info.ProgressCallback(info.Request.RequestUri.AbsoluteUri, info.bytesProcessed, info.dataLength);

                    // Continue reading data until responseStream.EndRead returns –1.
                    IAsyncResult ar = responseStream.BeginRead(
                        info.BufferRead, 0, BUFFER_SIZE,
                        new AsyncCallback(ReadCallBack), info);
                }
                else
                {
                    // We're done, signal completion
                    allDone.Set();
                }                
            }
		}

        #region IDisposable Members

        public void Dispose()
        {
            this.disposed = true;
            this.info.ProgressCallback -= this.progressCB;   
            this.downloadTimer.Dispose();
            this.info.Dispose();

            this.progressCB = null;
            this.allDownloadedData = null;
            this.downloadTimer = null;
            this.info = null;
        }

        #endregion
    }
}
