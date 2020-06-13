using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntry : IWallEntry
    {
        private static List<string> _crawlerExtensions;

        internal WallEntry(
            string url, string thumbnailUrl, string fileName, string extension,
            IContentCategory contentCategory, string siteCategory, List<string> tags, string albumName)
        {
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            FileName = fileName;
            Extension = extension;
            ContentCategory = contentCategory;
            SiteCategory = siteCategory;
            Tags = tags;
            AlbumName = albumName;
        }


        public string Url { get; set; }
        public string ThumbnailUrl { get; }
        public string FileName { get; }
        public string Extension { get; }
        public IContentCategory ContentCategory { get; }
        public string SiteCategory { get; }
        public List<string> Tags { get; }
        public string AlbumName { get; }


        public string FileContentAsBase64String { get; set; }


        [JsonIgnore]
        public string Json
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }
        [JsonIgnore]
        internal bool IsValid
        {
            get
            {
                //Once initialized with valid extensions
                if (_crawlerExtensions == null)
                {
                    _crawlerExtensions = new List<string>();
                    foreach (var ext in "bmp,jpg,jpeg,png,gif".Split(','))
                    {
                        _crawlerExtensions.Add(ext.Replace(".", string.Empty).ToLower().Insert(0, "."));
                    }
                }


                if (
                    (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(FileContentAsBase64String)) || //Entry muss Url oder File haben
                    string.IsNullOrEmpty(ThumbnailUrl) || //Entry muss ThumbnailUrl haben
                    string.IsNullOrEmpty(FileName) || //Entry muss FileName haben
                    string.IsNullOrEmpty(Extension) || //Entry muss Extension haben
                    string.IsNullOrEmpty(SiteCategory) || //Entry muss SiteCategory haben
                    ContentCategory == null || //Entry muss ContentCategory haben
                    Tags == null || //Entry muss Tags haben                    
                    !_crawlerExtensions.Contains(Extension) //Extension muss erlaubt sein
                    )
                {
                    return false;
                }
                return true;
            }
        }
    }
}
