using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace com.drollic.net
{
    public class UploadProgressEventArgs : EventArgs
    {
        public readonly int BytesSent;
        public readonly int FileSize;
        public readonly string FilePath;
        public readonly int UploadSpeed;

        public UploadProgressEventArgs(string file, int sent, int size, DateTime timestamp)
        {
            this.BytesSent = sent;
            this.FileSize = size;
            this.FilePath = file;

            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan then = new TimeSpan(timestamp.Ticks);
            int diff = (int)now.Subtract(then).TotalSeconds;
            if (diff == 0)
            {
                this.UploadSpeed = 0;
            }
            else
            {
                this.UploadSpeed = sent / diff;
            }
        }
    }

    public class HttpFileUpload
    {
        public event EventHandler<UploadProgressEventArgs> UploadProgress;
        public int buffer_size = 4096;

        public HttpFileUpload()
        {
            // This is a global property and probably shouldn't be used.
            System.Net.ServicePointManager.Expect100Continue = false;
        }

        public HttpWebResponse UploadFile(string uri, string fileField, string filePath, ArrayList formFields, CookieContainer cookies)
        {
            string newLine = "\r\n";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            if (cookies != null)
            {
                req.CookieContainer = cookies;
            }

            string boundary = Guid.NewGuid().ToString().Replace("-", "");
            req.ContentType = "multipart/form-data; boundary=" + boundary;
            req.Method = "POST";
            req.Timeout = Timeout.Infinite;
            req.KeepAlive = false;
            string postData = "";

            // Handle form fields
            // StringDictionary doesn't support duplicated keys, therefore we use a list of DictionaryEntry's
            if (formFields != null)
            {
                foreach (DictionaryEntry de in formFields)
                {
                    postData += String.Format("--" + boundary + newLine);
                    postData += String.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}{2}{1}", de.Key, newLine, de.Value);
                }
            }
            // End handle form fields

            // Begin file upload
            using (FileStream fs = File.OpenRead(filePath))
            {
                string streamEnd = String.Format("{1}--{0}--{1}", boundary, newLine);
                postData += String.Format("--" + boundary + newLine);
                postData += String.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", fileField, Path.GetFileName(filePath), newLine);
                postData += String.Format("Content-Type: application/octet-stream" + newLine + newLine);

                byte[] postdata = str2bytes(postData);
                byte[] streamend = str2bytes(streamEnd);
                req.ContentLength = postdata.Length + fs.Length + streamend.Length;

                using (Stream rs = req.GetRequestStream())
                {
                    rs.Write(postdata, 0, postdata.Length);

                    byte[] buf = new byte[buffer_size];
                    int bytesread = 0;
                    int filesize = (int)fs.Length;
                    int uploaded = 0;
                    DateTime timestamp = DateTime.Now;
                    while ((bytesread = fs.Read(buf, 0, buffer_size)) > 0)
                    {
                        rs.Write(buf, 0, bytesread);
                        uploaded += bytesread;
                        OnUploadProgress(new UploadProgressEventArgs(filePath, uploaded, filesize, timestamp));
                    }
                    rs.Write(streamend, 0, streamend.Length);
                    rs.Flush();
                }
            }
            // End file upload

            return (HttpWebResponse)req.GetResponse();
        }

        private void OnUploadProgress(UploadProgressEventArgs e)
        {
            if (UploadProgress != null)
            {
                UploadProgress(this, e);
            }
        }

        private byte[] str2bytes(string Input)
        {
            return Encoding.UTF8.GetBytes(Input);
        }
    }
}
