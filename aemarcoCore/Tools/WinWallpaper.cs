using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace aemarcoCore.Tools
{
    internal static class WinWallpaper
    {


        const int SETWALLPAPER = 20;
        const int UPDATEINIFILE = 0x01;
        const int SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);



        internal static void SetWallpaper(string wallpaper)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SETWALLPAPER, 0, wallpaper, UPDATEINIFILE | SENDWININICHANGE);
        }






    }




}
