using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IWallCrawlerResult
    {
        string ResultName { get; }
        List<string> SitesFilter { get; }
        List<string> CategoryFilter { get; }

        List<string> CrawlersInvolved { get; }
        int NumberOfCrawlersInvolved { get; }

        bool HasBeenAborted { get; }
        Exception Exception { get; }

        int NumberOfNewEntries { get; }
        int NumberOfKnownEntries { get; }


        List<IWallEntry> NewEntries { get; }
        List<IWallEntry> KnownEntries { get; }

        [JsonIgnore]
        string JSON { get; }

    }
}
