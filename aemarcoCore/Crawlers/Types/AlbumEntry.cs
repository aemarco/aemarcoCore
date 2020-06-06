using aemarcoCore.Common;
using System.Collections.Generic;

namespace aemarcoCore.Crawlers.Types
{
    public class AlbumEntry : IAlbumEntry
    {
        public AlbumEntry(string name)
        {
            Name = name;
            Entries = new List<IWallEntry>();
        }

        public string Name { get; }


        public List<IWallEntry> Entries { get; }


        internal bool HasNewEntries { get; set; }


    }
}
