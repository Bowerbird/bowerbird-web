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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Bowerbird.Core.ImageUtilities;
using ExifLib;
using NLog;

namespace Bowerbird.Core.Services
{
    public class ImageService : IImageService
    {
        #region Fields

        private Logger _logger = LogManager.GetLogger("ImageService");

        public const string MIME_TYPE_JPEG = @"image/jpeg";

        private Bitmap _sourceImage;
        private Bitmap _newImage;
        private Stream _imageStream;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor to avoid conflicts with chaining initialisation
        /// </summary>
        private ImageService(Bitmap image, Stream imageStream)
        {
            _sourceImage = image;
            _newImage = image;
            _imageStream = imageStream;
        }

        #endregion

        #region Methods

        public static ImageService Load(Stream imageStream)
        {
            Bitmap image = null;

            imageStream.Seek(0, SeekOrigin.Begin);

            image = Image.FromStream(imageStream) as Bitmap;

            imageStream.Seek(0, SeekOrigin.Begin);

            return new ImageService(image, imageStream);
        }

        public ImageService SaveAs(out Stream imageStream)
        {
            imageStream = Stream.Null;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

                    _newImage.Save(memoryStream, GetEncoderInfo(MIME_TYPE_JPEG), encoderParams);

                    imageStream = memoryStream;
                }
            }
            catch (Exception)
            {
                if (_newImage != null)
                {
                    _newImage.Dispose();
                    _newImage = null;
                }

                throw;
            }

