using aemarcoCore.Common;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace aemarcoCore.Wallpaper.Types
{
    internal class Monitor
    {

        #region fields

        private Rectangle _rectangle;
        private Image _wallpaper;
        private string _deviceName;

        private WallpaperMode _wallMode;

        #endregion

        #region ctor

        internal Monitor(Screen screen, string backgroundFile, WallpaperMode mode)
        {
            if (screen == null || backgroundFile == null)
            {
                throw new NullReferenceException("Monitor kann nicht initialisiert werden");
            }

            //set mandatory fields 
            _rectangle = new Rectangle(
                GetRectangleX(screen),
                GetRectangleY(screen),
                screen.Bounds.Width,
                screen.Bounds.Height);
            _wallpaper = GetPreviousImage(backgroundFile, _rectangle);
            _deviceName = screen.DeviceName;
            _wallMode = mode;
        }

        internal Monitor(Rectangle rect, string name, string backgroundFile, WallpaperMode mode)
        {
            if (backgroundFile == null)
            {
                throw new NullReferenceException("Monitor kann nicht initialisiert werden");
            }
            _rectangle = rect;            
            _deviceName = name;
            _wallMode = mode;

        }


        #endregion

        #region props

        internal string DeviceName { get { return _deviceName; } }
        internal Rectangle Rectangle { get { return _rectangle; } }
        internal Image Wallpaper { get { return _wallpaper; } }

        #endregion

        #region private

        private int GetRectangleX(Screen screen)
        {
            var minX = Screen.AllScreens.Min(x => x.Bounds.X);
            int result = -minX + screen.Bounds.Left;
            return result;
        }
        private int GetRectangleY(Screen screen)
        {
            var minY = Screen.AllScreens.Min(x => x.Bounds.Y);
            int result = -minY + screen.Bounds.Top;
            return result;
        }
        private Bitmap GetPreviousImage(string backgroundFile, Rectangle rectangle)
        {
            Bitmap result = null;

            if (File.Exists(backgroundFile))
            {
                try
                {
                    using (Bitmap old = new Bitmap(backgroundFile))
                    {
                        if (old.Width >= (rectangle.X + rectangle.Width) &&
                            old.Height >= (rectangle.Y + rectangle.Height))
                        {
                            result = (Bitmap)old.Clone(rectangle, old.PixelFormat);
                        }
                        else
                        {
                            throw new FileLoadException("Image size not compatible.");
                        }
                    }
                }
                catch
                {
                    File.Delete(backgroundFile);
                }
            }
            if (result == null)
            {
                result = new Bitmap(rectangle.Width, rectangle.Height);

            }

            return result;
        }

        private void SetDirectWallpaper(Image wall)
        {
            float heightRatio = (float)_rectangle.Height / (float)wall.Height;
            float widthRatio = (float)_rectangle.Width / (float)wall.Width;

            int height = _rectangle.Height;
            int width = _rectangle.Width;
            int x = 0;
            int y = 0;

            if (heightRatio < widthRatio) //Bild schmaler als Monitor
            {
                width = (int)((float)wall.Width * heightRatio);
                height = (int)((float)wall.Height * heightRatio);
                x = (int)((float)(_rectangle.Width - width) / 2f);
            }
            else //Bild breiter als Monitor
            {
                width = (int)((float)wall.Width * widthRatio);
                height = (int)((float)wall.Height * widthRatio);
                y = (int)((float)(_rectangle.Height - height) / 2f);
            }

            Rectangle drawTo = new Rectangle(x, y, width, height);

            Bitmap targetImg = new Bitmap(_rectangle.Width, _rectangle.Height);
            Graphics g = Graphics.FromImage(targetImg);
            g.DrawImage(wall, drawTo);
            _wallpaper = targetImg;
        }
        private void SetSnappedWallpaper(Image img)
        {
            Rectangle rect;
            if ((1.0 * _rectangle.Width / _rectangle.Height) - (1.0 * img.Width / img.Height) < 0) // Bild breiter als Monitor
            {
                rect = new Rectangle(0, 0, (int)((1.0 * _rectangle.Width / _rectangle.Height) * img.Height), img.Height);
                rect.X = (img.Width - rect.Width) / 2;
            }
            else // Bild schmaler als Monitor
            {
                rect = new Rectangle(0, 0, img.Width, (int)(1.0 * img.Width / (1.0 * _rectangle.Width / _rectangle.Height)));
                rect.Y = (img.Height - rect.Height) / 2;
            }
            SetDirectWallpaper(((Bitmap)img).Clone(rect, img.PixelFormat));
        }
        private void SetCuttedWallpaper(Image img)
        {
            Rectangle rect;
            if ((1.0 * _rectangle.Width / _rectangle.Height) - (1.0 * img.Width / img.Height) < 0) // Bild breiter als Monitor
            {
                double pixelsToCut = 1.0 * img.Width / 100 * ConfigurationHelper.PercentLeftRightCutAllowed;
                rect = new Rectangle(0, 0, img.Width - (int)pixelsToCut, img.Height);
                rect.X = (img.Width - rect.Width) / 2;
            }
            else // Bild schmaler als Monitor
            {
                double pixelsToCut = 1.0 * img.Height / 100 * ConfigurationHelper.PercentTopBottomCutAllowed;
                rect = new Rectangle(0, 0, img.Width, img.Height - (int)pixelsToCut);
                rect.Y = (img.Height - rect.Height) / 2;
            }

            SetDirectWallpaper(((Bitmap)img).Clone(rect, img.PixelFormat));
        }



        #endregion

        internal void SetWallpaper(Image wall)
        {
            if (wall == null)
            {
                throw new NullReferenceException("Wallpaper can´t be null");
            }

            switch (_wallMode)
            {
                case WallpaperMode.AllowFill:
                    {
                        if (WallpaperSetter.CanBeSnapped(wall.Width, wall.Height, _rectangle.Width, _rectangle.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            SetDirectWallpaper(wall);
                        }
                        break;
                    }
                case WallpaperMode.AllowFillForceCut:
                    {
                        if (WallpaperSetter.CanBeSnapped(wall.Width, wall.Height, _rectangle.Width, _rectangle.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            SetCuttedWallpaper(wall);
                        }
                        break;
                    }
                case WallpaperMode.Fit:
                    {
                        SetDirectWallpaper(wall);
                        break;
                    }
                case WallpaperMode.Fill:
                    {
                        SetSnappedWallpaper(wall);
                        break;
                    }
            }
        }

    }
}
