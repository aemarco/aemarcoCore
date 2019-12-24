using Newtonsoft.Json;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IWallEntry
    {
        string Url { get; }
        string ThumbnailUrl { get; }
        string FileName { get; }
        string Extension { get; }
        IContentCategory ContentCategory { get; }
        string SiteCategory { get; }
        List<string> Tags { get; }
        string Filepath { get; }
        string AlbumName { get; }


        [JsonIgnore]
        string JSON { get; }
    }
}
