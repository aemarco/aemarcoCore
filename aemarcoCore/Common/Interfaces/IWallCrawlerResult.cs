using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IWallCrawlerResult
    {
        string ResultName { get; }
        bool HasBeenAborted { get; set; }
        Exception Exception { get; set; }
        int NumberOfCrawlersInvolved { get; set; }

        List<IWallEntry> NewEntries { get; }
        List<IWallEntry> KnownEntries { get; }

        [JsonIgnore]
        string JSON { get; }

    }
}
