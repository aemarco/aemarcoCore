using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using aemarcoCore.Caching;
using aemarcoCore.Common;

namespace aemarcoCore.Wallpaper
{
    public interface IWallpaperSetter
    {
        WallpaperMode Wallpapermode { get; set; }

        Task SetSameWallOnEveryScreen(string file);
        Task SetWallForScreen(string screen, string file);
        void SetWallForScreen(string screen, Image image);
        Task SetWallsForScreens(List<string> screens, List<string> files);
        void SetWallsForScreens(List<string> screens, List<Image> images);


        void SetWallpaperSourceList(string screen, IEnumerable<string> files);
        Task SetRandomWallpaper(bool individualScreens = true);
    }
}