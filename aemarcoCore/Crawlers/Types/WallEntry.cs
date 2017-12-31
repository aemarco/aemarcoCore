using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallEntry : IWallEntry
    {
        public string Url { get; internal set; }
        public string FileName { get; internal set; }
        public string Extension { get; internal set; }
        public string Kategorie { get; internal set; }
        public string SeitenKategorie { get; internal set; }
        public List<string> Tags { get; internal set; }


        public string getJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
