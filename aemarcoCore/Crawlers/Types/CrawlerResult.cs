using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class CrawlerResult : ICrawlerResult
    {

        private string _resultName;
        private List<IWallEntry> _newEntries;
        private List<IWallEntry> _knownEntries;

        internal CrawlerResult(string resultName)
        {
            _resultName = resultName;
            _newEntries = new List<IWallEntry>();
            _knownEntries = new List<IWallEntry>();
        }



        internal void AddNewEntry(IWallEntry entry)
        {
            _newEntries.Add(entry);
        }
        internal void AddKnownEntry(IWallEntry entry)
        {
            _knownEntries.Add(entry);
        }

        public string ResultName { get { return _resultName; } }
        public List<IWallEntry> NewEntries { get { return _newEntries; } }
        public List<IWallEntry> KnownEntries { get { return _knownEntries; } }
        public string GetJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
