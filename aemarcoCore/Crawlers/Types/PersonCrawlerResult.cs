using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aemarcoCore.Crawlers.Types
{
    internal class PersonCrawlerResult : IPersonCrawlerResult
    {
        internal PersonCrawlerResult()
        {
            ResultName = string.Empty;
            HasBeenAborted = false;
            Exception = null;
            Entries = new List<IPersonEntry>();
        }

        public string ResultName { get; set; }

        public bool HasBeenAborted { get; set; }

        public Exception Exception { get; set; }

        public List<IPersonEntry> Entries { get; private set; }

        [JsonIgnore]
        public string Json => JsonConvert.SerializeObject(this, Formatting.Indented);


        internal void AddNewEntry(IPersonEntry entry)
        {
            Entries.Add(entry);
            Entries = Entries.OrderByDescending(x => x.PersonEntryPriority).ToList();
        }





    }
}
