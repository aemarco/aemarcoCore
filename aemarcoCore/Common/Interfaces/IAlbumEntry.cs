using System;
using System.Collections.Generic;
using System.Text;

namespace aemarcoCore.Common
{
    public interface IAlbumEntry
    {
        string Name { get; }
        List<IWallEntry> Entries { get; }

    }
}
