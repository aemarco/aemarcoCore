using aemarcoCore.Tools;
using aemarcoCore.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntry : IWallEntry
    {
        public string Url { get; internal set; }
        public string ThumbnailUrl { get; internal set; }
        public string FileName { get; internal set; }
        public string Extension { get; internal set; }
        public IContentCategory ContentCategory { get; internal set; }
        public string SiteCategory { get; internal set; }
        public List<string> Tags { get; internal set; }


        public string GetJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }


        /// <summary>
        /// Validation
        /// </summary>
        /// <returns></returns>
        internal bool IsValid()
        {
            //TODO: Some more validation

            if (
                String.IsNullOrEmpty(Url) || //Entry muss Url haben
                !FileExtension.IsCrawlerExtension(Extension) //Extension muss erlaubt sein
                )
            {
                return false;
            }
            return true;
        }




    }
}
