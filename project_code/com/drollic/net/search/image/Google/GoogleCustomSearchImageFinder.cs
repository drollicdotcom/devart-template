using Google.Apis;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Json;
using Google.Apis.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;

using com.drollic.util;

namespace com.drollic.net.search.image
{
    public sealed class GoogleCustomSearchImageFinder : ImageUrlProvider
    {
        private int totalDesiredImages = 5;


        public GoogleCustomSearchImageFinder()
		{
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

        public override String Name
        {
            get
            {
                return "Google Images";
            }
        }

        protected override System.Collections.Generic.List<string> DiscoverImageUrls(string search)
        {
            string apiKey = REPLACE_WITH_YOUR_API_KEY;
            string cx = REPLACE_WITH_YOUR_CUSTOM_SEARCH_CX;
            string query = search;

            CustomsearchService svc = new CustomsearchService(new BaseClientService.Initializer()
            {
                ApplicationName = "Drollic Dreamer",
                ApiKey = apiKey,

            });
                       
            Random rand = new Random();

            CseResource.ListRequest listRequest = svc.Cse.List(query);
            listRequest.Cx = cx;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            listRequest.ImgType = CseResource.ListRequest.ImgTypeEnum.Photo;
            listRequest.Num = 10;
            listRequest.Start = rand.Next() % 90;
            
            Search searchResult = listRequest.Execute();

            System.Collections.Generic.List<string> results = new System.Collections.Generic.List<string>();
            foreach (Result result in searchResult.Items)
            {
                //Console.WriteLine("Title: {0}", result.Title);
                //Console.WriteLine("Link: {0}", result.Link);
                results.Add(result.Link);

                if (results.Count >= this.TotalDesiredImages)
                {
                    break;
                }
            }

            return results;
        }
    }
}
