using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallCrawlerResult : IWallCrawlerResult
    {
        private string _resultName;
        private List<IWallEntry> _newEntries;
        private List<IWallEntry> _knownEntries;
        private bool _hasBeenAborted;
        private Exception _exception;
        private int _numberOfCrawlersInvolved;

        internal WallCrawlerResult()
        {
            _resultName = string.Empty;
            _hasBeenAborted = false;
            _exception = null;
            _numberOfCrawlersInvolved = 0;
            _newEntries = new List<IWallEntry>();
            _knownEntries = new List<IWallEntry>();
        }


        public string ResultName
        {
            get { return _resultName; }
            set { _resultName = value; }
        }
        public bool HasBeenAborted
        {
            get => _hasBeenAborted;
            set => _hasBeenAborted = value;
        }
        public Exception Exception
        {
            get => _exception;
            set => _exception = value;
        }

        public int NumberOfCrawlersInvolved
        {
            get => _numberOfCrawlersInvolved;
            set => _numberOfCrawlersInvolved = value;
        }







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
