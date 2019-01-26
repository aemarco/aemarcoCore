using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aemarcoCore.Wallpaper
{
    public class WallpaperSetterSettings
    {

        public WallpaperMode WallpaperMode { get; set; } = WallpaperMode.AllowFillForceCut;
        public CacheMode CacheMode { get; set; } = CacheMode.None;
        public long CacheSizeInBytes { get; set; } = 262144000;
        public int CacheSizePercentage { get; set; } = 5;
        public int FolderDepth { get; set; } = 4;
    }
}
