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


    public class IPersonCrawlerResultEventArgs : EventArgs
    {
        public IPersonCrawlerResult Result { get; set; }

    }

    public class IPersonEntryEventArgs : EventArgs
    {
        public IPersonEntry Entry { get; set; }

    }


}
