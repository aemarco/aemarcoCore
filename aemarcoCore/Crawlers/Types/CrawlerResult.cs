using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class CrawlerResult : ICrawlerResult
    {
        private List<IWallEntry> _entries;
        public List<IWallEntry> Entries { get { return _entries; } }

        public string GetJSON()
        {
            return JsonConvert.SerializeObject(_entries, Formatting.Indented);
        }


        internal CrawlerResult()
        {
            _entries = new List<IWallEntry>();
        }


        internal void AddEntry(IWallEntry entry)
        {
            _entries.Add(entry);
        }
    }
}
