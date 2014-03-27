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

namespace com.drollic.net.search.image
{
    public sealed class CrossServiceImageFinder : ImageUrlProvider
    {
        private int totalDesiredImages = 5;
        //private int totalDownloaded = 0;

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

        public override String Name
        {
            get
            {
                return "All Image Services";
            }
        }

        protected override List<String> DiscoverImageUrls(String searchText)
        {
            ImageUrlProvider flickr = new Flickr();
            flickr.TotalDesiredImages = this.totalDesiredImages;

            List<String> flickrUrls = flickr.Search(searchText);
            List<String> finalUrls = new List<String>();

            int maxUrl = flickrUrls.Count;
            for (int i=0; i < maxUrl; i++)
            {
                if (finalUrls.Count >= this.totalDesiredImages)
                    break;

                if (i < flickrUrls.Count)
                    finalUrls.Add(flickrUrls[i]);

                if (finalUrls.Count >= this.totalDesiredImages)
                    break;
            }

            return finalUrls;
        }
    }
}
