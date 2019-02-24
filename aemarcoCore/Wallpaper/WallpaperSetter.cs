using aemarcoCore.Caching;
using aemarcoCore.Common;
using aemarcoCore.Tools;
using aemarcoCore.Wallpaper.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace aemarcoCore.Wallpaper
{
    public class WallpaperSetter : IWallpaperSetter
    {
        #region fields

        private readonly string _defaultBackgroundFile;
        private WallpaperMode _wallMode;
        private readonly Random _rand;

        private readonly Dictionary<Monitor, List<string>> _monitorDictionary;
        protected readonly HttpClient _client;

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
        public WallpaperSetter(WallpaperMode wallMode)
        {
            _defaultBackgroundFile = new FileInfo("CurrentWallpaper.jpg").FullName;
            _wallMode = wallMode;
            _rand = new Random();

            _monitorDictionary = new Dictionary<Monitor, List<string>>();
            foreach (Screen scr in Screen.AllScreens)
            {
                var mon = new Monitor(scr.Bounds, scr.DeviceName, _defaultBackgroundFile, _wallMode);
                _monitorDictionary.Add(mon, null);
            }
            //adding Virtual Screen as well.
            var allScreenRect = new Rectangle(
                Screen.AllScreens.Min(x => x.Bounds.X),
                Screen.AllScreens.Min(x => x.Bounds.Y),
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(allScreenRect, Constants.VIRTUALSCREEN_NAME, _defaultBackgroundFile, _wallMode);
            _monitorDictionary.Add(virtualMon, null);

            _client = new HttpClient(new WebRequestHandler());
        }

        #endregion

        #region props

        public WallpaperMode Wallpapermode
        {
            get { return _wallMode; }
            set
            {
                if (value != _wallMode)
                {
                    _wallMode = value;
                    foreach (var key in _monitorDictionary.Keys)
                        key.WallpaperMode = value;
                }
            }
        }

        #endregion

        #region private

        private void SetBackgroundImage(WallpaperSetMode mode)
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    switch (mode)
                    {
                        case WallpaperSetMode.VirtualScreen:
                            {
                                var mon = _monitorDictionary.Keys.Where(x => x.DeviceName == Constants.VIRTUALSCREEN_NAME).First();
                                mon.DrawToGraphics(virtualScreenGraphic);
                                break;
                            }
                        case WallpaperSetMode.IndividualScreens:
                            {
                                foreach (var mon in _monitorDictionary.Keys
                                    .Where(x => x.DeviceName != Constants.VIRTUALSCREEN_NAME))
                                {
                                    mon.DrawToGraphics(virtualScreenGraphic);
                                }
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                virtualScreenBitmap.Save(_defaultBackgroundFile, ImageFormat.Jpeg);
            }
            WinWallpaper.SetWallpaper(_defaultBackgroundFile, WindowsWallpaperStyle.Tile);
        }

        protected virtual async Task<Image> GetImage(string file)
        {
            if (file.StartsWith("http"))
            {
                var resp = await _client.GetAsync(file, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                using (Stream stream = await resp.Content.ReadAsStreamAsync())
                {
                    Image img = Image.FromStream(stream);
                    return img;
                }
            }
            else
            {
                using (var bmpTemp = new Bitmap(file))
                    return new Bitmap(bmpTemp);
            }
        }

        #endregion

        #region Setting Wall

        //eins auf allen Monitoren
        /// <summary>
        /// Sets given Wallpaper foreach Screen
        /// </summary>
        /// <param name="file">Wallpaper to set</param>
        public async Task SetSameWallOnEveryScreen(string file)
        {

            switch (_wallMode)
            {
                case WallpaperMode.Fit:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fit);
                    break;
                case WallpaperMode.AllowFill:
                case WallpaperMode.AllowFillForceCut:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fill);
                    break;
                case WallpaperMode.Fill:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fill);
                    break;
                default:
                    throw new NotImplementedException();
            }
            await Task.CompletedTask;

            //foreach (Monitor mon in _monitorDictionary.Keys
            //    .Where(x => x.DeviceName != Constants.VIRTUALSCREEN_NAME))
            //{
            //    mon.SetWallpaper(await GetImage(file));
            //}
            //SetBackgroundImage(WallpaperSetMode.IndividualScreens);
        }


        //einzelne
        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="file">Wallpaper to set</param>
        public async Task SetWallForScreen(string screen, string file)
        {
            if (String.IsNullOrWhiteSpace(screen) || String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            Image image = await GetImage(file);
            SetWallForScreen(screen, image);
        }
        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="image">Image to set</param>
        public void SetWallForScreen(string screen, Image image)
        {
            if (String.IsNullOrWhiteSpace(screen) || image == null)
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            SetWallsForScreens(new List<string> { screen }, new List<Image> { image });
        }

        //mehrere
        /// <summary>
        /// Sets given Wallpapers to given Screens
        /// </summary>
        /// <param name="screens">Screen Device names</param>
        /// <param name="bitmaps">Wallpapers to set</param>
        public async Task SetWallsForScreens(List<string> screens, List<string> files)
        {
            if (screens == null) throw new ArgumentNullException(nameof(screens));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (files.Contains(null)) throw new ArgumentNullException($"inside {nameof(files)}");
            if (screens.Count == 0) throw new ArgumentException(nameof(screens));
            if (screens.Count != files.Count) throw new ArgumentException(nameof(files));


            List<Image> images = new List<Image>();
            foreach (var file in files)
            {
                images.Add(await GetImage(file));
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
            if (screens == null) throw new ArgumentNullException(nameof(screens));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (images.Contains(null)) throw new ArgumentNullException($"inside {nameof(images)}");
            if (screens.Count == 0) throw new ArgumentException(nameof(screens));
            if (screens.Count != images.Count) throw new ArgumentException(nameof(images));


            for (int i = 0; i < screens.Count; i++)
            {
                Monitor mon = _monitorDictionary.Keys.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                if (mon == null)
                {
                    Screen scr = Screen.AllScreens.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                    if (scr == null) continue;
                    mon = new Monitor(scr.Bounds, scr.DeviceName, _defaultBackgroundFile, _wallMode);
                    _monitorDictionary.Add(mon, null);
                }
                mon.SetWallpaper(images[i]);
            }


            if (screens.Contains(Constants.VIRTUALSCREEN_NAME))
            {
                SetBackgroundImage(WallpaperSetMode.VirtualScreen);
            }
            else
            {
                SetBackgroundImage(WallpaperSetMode.IndividualScreens);
            }
        }


        //random stuff
        /// <summary>
        /// Sets a list for Random Wallpaper Function
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="files">Wallpapers to set</param>
        public void SetWallpaperSourceList(string screen, IEnumerable<string> files)
        {
            if (String.IsNullOrWhiteSpace(screen) || files == null ||
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
        public async Task SetRandomWallpaper(bool individualScreens = true)
        {
            foreach (var pair in _monitorDictionary)
            {
                if ((individualScreens && pair.Key.DeviceName != Constants.VIRTUALSCREEN_NAME) ||
                    !individualScreens && pair.Key.DeviceName == Constants.VIRTUALSCREEN_NAME)
                {

                    var sourceList = pair.Value;
                    if (sourceList == null)
                    {
                        sourceList = WallCrawlerData.GetKnownUrls();
                    }

                    int index = _rand.Next(0, sourceList.Count);
                    Image wall = await GetImage(sourceList[index]);
                    pair.Key.SetWallpaper(wall);
                }
            }

            if (individualScreens)
                SetBackgroundImage(WallpaperSetMode.IndividualScreens);
            else
                SetBackgroundImage(WallpaperSetMode.VirtualScreen);
        }

        #endregion

        #region statics

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
        /// returns the min and max Ratio for which Pictures can be Snapped based on allowed % cuts
        /// </summary>
        /// <param name="monitorWidth"></param>
        /// <param name="monitorHeight"></param>
        /// <param name="percentLeftRightCutAllowed"></param>
        /// <param name="percentTopBottomCutAllowed"></param>
        /// <returns>minRatio and maxRatio</returns>
        public static (double minRatio, double maxRatio) GetRatioRange(int monitorWidth, int monitorHeight,
            int percentLeftRightCutAllowed = -1,
            int percentTopBottomCutAllowed = -1)
        {
            if (percentLeftRightCutAllowed == -1) percentLeftRightCutAllowed = ConfigurationHelper.PercentLeftRightCutAllowed;
            if (percentTopBottomCutAllowed == -1) percentTopBottomCutAllowed = ConfigurationHelper.PercentTopBottomCutAllowed;

            double maxWidth = 100.0 * monitorWidth / (100 - percentLeftRightCutAllowed);
            double maxHeight = 100.0 * monitorHeight / (100 - percentTopBottomCutAllowed);

            var minratio = monitorWidth / maxHeight;
            var maxratio = maxWidth / monitorHeight;
            if (maxratio == double.PositiveInfinity)
                maxratio = double.MaxValue;

            return (minratio, maxratio);
        }

        #endregion

    }
}
