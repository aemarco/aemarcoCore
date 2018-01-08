using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IWallCrawlerResult
    {
        string ResultName { get; }
        List<IWallEntry> NewEntries { get; }
        List<IWallEntry> KnownEntries { get; }
        [JsonIgnore]
        string JSON { get; }

    }
}
