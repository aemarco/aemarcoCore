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

        #endregion

        #region ctor

        internal Monitor(Rectangle rect, string name, string backgroundFile, WallpaperMode mode)
        {
            if (String.IsNullOrWhiteSpace(backgroundFile) || String.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException("Monitor kann nicht initialisiert werden");
            }


            //set mandatory fields 
            _rectangle = new Rectangle(
                GetRectangleX(rect),
                GetRectangleY(rect),
                rect.Width,
                rect.Height);


            _wallpaper = GetPreviousImage(backgroundFile, _rectangle);
            DeviceName = name;
            WallpaperMode = mode;
        }
        #endregion

        #region props

        internal string DeviceName { get; }
        public WallpaperMode WallpaperMode { get; internal set; }

        #endregion

        #region private

        private int GetRectangleX(Rectangle rect)
        {
            var minX = Screen.AllScreens.Min(x => x.Bounds.X);
            int result = -minX + rect.Left;
            return result;
        }
        private int GetRectangleY(Rectangle rect)
        {
            var minY = Screen.AllScreens.Min(x => x.Bounds.Y);
            int result = -minY + rect.Top;
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
                            result = new Bitmap(old.Clone(rectangle, old.PixelFormat));
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
                    result = new Bitmap(_rectangle.Width, _rectangle.Height);
                }
            }
            return result;
        }


        /// <summary>
        /// Sets the Picture as big as possible with Black bars if needed
        /// </summary>
        /// <param name="readyToUsePicture"></param>
        private void SetDirectWallpaper(Image readyToUsePicture)
        {
            float heightRatio = (float)_rectangle.Height / (float)readyToUsePicture.Height;
            float widthRatio = (float)_rectangle.Width / (float)readyToUsePicture.Width;

            int height, width;
            int x = 0;
            int y = 0;

            if (heightRatio < widthRatio) //Bild schmaler als Monitor
            {
                width = (int)((float)readyToUsePicture.Width * heightRatio);
                height = (int)((float)readyToUsePicture.Height * heightRatio);
                x = (int)((float)(_rectangle.Width - width) / 2f);
            }
            else //Bild breiter als Monitor
            {
                width = (int)((float)readyToUsePicture.Width * widthRatio);
                height = (int)((float)readyToUsePicture.Height * widthRatio);
                y = (int)((float)(_rectangle.Height - height) / 2f);
            }

            Rectangle drawTo = new Rectangle(x, y, width, height);

            Bitmap targetImg = new Bitmap(_rectangle.Width, _rectangle.Height);
            Graphics g = Graphics.FromImage(targetImg);
            g.DrawImage(readyToUsePicture, drawTo);
            _wallpaper = targetImg;
        }
        /// <summary>
        /// Sets the Picture and fills the screen by cutting the Picture
        /// </summary>
        /// <param name="pictureToBeCutted"></param>
        private void SetSnappedWallpaper(Image pictureToBeCutted)
        {
            Rectangle rect;
            double targetRatio = 1.0 * _rectangle.Width / _rectangle.Height;

            if (targetRatio < (1.0 * pictureToBeCutted.Width / pictureToBeCutted.Height))
            {   // ratio zu groß
                int targetWidth = (int)(targetRatio * pictureToBeCutted.Height);
                rect = new Rectangle(0, 0, targetWidth, pictureToBeCutted.Height);
                rect.X = (pictureToBeCutted.Width - rect.Width) / 2;
            }
            else
            {
                // ratio zu klein
                int targetHeight = (int)(pictureToBeCutted.Width / targetRatio);
                rect = new Rectangle(0, 0, pictureToBeCutted.Width, targetHeight);
                rect.Y = (pictureToBeCutted.Height - rect.Height) / 2;
            }

            if (rect.X == 0 && rect.Y == 0)
            {   // ratio stimmt überein
                SetDirectWallpaper(pictureToBeCutted);
            }
            else
            {
                SetDirectWallpaper(((Bitmap)pictureToBeCutted).Clone(rect, pictureToBeCutted.PixelFormat));
            }
        }
        /// <summary>
        /// Cuts the Picture by the allowed amount and sets it as big as possible with black bars.
        /// Should be called only if it can´t be "Snapped"
        /// </summary>
        /// <param name="pictureToBeCutted"></param>
        private void SetCuttedWallpaper(Image pictureToBeCutted)
        {
            Rectangle rect;
            double targetRatio = 1.0 * _rectangle.Width / _rectangle.Height;

            if (targetRatio < 1.0 * pictureToBeCutted.Width / pictureToBeCutted.Height)
            {   // ratio zu groß
                double pixelsToCut = 1.0 * pictureToBeCutted.Width / 100 * ConfigurationHelper.PercentLeftRightCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCutted.Width - (int)pixelsToCut, pictureToBeCutted.Height);
                rect.X = (pictureToBeCutted.Width - rect.Width) / 2;
            }
            else
            {   // ratio zu klein
                double pixelsToCut = 1.0 * pictureToBeCutted.Height / 100 * ConfigurationHelper.PercentTopBottomCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCutted.Width, pictureToBeCutted.Height - (int)pixelsToCut);
                rect.Y = (pictureToBeCutted.Height - rect.Height) / 2;
            }

            SetDirectWallpaper(((Bitmap)pictureToBeCutted).Clone(rect, pictureToBeCutted.PixelFormat));
        }



        #endregion

        internal void SetWallpaper(Image wall)
        {
            if (wall == null)
            {
                throw new NullReferenceException("Wallpaper can´t be null");
            }

            switch (WallpaperMode)
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

        internal void DrawToGraphics(Graphics g)
        {
            g.DrawImage(_wallpaper, _rectangle);
        }

    }
}
