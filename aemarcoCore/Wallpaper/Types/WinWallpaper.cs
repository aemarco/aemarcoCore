using aemarcoCore.Common;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace aemarcoCore.Wallpaper.Types
{
    internal static class WinWallpaper
    {
        const int SETWALLPAPER = 20; //0x0014
        const int UPDATEINIFILE = 0x01;
        const int SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);



        internal static void SetWallpaper(string wallpaper, WindowsWallpaperStyle style)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                switch (style)
                {
                    case WindowsWallpaperStyle.Fit:
                        key.SetValue(@"WallpaperStyle", 6.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WindowsWallpaperStyle.Fill:
                        key.SetValue(@"WallpaperStyle", 10.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WindowsWallpaperStyle.Tile:
                        key.SetValue(@"WallpaperStyle", 0.ToString());
                        key.SetValue(@"TileWallpaper", 1.ToString());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            SystemParametersInfo(SETWALLPAPER, 0, wallpaper, UPDATEINIFILE | SENDWININICHANGE);
        }

    }




}
