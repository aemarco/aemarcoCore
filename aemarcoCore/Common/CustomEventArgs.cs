using System;

namespace aemarcoCore.Common
{
    public class WallCrawlerResultEventArgs : EventArgs
    {
        public IWallCrawlerResult Result { get; set; }
    }

    public class WallEntryEventArgs : EventArgs
    {
        public IWallEntry Entry { get; set; }
    }
    public class AlbumEntryEventArgs : EventArgs
    {
        public IAlbumEntry Entry { get; set; }
    }

    public class PersonCrawlerResultEventArgs : EventArgs
    {
        public IPersonCrawlerResult Result { get; set; }

    }

    public class PersonEntryEventArgs : EventArgs
    {
        public IPersonEntry Entry { get; set; }

    }


}
