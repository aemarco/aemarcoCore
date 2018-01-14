using aemarcoCore.Common;
using aemarcoCore.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntry : IWallEntry
    {
        private static List<string> _crawlerExtensions;

        private string _url;
        private string _thumbnailUrl;
        private string _fileName;
        private string _extension;
        private IContentCategory _contentCategory;
        private string _siteCategory;
        private List<string> _tags;


        internal WallEntry(
            string url, string thumbnailUrl, string fileName,
            IContentCategory contentCategory, string siteCategory, List<string> tags)
        {
            _url = url;
            _thumbnailUrl = thumbnailUrl;
            _fileName = fileName;
            _contentCategory = contentCategory;
            _siteCategory = siteCategory;
            _tags = tags;

            if (!String.IsNullOrEmpty(url))
            {
                _extension = Path.GetExtension(url).ToLower();
            }

        }


        public string Url
        { get { return _url; } }
        public string ThumbnailUrl
        { get { return _thumbnailUrl; } }
        public string FileName
        { get { return _fileName; } }
        public string Extension
        { get { return _extension; } }
        public IContentCategory ContentCategory
        { get { return _contentCategory; } }
        public string SiteCategory
        { get { return _siteCategory; } }
        public List<string> Tags
        { get { return _tags; } }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }
        [JsonIgnore]
        internal bool IsValid
        {
            get
            {
                //Once initiallized with valid extentions
                if (_crawlerExtensions == null)
                {
                    _crawlerExtensions = new List<string>();
                    foreach (var ext in Settings.Default.CrawlerFileExtensions.Split(','))
                    {
                        _crawlerExtensions.Add(ext.Replace(".", "").ToLower().Insert(0, "."));
                    }
                }


                if (
                    String.IsNullOrEmpty(_url) || //Entry muss Url haben
                    String.IsNullOrEmpty(_thumbnailUrl) || //Entry muss ThumbnailUrl haben
                    String.IsNullOrEmpty(_fileName) || //Entry muss FileName haben
                    String.IsNullOrEmpty(_extension) || //Entry muss Extension haben
                    String.IsNullOrEmpty(_siteCategory) || //Entry muss SiteCategory haben
                    _contentCategory == null || //Entry muss ContentCategory haben
                    _tags == null || //Entry muss Tags haben                    
                    !_crawlerExtensions.Contains(_extension) //Extension muss erlaubt sein
                    )
                {
                    return false;
                }
                return true;
            }
        }
    }
}
