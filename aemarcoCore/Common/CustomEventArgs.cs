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



}
