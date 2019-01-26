using aemarcoCore.Caching;
using aemarcoCore.Common;
using aemarcoCore.Tools;
using aemarcoCore.Wallpaper.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aemarcoCore.Wallpaper
{
    public class WallpaperSetter
    {
        #region fields

        private readonly string _defaultBackgroundFile;
        private readonly WallpaperMode _wallMode;
        private readonly Random _rand;

        private readonly Dictionary<Monitor, List<string>> _monitorDictionary;
        private readonly HttpClient _client;
        private readonly CacheHandler _cacheHandler;

        #endregion

        #region ctor

        /// <summary>
        /// Use this Instance to handle setting Wallpapers
        /// </summary>
        /// <param name="mode">
        ///  Fit: Places the Wallpaper as big as possible without cutting (black bars)
        ///  Fill: Cuts as much needed to fill the screen
        ///  AllowFill: Decides automatically between Fill and Fit based on allowed cutting
        ///  AllowFillForceCut (default): Like AllowFill, otherwise Fit with allowed cutting 
        /// </param>
        public WallpaperSetter(WallpaperSetterSettings settings = null)
        {
            if (settings == null) settings = new WallpaperSetterSettings();

            _defaultBackgroundFile = new FileInfo("CurrentWallpaper.jpg").FullName;
            _wallMode = settings.WallpaperMode;
            _rand = new Random();
            _monitorDictionary = new Dictionary<Monitor, List<string>>();

            foreach (Screen scr in Screen.AllScreens)
            {
                var mon = new Monitor(scr, _defaultBackgroundFile, _wallMode);
                _monitorDictionary.Add(mon, null);
            }

            //adding Virtual Screen as well.
            var allScreenRect = new Rectangle(0, 0, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(allScreenRect, Constants.VIRTUALSCREEN_NAME, _defaultBackgroundFile, _wallMode);
            _monitorDictionary.Add(virtualMon, null);


            _client = new HttpClient(new WebRequestHandler());

            if (settings.CacheMode != CacheMode.None)
            {
                _cacheHandler = new CacheHandler(_client, settings.CacheMode)
                {
                    CacheSize = settings.CacheSizeInBytes,
                    CacheSizePercentage = settings.CacheSizePercentage,
                    FolderDepth = settings.FolderDepth
                };
            }

        }

        #endregion

        #region private

        private void SetBackgroundImage()
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    foreach (var mon in _monitorDictionary.Keys)
                    {
                        if (mon.DeviceName == Constants.VIRTUALSCREEN_NAME)
                        {
                            continue;
                        }
                        virtualScreenGraphic.DrawImage(mon.Wallpaper, mon.Rectangle);
                    }
                    virtualScreenBitmap.Save(_defaultBackgroundFile, ImageFormat.Jpeg);
                }
            }
            WinWallpaper.SetWallpaper(_defaultBackgroundFile);
        }

       

        private void SetVirtualBackgroundImage()
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    var mon = _monitorDictionary.Keys.Where(x => x.DeviceName == Constants.VIRTUALSCREEN_NAME).First();
                    virtualScreenGraphic.DrawImage(mon.Wallpaper, mon.Rectangle);
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
                    if (_cacheHandler != null)
                    {
                        using (var stream = _cacheHandler.GetEntryFromCache(file))
                        {
                            using (var bmpTemp = new Bitmap(stream))
                                return new Bitmap(bmpTemp);
                        }
                    }
                    else
                    {
                        var resp = _client.GetAsync(file, HttpCompletionOption.ResponseHeadersRead).Result;
                        resp.EnsureSuccessStatusCode();

                        using (Stream stream = resp.Content.ReadAsStreamAsync().Result)
                        {
                            Image img = Image.FromStream(stream);
                            return img;
                        }
                    }
                }  
                else
                {
                    using (var bmpTemp = new Bitmap(file))
                        return new Bitmap(bmpTemp);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }

        }
        

        #endregion



        /// <summary>
        /// Sets given Wallpaper foreach Screen
        /// </summary>
        /// <param name="file">Wallpaper to set</param>
        public void SetWallOnAllScreen(string file)
        {
            foreach (Monitor mon in _monitorDictionary.Keys)
            {
                if (mon.DeviceName == Constants.VIRTUALSCREEN_NAME)
                {
                    continue;
                }
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
                screens.Count < 1 || screens.Count != images.Count)
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            for (int i = 0; i < screens.Count; i++)
            {
                Monitor mon = _monitorDictionary.Keys.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                if (mon == null)
                {
                    Screen scr = Screen.AllScreens.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                    if (scr == null)
                    {
                        continue;
                    }
                    mon = new Monitor(scr, _defaultBackgroundFile, _wallMode);
                    _monitorDictionary.Add(mon, null);
                }

                if (images[i] != null)
                    mon.SetWallpaper(images[i]);
                
            }

            if (screens.Contains(Constants.VIRTUALSCREEN_NAME))
            {
                SetVirtualBackgroundImage();
            }
            else
            {
                SetBackgroundImage();
            }
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
            double imageRatio = 1.0 * imageWidth / imageHeight;
            var (minRatio, maxRatio) = GetRatioRange(monitorWidth, monitorHeight);

            return (imageRatio <= maxRatio && imageRatio >= minRatio);
        }

        /// <summary>
        /// returns the min and max Ratio for which Pictures can be Snapped
        /// </summary>
        /// <param name="monitorWidth"></param>
        /// <param name="monitorHeight"></param>
        /// <returns>minRatio and maxRatio</returns>
        public static (double minRatio, double maxRatio) GetRatioRange(int monitorWidth, int monitorHeight,
            int percentLeftRightCutAllowed = -1,
            int percentTopBottomCutAllowed = -1)
        {
            if (percentLeftRightCutAllowed == -1) percentLeftRightCutAllowed = ConfigurationHelper.PercentLeftRightCutAllowed;
            if (percentTopBottomCutAllowed == -1) percentTopBottomCutAllowed = ConfigurationHelper.PercentTopBottomCutAllowed;
            
            double monitorRatio = 1.0 * monitorWidth / monitorHeight;

            
            double maxWidth = 100.0 * monitorWidth / (100 - percentLeftRightCutAllowed);
            double maxHeight = 100.0 * monitorHeight / (100 - percentTopBottomCutAllowed);

            var minratio = monitorWidth / maxHeight;
            var maxratio = maxWidth / monitorHeight;
            if (maxratio == double.PositiveInfinity)
                maxratio = double.MaxValue;


            return (minratio, maxratio);
        }


        public void ClearCache()
        {
            if (_cacheHandler != null)
            {
                _cacheHandler.ClearCache();
            }
        }




        /// <summary>
        /// Sets a list for Random Wallpaper Function
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="files">Wallpapers to set</param>
        public void SetWallpaperSourceList(string screen, IEnumerable<string> files)
        {
            if (screen == null || files == null ||
                !files.Any() || files.Contains(null))
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            Monitor mon = _monitorDictionary.Keys.Where(x => x.DeviceName == screen).FirstOrDefault();
            if (mon == null)
            {
                throw new ArgumentException($"Monitor {screen} not found.");
            }

            _monitorDictionary[mon] = files.ToList();
        }

        /// <summary>
        /// Sets a random Wallpaper on each Screen a list was provided
        /// </summary>
        public void SetRandomWallpaper()
        {

            foreach (var pair in _monitorDictionary)
            {
                if (pair.Key.DeviceName == Constants.VIRTUALSCREEN_NAME)
                {
                    continue;
                }

                var source = pair.Value;
                if (source == null)
                {
                    source = WallCrawlerData.GetKnownUrls();
                }

                int index = _rand.Next(0, source.Count);
                Image wall = GetImage(source[index]);
                pair.Key.SetWallpaper(wall);
            }

            SetBackgroundImage();
        }


    }
}
