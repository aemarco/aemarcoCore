using System.Collections.Generic;

namespace aemarcoCore.Crawlers
{
    public interface ICrawlerResult
    {
        string ResultName { get; }
        List<IWallEntry> NewEntries { get; }
        List<IWallEntry> KnownEntries { get; }
        string GetJSON();
    }
}
