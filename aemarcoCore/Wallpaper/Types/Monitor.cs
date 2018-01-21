using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace aemarcoCore.Wallpaper.Types
{
    internal class Monitor
    {
        private Rectangle _rectangle;
        private Image _wallpaper;
        private string _deviceName;
        private WallpaperMode _wallMode;
        private List<string> _sourceFiles;

        internal Monitor(Screen screen, string backgroundFile, WallpaperMode mode)
        {
            if (screen == null || backgroundFile == null)
            {
                throw new NullReferenceException("Monitor kann nicht initialisiert werden");
            }

            //set mandatory fields            
            _deviceName = screen.DeviceName;
            _wallMode = mode;
            _sourceFiles = new List<string>();
            _rectangle = new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height);


            foreach (Screen scr in Screen.AllScreens)
            {
                if (scr.Bounds.Left < _rectangle.X)
                    _rectangle.X = scr.Bounds.Left;
                if (scr.Bounds.Top < _rectangle.Y)
                    _rectangle.Y = scr.Bounds.Top;
            }
            _rectangle.X *= -1;
            _rectangle.Y *= -1;
            _rectangle.X += screen.Bounds.Left;
            _rectangle.Y += screen.Bounds.Top;

            if (File.Exists(backgroundFile))
            {
                try
                {
                    using (var old = new Bitmap(backgroundFile))
                    {
                        if (old.Width >= (_rectangle.X + _rectangle.Width) &&
                            old.Height >= (_rectangle.Y + _rectangle.Height))
                        {
                            _wallpaper = (Bitmap)old.Clone(_rectangle, old.PixelFormat);
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
            if (_wallpaper == null)
            {
                _wallpaper = new Bitmap(_rectangle.Width, _rectangle.Height);

            }

        }

        internal string DeviceName { get { return _deviceName; } }
        internal Rectangle Rectangle { get { return _rectangle; } }
        internal Image Wallpaper { get { return _wallpaper; } }


        private void SetDirectWallpaper(Image wall)
        {
            Bitmap targetImg = new Bitmap(_rectangle.Width, _rectangle.Height);
            Graphics g = Graphics.FromImage(targetImg);

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



        internal void SetWallpaperSourceList(List<string> files)
        {
            _sourceFiles = files;
        }

        internal void SetRandomWallpaper(Func<string, Image> getImage, Random rand)
        {
            if (_sourceFiles.Count < 1)
            {
                return;
            }

            int index = rand.Next(0, _sourceFiles.Count);
            Image wall = getImage(_sourceFiles[index]);
            SetWallpaper(wall);
        }
    }
}
