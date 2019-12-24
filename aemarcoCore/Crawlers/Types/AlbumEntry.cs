using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace aemarcoCore.Crawlers.Types
{
    public class AlbumEntry : IAlbumEntry
    {
        public AlbumEntry(IWallEntry entry)
        {
            Name = entry.AlbumName;
            Entries = new List<IWallEntry> { entry };
            HasNewEntries = false;
        }



        public string Name { get; }
        public List<IWallEntry> Entries { get; }


        internal bool HasNewEntries { get; set; }


    }
}
