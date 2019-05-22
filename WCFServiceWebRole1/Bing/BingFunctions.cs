using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Text;

namespace WCFServiceWebRole
{
    public class BingFunctions
    {
        private const string rootUrl = "https://api.cognitive.microsoft.com/bing/v5.0/search";
        //private const string AccountKey = "9c565490552b4f818afc98dd4db83ab4 ";
        private const string AccountKey = "80bcefc4b9cd42d7a8fe72f64e64df73";
        //https://msdn.microsoft.com/en-us/library/dd251064.aspx
        string market = "ar-XA";
        public List<Bing.WebResult> WebSearch(string query, int resultReturn = 1)
        {
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);
            var webQuery = bingContainer.Web(query, null, null, market, null, null, null, null);
            webQuery = webQuery.AddQueryOption("$top", resultReturn);

            return webQuery.Execute().ToList();
           
        }
        public List<Bing.ImageResult> ImageSearch(string query, int resultReturn = 1)
        {
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);
            var imageQuery = bingContainer.Image(query, null, market, null, null, null, "Size:Medium");
            imageQuery = imageQuery.AddQueryOption("$top", resultReturn);
            return imageQuery.Execute().ToList();
        }
   



    }
    public class BingImageSearchResponse
    {
        public string _type { get; set; }
        public int totalEstimatedMatches { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public ImageResult[] value { get; set; }
    }

    public class ImageResult
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public object datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string accentColor { get; set; }
    }
}