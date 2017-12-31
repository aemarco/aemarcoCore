using System.Collections.Generic;

namespace aemarcoCore.Crawlers
{
    public interface ICrawlerResult
    {
        List<IWallEntry> Entries { get; }
        string GetJSON();
    }
}
