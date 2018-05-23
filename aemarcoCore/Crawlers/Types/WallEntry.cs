﻿using aemarcoCore.Common;
using aemarcoCore.Properties;
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
            IContentCategory contentCategory, string siteCategory, List<string> tags)
        {
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            FileName = fileName;
            Extension = extension;
            ContentCategory = contentCategory;
            SiteCategory = siteCategory;
            Tags = tags;
        }


        public string Url { get; }
        public string ThumbnailUrl { get; }
        public string FileName { get; }
        public string Extension { get; }
        public IContentCategory ContentCategory { get; }
        public string SiteCategory { get; }
        public List<string> Tags { get; }

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
                    String.IsNullOrEmpty(Url) || //Entry muss Url haben
                    String.IsNullOrEmpty(ThumbnailUrl) || //Entry muss ThumbnailUrl haben
                    String.IsNullOrEmpty(FileName) || //Entry muss FileName haben
                    String.IsNullOrEmpty(Extension) || //Entry muss Extension haben
                    String.IsNullOrEmpty(SiteCategory) || //Entry muss SiteCategory haben
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
