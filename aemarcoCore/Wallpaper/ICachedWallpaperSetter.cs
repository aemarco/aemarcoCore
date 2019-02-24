using aemarcoCore.Caching;
using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Wallpaper
{
    public interface ICachedWallpaperSetter : IWallpaperSetter
    {
        CacheMode CacheMode { get; set; }
        int CacheSizePercentage { get; set; }
        int CacheSizeMB { get; set; }

        CacheStats CacheStats { get; }


        Task ClearCache(CancellationToken ct);
    }
}
