using aemarcoCore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Exceptions = new List<Exception>();

            NewEntries = new List<IWallEntry>();
            KnownEntries = new List<IWallEntry>();
            AlbumEntries = new List<IAlbumEntry>();
        }


        public string ResultName { get; set; }
        public List<string> SitesFilter { get; set; }
        public List<string> CategoryFilter { get; set; }

        public List<string> CrawlersInvolved { get; set; }
        public int NumberOfCrawlersInvolved { get; set; }

        public bool HasBeenAborted { get; set; }

        [JsonIgnore]
        public List<Exception> Exceptions { get; set; }

        public int NumberOfNewEntries { get { return NewEntries.Count + AlbumEntries.Sum(x => x.Entries.Count); } }
        public int NumberOfKnownEntries { get { return KnownEntries.Count; } }

        public List<IWallEntry> NewEntries { get; set; }
        public List<IAlbumEntry> AlbumEntries { get; set; }
        public List<IWallEntry> KnownEntries { get; set; }

        [JsonIgnore]
        public string JSON
        { get { return JsonConvert.SerializeObject(this, Formatting.Indented); } }




        internal void AddNewEntry(IWallEntry entry)
        {
            NewEntries.Add(entry);
        }
        internal void AddToAlbums(IWallEntry entry, bool isNew)
        {
            if (AlbumEntries.FirstOrDefault(x => x.Name == entry.AlbumName) is AlbumEntry album)
            {
                album.Entries.Add(entry);
            }
            else
            {
                album = new AlbumEntry(entry);
                AlbumEntries.Add(album);
            }

            album.HasNewEntries |= isNew;
        }
        public void CleanupAlbums()
        {
            foreach (AlbumEntry album in AlbumEntries)
            {
                if (!album.HasNewEntries)
                {
                    KnownEntries.AddRange(album.Entries);
                    album.Entries.Clear();
                }
            }
            AlbumEntries.RemoveAll(x => x.Entries.Count == 0);
        }
        internal void AddKnownEntry(IWallEntry entry)
        {
            KnownEntries.Add(entry);
        }
    }
}
