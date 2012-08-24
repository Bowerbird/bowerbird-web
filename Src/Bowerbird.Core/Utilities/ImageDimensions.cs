/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System;

namespace Bowerbird.Core.Utilities
{
    public enum ImageResizeMode
    {
        Crop, Normal, Fill
    }

    public enum ImageOrientation
    {
        Portrait, Landscape, Square
    }

    public class ImageDimensions
    {

        #region Fields

        #endregion

        #region Constructors

        public ImageDimensions(
            int width,
            int height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Properties

        public int Width { get; private set; }

        public int Height { get; private set; }

        public ImageOrientation Orientation
        {
            get
            {
                if (Width > Height)
                {
                    return ImageOrientation.Landscape;
                }
                else if (Height > Width)
                {
                    return ImageOrientation.Portrait;
                }
                else
                {
                    return ImageOrientation.Square;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Flips the orientation from Portrait to Landscape (or vice-versa), or leaves as Square
        /// </summary>
        public ImageDimensions FlipOrientation()
        {
            return new ImageDimensions(Height, Width);
        }

        /// <summary>
        /// Calculates the new height, based on given target width, keeping aspect ratio
        /// </summary>
        public ImageDimensions ResizeWithTargetWidth(int targetWidth)
        {
            return new ImageDimensions(targetWidth, CalcDimension(Height, Width, targetWidth));
        }

        /// <summary>
        /// Calculates the new width, based on given target height, keeping aspect ratio
        /// </summary>
        public ImageDimensions ResizeWithTargetHeight(int targetHeight)
        {
            return new ImageDimensions(CalcDimension(Width, Height, targetHeight), targetHeight);
        }

        /// <summary>
        /// Calculates the new width and height, based on given target width and height, keeping aspect ratio
        /// </summary>
        public ImageDimensions ResizeWithTargetDimensions(int targetWidth, int targetHeight)
        {
            switch (Orientation)
            {
                case ImageOrientation.Portrait:
                    return ResizeWithTargetHeight(targetHeight);
                case ImageOrientation.Landscape:
                case ImageOrientation.Square:
                default:
                    return ResizeWithTargetWidth(targetWidth);
            }
        }

        /// <summary>
        /// To calculate dimensions, keeping aspect ratios:
        /// Get height: original height / original width x target width = new height
        /// Get Width: original width / original height x target height = new width
        /// </summary>
        /// <param name="x1">If calculating target height, then original height, if calculating target width, then original width</param>
        /// <param name="x2">If calculating target width, then original width, if calculating target height, then original height</param>
        /// <param name="target">The target width or height</param>
        /// <returns>Target width or height</returns>
        public static int CalcDimension(int x1, int x2, int target)
        {
            return (int)Math.Round(((double)x1 / (double)x2) * (double)target);
        }

        /// <summary>
        /// Makes a new image dimensions
        /// </summary>
        public static ImageDimensions MakeRectangle(int width, int height)
        {
            return new ImageDimensions(width, height);
        }

        /// <summary>
        /// Makes a new square image dimensions
        /// </summary>
        public static ImageDimensions MakeSquare(int widthAndHeight)
        {
            return new ImageDimensions(widthAndHeight, widthAndHeight);
        }

        #endregion

    }
}
