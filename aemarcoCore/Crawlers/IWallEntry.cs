using aemarcoCore.Types;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers
{
    public interface IWallEntry
    {
        string Url { get; }
        string ThumbnailUrl { get; }
        string FileName { get; }
        string Extension { get; }
        string Kategorie { get; }
        IContentCategory ContentCategory { get; }
        string SiteCategory { get; }
        List<string> Tags { get; }


        string GetJSON();
    }
}
