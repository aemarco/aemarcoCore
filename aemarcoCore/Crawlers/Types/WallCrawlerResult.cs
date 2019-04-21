﻿using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    internal class WallCrawlerResult : IWallCrawlerResult
    {

        internal WallCrawlerResult()
        {
            ResultName = string.Empty;
            SitesFilter = new List<string>();
            CategoryFilter = new List<string>();
            CrawlersInvolved = new List<string>();
            NumberOfCrawlersInvolved = 0;
            HasBeenAborted = false;
            Exception = null;

            NewEntries = new List<IWallEntry>();
            KnownEntries = new List<IWallEntry>();
        }


        public string ResultName { get; set; }
        public List<string> SitesFilter { get; set; }
        public List<string> CategoryFilter { get; set; }

        public List<string> CrawlersInvolved { get; set; }
        public int NumberOfCrawlersInvolved { get; set; }

        public bool HasBeenAborted { get; set; }
        public Exception Exception { get; set; }

        public int NumberOfNewEntries { get { return NewEntries.Count; } }
        public int NumberOfKnownEntries { get { return KnownEntries.Count; } }

        public List<IWallEntry> NewEntries { get; set; }
        public List<IWallEntry> KnownEntries { get; set; }



        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }



        internal void AddNewEntry(IWallEntry entry)
        {
            NewEntries.Add(entry);
        }
        internal void AddKnownEntry(IWallEntry entry)
        {
            KnownEntries.Add(entry);
        }


    }
}
