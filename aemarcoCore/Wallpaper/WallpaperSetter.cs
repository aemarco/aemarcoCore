using aemarcoCore.Common;
using aemarcoCore.Properties;
using aemarcoCore.Tools;
using aemarcoCore.Wallpaper.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace aemarcoCore.Wallpaper
{
    public class WallpaperSetter
    {

        private string _defaultBackgroundFile;
        private List<Monitor> _monitors;
        private WallpaperMode _wallMode;
        Random _rand;

        private static int percentLeftRightCutAllowed;
        private static int percentTopBottomCutAllowed;

        /// <summary>
        /// Use this Instance to handle stting Wallpapers
        /// </summary>
        /// <param name="mode">
        ///  Fit: Places the Wallpaper as big as possible without cutting (black bars)
        ///  Fill: Cuts the Wallpaper and fills the screen
        ///  AllowFill: Decides automatically between Fit and Fill
        /// </param>
        public WallpaperSetter(WallpaperMode mode = WallpaperMode.AllowFill)
        {
            _defaultBackgroundFile = new FileInfo("CurrentWallpaper.jpg").FullName;
            _wallMode = mode;
            _rand = new Random();
            _monitors = new List<Monitor>();
            foreach (Screen scr in Screen.AllScreens)
            {
                _monitors.Add(new Monitor(scr, _defaultBackgroundFile, _wallMode));
            }

        }



        private void SetBackgroundImage()
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    foreach (var mon in _monitors)
                    {
                        virtualScreenGraphic.DrawImage(mon.Wallpaper, mon.Rectangle);
                    }
                    virtualScreenBitmap.Save(_defaultBackgroundFile, ImageFormat.Jpeg);
                }
            }
            WinWallpaper.SetWallpaper(_defaultBackgroundFile);
        }

        private Image GetImage(string file)
        {
            try
            {
                if (file.StartsWith("http"))
                {
                    using (Stream stream = new WebClient().OpenRead(file))
                        return Image.FromStream(stream);
                }
                else
                {
                    using (var bmpTemp = new Bitmap(file))
                        return new Bitmap(bmpTemp);
                }
            }
            catch { return null; }
        }



        /// <summary>
        /// Sets given Wallpaper on all Screens
        /// </summary>
        /// <param name="file">Wallpaper to set</param>
        public void SetWallOnAllScreen(string file)
        {
            foreach (Monitor mon in _monitors)
            {
                mon.SetWallpaper(GetImage(file));
            }
            SetBackgroundImage();
        }


        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="file">Wallpaper to set</param>
        public void SetWallForScreen(string screen, string file)
        {
            if (screen == null || file == null)
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            Image image = GetImage(file);
            SetWallForScreen(screen, image);
        }
        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="image">Image to set</param>
        public void SetWallForScreen(string screen, Image image)
        {
            if (screen == null || image == null)
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            SetWallsForScreens(new List<string> { screen }, new List<Image> { image });
        }


        /// <summary>
        /// Sets given Wallpapers to given Screens
        /// </summary>
        /// <param name="screens">Screen Device names</param>
        /// <param name="bitmaps">Wallpapers to set</param>
        public void SetWallsForScreens(List<string> screens, List<string> files)
        {
            if (screens == null || files == null ||
                screens.Count < 1 || screens.Count != files.Count ||
                screens.Contains(null) || files.Contains(null))
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            List<Image> images = new List<Image>();
            foreach (var file in files)
            {
                images.Add(GetImage(file));
            }
            SetWallsForScreens(screens, images);
        }
        /// <summary>
        /// Sets given Wallpapers to given Screens
        /// </summary>
        /// <param name="screens">Screen Device names</param>
        /// <param name="images">Image to set on those screens</param>
        public void SetWallsForScreens(List<string> screens, List<Image> images)
        {
            if (screens == null || images == null ||
                screens.Count < 1 || screens.Count != images.Count ||
                screens.Contains(null) || images.Contains(null))
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            for (int i = 0; i < screens.Count; i++)
            {
                Monitor mon = _monitors.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                if (mon == null)
                {
                    Screen scr = Screen.AllScreens.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                    if (scr == null)
                    {
                        continue;
                    }
                    mon = new Monitor(scr, _defaultBackgroundFile, _wallMode);
                    _monitors.Add(mon);
                }
                mon.SetWallpaper(images[i]);
            }
            SetBackgroundImage();
        }


        /// <summary>
        /// Returns true if the Image can be snapped to the desired Monitor.
        /// Tolerated cutting amount can be adjusted in the AppSettings
        /// </summary>
        /// <param name="imageWidth">Width of Image</param>
        /// <param name="imageHeight">Height of Image</param>
        /// <param name="monitorWidth">Width of Monitor</param>
        /// <param name="monitorHeight">Height of Monitor</param>
        /// <returns></returns>
        public static bool CanBeSnapped(int imageWidth, int imageHeight, int monitorWidth, int monitorHeight)
        {
            if (percentLeftRightCutAllowed == default(int))
            {
                percentLeftRightCutAllowed = Settings.Default.WallpaperPercentCutAllowedLeftRight;
                percentTopBottomCutAllowed = Settings.Default.WallpaperPercentCutAllowedTopBottom;
            }


            double imageRatio = 1.0 * imageWidth / imageHeight;
            double monitorRatio = 1.0 * monitorWidth / monitorHeight;

            if (monitorRatio - imageRatio < 0) // Bild breiter als Monitor (links und rechts schneiden)
            {
                int width = (int)(monitorRatio * imageHeight);
                return (imageWidth - width <= 1.0 * imageWidth / 100 * percentLeftRightCutAllowed);
            }
            else  // Bild schmaler als Monitor (oben und unten schneiden)
            {
                int height = (int)(1.0 * imageWidth / (1.0 * monitorWidth / monitorHeight));
                return (imageHeight - height <= 1.0 * imageHeight / 100 * percentTopBottomCutAllowed);
            }
        }


        /// <summary>
        /// Sets a list for Random Wallpaper Function
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="files">Wallpapers to set</param>
        public void SetWallpaperSourceList(string screen, List<string> files)
        {
            if (screen == null || files == null ||
                files.Count < 1 || files.Contains(null))
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            Monitor mon = _monitors.Where(x => x.DeviceName == screen).FirstOrDefault();
            if (mon == null)
            {
                throw new ArgumentException($"Monitor {screen} not found.");
            }

            mon.SetWallpaperSourceList(files);

        }

        /// <summary>
        /// Sets a random Wallpaper on each Screen a list was provided
        /// </summary>
        public void SetRandomWallpaper()
        {
            foreach (var mon in _monitors)
            {
                mon.SetRandomWallpaper(GetImage, _rand);
            }

            SetBackgroundImage();
        }


    }
}
