using System;

namespace aemarcoCore.Common
{
    public class IWallCrawlerResultEventArgs : EventArgs
    {
        public IWallCrawlerResult Result { get; set; }
    }

    public class IWallEntryEventArgs : EventArgs
    {
        public IWallEntry Entry { get; set; }
    }



}
