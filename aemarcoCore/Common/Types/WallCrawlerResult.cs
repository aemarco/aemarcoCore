using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    internal class WallCrawlerResult : IWallCrawlerResult
    {
        private string _resultName;
        private List<IWallEntry> _newEntries;
        private List<IWallEntry> _knownEntries;

        internal WallCrawlerResult(string resultName)
        {
            _resultName = resultName;
            _newEntries = new List<IWallEntry>();
            _knownEntries = new List<IWallEntry>();
        }


        public string ResultName
        { get { return _resultName; } }
        public List<IWallEntry> NewEntries
        { get { return _newEntries; } }
        public List<IWallEntry> KnownEntries
        { get { return _knownEntries; } }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



        internal void AddNewEntry(IWallEntry entry)
        {
            _newEntries.Add(entry);
        }
        internal void AddKnownEntry(IWallEntry entry)
        {
            _knownEntries.Add(entry);
        }

    }
}