            return this;
        }

        public ImageService SaveAs(string filename)
        {
            try
            {
                _newImage.Save(filename, ImageFormat.Jpeg);
            }
            catch (Exception exception)
            {
                if (_newImage != null)
                {
                    _newImage.Dispose();
                    _newImage = null;
                }

                throw exception;
            }

            return this;
        }

        public ImageService Resize(ImageDimensions targetImageDimensions, bool determineBestOrientation, ImageResizeMode imageResizeMode)
        {
            int sourceWidth = _newImage.Width;
            int sourceHeight = _newImage.Height;

            int targetWidth = targetImageDimensions.Width;
            int targetHeight = targetImageDimensions.Height;

            if (determineBestOrientation)
            {
                // Supplied image is landscape, while the target resolution is portait OR
                // supplied image is in portait, while the target resolution is in landscape.
                // switch target resolution to match the image.
                if ((sourceWidth > sourceHeight && targetWidth < targetHeight) ||
                    (sourceWidth < sourceHeight && targetWidth > targetHeight))
                {
                    targetWidth = targetImageDimensions.Height;
                    targetHeight = targetImageDimensions.Width;
                }
            }

            float ratio = 0;
            float ratioWidth = ((float)targetWidth / (float)sourceWidth);
            float ratioHeight = ((float)targetHeight / (float)sourceHeight);

            if (ratioHeight < ratioWidth)
                ratio = ratioHeight;
            else
                ratio = ratioWidth;

            Bitmap newImage = null;

            switch (imageResizeMode)
            {
                case ImageResizeMode.Normal:
                default:
                    {
                        //int destWidth = (int)(sourceWidth * ratio);
                        //int destHeight = (int)(sourceHeight * ratio);

                        int destWidth = (int)(_newImage.Width * ratioHeight);
                        int destHeight = (int)(_newImage.Height * ratioWidth);

                        //int resizeWidth = 0;
                        //int resizeHeight = 0;

                        if (((float)targetHeight / (float)_newImage.Width < targetHeight / (float)_newImage.Height))
                        {
                            destWidth = targetWidth;
                        }
                        else
                        {
                            destHeight = targetHeight;
                        }

                        newImage = new Bitmap(destWidth, destHeight);
                        using (Graphics graphics = Graphics.FromImage(newImage))
                        {
                            BuildGraphics(graphics);

                            //graphics.DrawImage(_newImage, 0, 0, destWidth, destHeight);

                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetWrapMode(WrapMode.TileFlipXY);

                            graphics.DrawImage(_newImage,
                                new Rectangle(0, 0, destWidth, destHeight),
                                0, 0, _newImage.Width, _newImage.Height,
                                GraphicsUnit.Pixel,
                                attributes);
                        }
                        _newImage = newImage;

                        break;
                    }
                case ImageResizeMode.Crop:
                    {
                        int destWidth = (int)(_newImage.Width * ratioHeight);
                        int destHeight = (int)(_newImage.Height * ratioWidth);

                        if (((float)targetWidth / (float)_newImage.Width > targetHeight / (float)_newImage.Height))
                        {
                            destWidth = targetWidth;
                        }
                        else
                        {
                            destHeight = targetHeight;
                        }

                        newImage = new Bitmap(targetWidth, targetHeight);
                        using (Graphics graphics = Graphics.FromImage(newImage))
                        {
                            BuildGraphics(graphics);

                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetWrapMode(WrapMode.TileFlipXY);

                            graphics.DrawImage(_newImage,
                                new Rectangle((targetWidth - destWidth) / 2, (targetHeight - destHeight) / 2, destWidth, destHeight),
                                0, 0, _newImage.Width, _newImage.Height,
                                GraphicsUnit.Pixel,
                                attributes);
                        }

                        _newImage = newImage;

                        break;
                    }
                case ImageResizeMode.Stretch:
                    {
                        newImage = new Bitmap(targetWidth, targetHeight);
                        using (Graphics graphics = Graphics.FromImage(newImage))
                        {
                            BuildGraphics(graphics);

                            graphics.DrawImage(_newImage, 0, 0, targetWidth, targetHeight);
                        }
                        _newImage = newImage;
                        break;
                    }
                case ImageResizeMode.Fill:
                    {
                        int destWidth = (int)(sourceWidth * ratio);
                        int destHeight = (int)(sourceHeight * ratio);

                        int startX = 0;
                        int startY = 0;

                        if (destWidth < targetWidth)
                            startX = 0 + ((targetWidth - destWidth) / 2);

                        if (destHeight < targetHeight)
                            startY = 0 + ((targetHeight - destHeight) / 2);

                        newImage = new Bitmap(targetWidth, targetHeight);
                        using (Graphics graphics = Graphics.FromImage(newImage))
                        {
                            BuildGraphics(graphics);

                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetWrapMode(WrapMode.Clamp, Color.White);

                            graphics.FillRectangle(Brushes.White, 0, 0, targetWidth, targetHeight);

                            graphics.DrawImage(_newImage,
                                new Rectangle(startX, startY, destWidth, destHeight),
                                0, 0, _newImage.Width, _newImage.Height,
                                GraphicsUnit.Pixel,
                                attributes);
                        }
                        _newImage = newImage;

                        break;
                    }
                case ImageResizeMode.FillVertical:
                    {
                        int destWidth = (int)(sourceWidth * ratio);
                        int destHeight = (int)(sourceHeight * ratio);

                        if (targetWidth > destWidth)
                        {
                            targetWidth = destWidth;
                        }

                        int startX = 0;
                        int startY = 0;

                        if (destWidth < targetWidth)
                            startX = 0 + ((targetWidth - destWidth) / 2);

                        if (destHeight < targetHeight)
                            startY = 0 + ((targetHeight - destHeight) / 2);

                        newImage = new Bitmap(targetWidth, targetHeight);
                        using (Graphics graphics = Graphics.FromImage((Image)newImage))
                        {
                            BuildGraphics(graphics);

                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetWrapMode(WrapMode.Clamp, Color.White);

                            graphics.FillRectangle(Brushes.White, 0, 0, targetWidth, targetHeight);

                            graphics.DrawImage(_newImage,
                                new Rectangle(startX, startY, destWidth, destHeight),
                                0, 0, _newImage.Width, _newImage.Height,
                                GraphicsUnit.Pixel,
                                attributes);
                        }
                        _newImage = newImage;

                        break;
                    }
            }

            return this;
        }

        /// <summary>
        /// Gets the manipulated image's dimensions (taking into account any modificatiions made)
        /// </summary>
        public ImageService GetImageDimensions(out ImageDimensions imageDimensions)
        {
            imageDimensions = new ImageDimensions(_newImage.Width, _newImage.Height);

            return this;
        }

        public ImageService GetExifData(out IDictionary<string, object> exifData)
        {
            exifData = new Dictionary<string, object>();

            ExifReader reader = null;

            try
            {
                reader = new ExifReader(_imageStream);

                foreach (ushort tagID in Enum.GetValues(typeof(ExifTags)))
                {
                    object val;
                    if (reader.GetTagValue(tagID, out val))
                    {
                        exifData.Add(Enum.GetName(typeof(ExifTags), tagID), val);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error extracting EXIF data from image", ex);

                // Error occurred, EXIF may be in inconsistent state, clear it out
                exifData.Clear();
            }
            finally
            {
                _imageStream.Seek(0, SeekOrigin.Begin);

                //if (reader != null)
                //    reader.Cleanup();                
            }

            return this;
        }

        /// <summary>
        /// This method MUST be called when you're done using this utility. It will release all remaining
        /// references to the bitmap resource and allow the GC to do its business. The call to Dispose() is 
        /// done before the final de-reference, as per Microsoft's recommendation:
        /// http://msdn.microsoft.com/en-us/library/system.drawing.image.dispose.aspx
        /// </summary>
        public void Cleanup()
        {
            if (_sourceImage != null)
            {
                _sourceImage.Dispose();
                _sourceImage = null;
            }

            if (_newImage != null)
            {
                _newImage.Dispose();
                _newImage = null;
            }
        }

        private void BuildGraphics(Graphics graphics)
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().SingleOrDefault(e => e.MimeType == mimeType);
        }

        public ImageService Reset()
        {
            _imageStream.Seek(0, SeekOrigin.Begin);

            _newImage = _sourceImage;

            return this;
        }

        #endregion
    }
}