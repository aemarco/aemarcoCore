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

        [JsonIgnore]
        List<Exception> Exceptions { get; }

        int NumberOfNewEntries { get; }
        int NumberOfKnownEntries { get; }


        List<IWallEntry> NewEntries { get; }
        List<IWallEntry> KnownEntries { get; }


        int NumberOfNewAlbums { get; }
        int NumberOfKnownAlbums { get; }

        List<IAlbumEntry> NewAlbums { get; }
        List<IAlbumEntry> KnownAlbums { get; }

        [JsonIgnore]
        string JSON { get; }

    }


    public interface IAlbumEntry
    {
        string Name { get; }
        List<IWallEntry> Entries { get; }

    }

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
