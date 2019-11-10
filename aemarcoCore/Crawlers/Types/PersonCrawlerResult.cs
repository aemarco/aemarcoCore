using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCore.Crawlers.Types
{
    internal class PersonCrawlerResult : IPersonCrawlerResult
    {
        private string _resultName;
        private List<IPersonEntry> _entries;
        private bool _hasBeenAborted;
        private Exception _exception;

        internal PersonCrawlerResult()
        {
            _resultName = string.Empty;
            _hasBeenAborted = false;
            _exception = null;
            _entries = new List<IPersonEntry>();
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

        public List<IPersonEntry> Entries
        { get { return _entries; } }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }


        internal void AddNewEntry(IPersonEntry entry)
        {
            _entries.Add(entry);
            _entries = _entries.OrderByDescending(x => x.PersonEntryPriority).ToList();
        }





    }
}
