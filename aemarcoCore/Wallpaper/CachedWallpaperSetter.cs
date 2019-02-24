using aemarcoCore.Caching;
using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace aemarcoCore.Wallpaper
{
    public class CachedWallpaperSetter : WallpaperSetter, ICachedWallpaperSetter
    {
        #region fields

        private readonly CacheHandler _cacheHandler;

        #endregion

        #region ctor


        public CachedWallpaperSetter(WallpaperMode wallpaperMode, int cacheSizeMB = 250, int cacheSizePercentage = 5, int folderDepth = 4)
            : base(wallpaperMode)
        {
            _cacheHandler = new CacheHandler(_client, CacheMode.BySize)
            {
                CacheSize = (long)cacheSizeMB * 1048576,
                CacheSizePercentage = cacheSizePercentage,
                FolderDepth = folderDepth
            };
        }

        #endregion

        #region props

        public CacheMode CacheMode
        {
            get { return _cacheHandler.CacheMode; }
            set { _cacheHandler.CacheMode = value; }
        }
        public int CacheSizePercentage
        {
            get { return _cacheHandler.CacheSizePercentage; }
            set { _cacheHandler.CacheSizePercentage = value; }
        }
        public int CacheSizeMB
        {
            get { return (int)(_cacheHandler.CacheSize / 1048576); }
            set { _cacheHandler.CacheSize = (long)value * 1048576; }
        }


        public CacheStats CacheStats
        {
            get
            {
                switch (CacheMode)
                {
                    case CacheMode.None:
                        return null;
                    case CacheMode.BySize:
                    case CacheMode.Auto:
                        return _cacheHandler.CacheStats;
                    default:
                        throw new NotImplementedException();
                }
            }
        }


        #endregion



        protected override async Task<Image> GetImage(string file)
        {
            if (file.StartsWith("http") && _cacheHandler.CacheMode != CacheMode.None)
            {
                using (var stream = await _cacheHandler.GetEntryFromCache(file))
                {
                    using (var bmpTemp = new Bitmap(stream))
                        return new Bitmap(bmpTemp);
                }
            }
            else
            {
                return await base.GetImage(file);
            }
        }


        public async Task ClearCache(System.Threading.CancellationToken ct)
        {
            await _cacheHandler.ClearCache(ct);

        }

    }
}
