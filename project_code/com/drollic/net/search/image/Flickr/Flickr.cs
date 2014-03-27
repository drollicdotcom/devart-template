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
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Security.Permissions;

using com.drollic.util;

namespace com.drollic.net.search.image
{
    public sealed class Flickr : ImageUrlProvider
    {
        private const string _baseUrl = "http://api.flickr.com/services/rest/";
        private int totalDesiredImages;

        //private static int imagesPerPage = 10;
        private static Random rand = new Random();

        private const String _apiKey = REPLACE_WITH_YOUR_API_KEY;
        private int _timeout = 30000;
        private const string UserAgent = "Mozilla/4.0 Drollic Dreamer(compatible; MSIE 6.0; Windows NT 5.1)";
        private string _lastRequest;
        private string _lastResponse;
        private WebProxy _proxy;

        // Static serializers
        private static XmlSerializer _responseSerializer;

        public Flickr()
        {
            _responseSerializer = new XmlSerializer(typeof(FlickrResponse));
        }

        public override String Name
        {
            get
            {
                return "FlickR";
            }
        }

        public override int TotalDesiredImages
        {
            get
            {
                return this.totalDesiredImages;
            }
            set
            {
                this.totalDesiredImages = value;
            }
        }

        /// <summary>
        /// Internal timeout for all web requests in milliseconds. Defaults to 30 seconds.
        /// </summary>
        public int HttpTimeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// You can set the <see cref="WebProxy"/> or alter its properties.
        /// It defaults to your internet explorer proxy settings.
        /// </summary>
        public WebProxy Proxy { get { return _proxy; } set { _proxy = value; } }

        protected override List<String> DiscoverImageUrls(String search)
        {
            List<String> urls = new List<String>();
            try
            {
                if (search == "")
                {
                    StatusManager.Instance.StatusMessage("Please enter some text and try again.");
                    return urls;
                }

                FlickrResponse response = PerformSearch(search);

                while (urls.Count < this.totalDesiredImages)
                {
                    // First, look for obvious error conditions, or exhausted results
                    if ((response == null) || (response.Photos == null))
                    {
                        // The Urls collection could be populated, so only
                        // present the error if it's not.
                        if (urls.Count == 0)
                        {
                            StatusManager.Instance.StatusMessage("No subjects found.");
                        }
                        return urls;
                    }



                    if (response.Photos.PhotoCollection.Count > 10)
                    {
                        urls.Add(response.Photos.PhotoCollection[rand.Next(response.Photos.PhotoCollection.Count - 1)].MediumUrl);
                    }

                    if (response.Photos.TotalPages > 1)
                    {
                        if (rand.Next(0, 3) == 1)
                        {
                            long nextPage = (response.Photos.PageNumber + 1) % response.Photos.TotalPages;
                            response = PerformSearch(search, (int)nextPage);
                        }
                    }
                    else
                    {
                        // We didn't get that many results, so try reducing 
                        // the search criteria
                        search = pruneSearch(search);

                        // Search again
                        response = PerformSearch(search);
                    }
                }
            }
            catch (Exception)
            {            
            }
            
            return urls;
        }


        private FlickrResponse PerformSearch(String searchText)
        {
            return PerformSearch(searchText, 1);
        }

        private FlickrResponse PerformSearch(String searchText, int pageNumber)
        {
            //System.Console.WriteLine("Requesting page number " + pageNumber);

            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("method", "flickr.photos.search");
            parameters.Add("privacy_filter", "1");  // Public photos only
            parameters.Add("text", searchText);  // Free text search, includes tags

            //parameters.Add("tags", "TAGS");  // Must be comma delimited
            //parameters.Add("tag_mode", "any"); // search for any/all tags

            //parameters.Add("per_page", imagesPerPage.ToString());  // FlickR will default to 100
            parameters.Add("page", pageNumber.ToString());        // FlickR will default to page 1

            // Calulate URL 
            StringBuilder UrlStringBuilder = new StringBuilder(_baseUrl, 2 * 1024);

            UrlStringBuilder.Append("?");

            parameters["api_key"] = _apiKey;

            string[] keys = parameters.AllKeys;
            Array.Sort(keys);

            foreach (string key in keys)
            {
                if (UrlStringBuilder.Length > 0) UrlStringBuilder.Append("&");
                UrlStringBuilder.Append(key);
                UrlStringBuilder.Append("=");
                UrlStringBuilder.Append(Utils.UrlEncode(parameters[key]));
            }

            string url = UrlStringBuilder.ToString();
            _lastRequest = url;
            _lastResponse = string.Empty;

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            // Initialise the web request
            req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentLength = 0;
            req.UserAgent = UserAgent;
            if (Proxy != null) req.Proxy = Proxy;
            req.Timeout = HttpTimeout;
            req.KeepAlive = false;

            try
            {
                // Get response from the internet
                res = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res2 = (HttpWebResponse)ex.Response;
                    if (res2 != null)
                    {
                        throw new Exception(res2.StatusDescription);
                    }
                }
                throw new Exception(ex.Message);
            }

            string responseString = string.Empty;

            using (System.IO.StreamReader sr = new System.IO.StreamReader(res.GetResponseStream()))
            {
                responseString = sr.ReadToEnd();
            }
            
            return Deserialize(responseString);
        }

        private static FlickrResponse Deserialize(string responseString)
        {
            XmlSerializer serializer = _responseSerializer;
            try
            {
                // Deserialise the web response into the Flickr response object
                System.IO.StringReader responseReader = new System.IO.StringReader(responseString);
                FlickrResponse response = (FlickrResponse)serializer.Deserialize(responseReader);
                responseReader.Close();

                return response;
            }
            catch (InvalidOperationException ex)
            {
                // Serialization error occurred!
                throw new Exception("FlickR: Invalid response received (" + ex.Message + ")");
            }
        }
    }
}
