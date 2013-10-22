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
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModelFactories;
using NLog;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Bowerbird.Core.Utilities
{
    public class ImageUtility
    {

        #region Fields

        private Logger _logger = LogManager.GetLogger("ImageUtility");

        private Bitmap _sourceImage;
        private Bitmap _newImage;
        private Stream _imageStream;

        #endregion  

        #region Constructors

        /// <summary>
        /// Private constructor to avoid conflicts with chaining initialisation
        /// </summary>
        private ImageUtility(Bitmap image, Stream imageStream)
        {
            _sourceImage = image;
            _newImage = image;
            _imageStream = imageStream;
        }

        #endregion

        #region Methods

        public static ImageUtility Load(Stream imageStream)
        {
            Bitmap image = null;

            imageStream.Seek(0, SeekOrigin.Begin);

            image = Image.FromStream(imageStream) as Bitmap;

            imageStream.Seek(0, SeekOrigin.Begin);

            return new ImageUtility(image, imageStream);
        }

        public static bool TryLoad(Stream imageStream, out ImageUtility imageUtility)
        {
            try
            {
                imageUtility = Load(imageStream);
                return true;
            }
            catch
            {
            }

            imageUtility = null;
            return false;
        }

        public ImageUtility SaveAs(string imageMimeType, string filePath)
        {
            try
            {
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

                _newImage.Save(filePath, GetEncoderInfo(imageMimeType), encoderParams);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Error saving image to file", exception);

                if (_newImage != null)
                {
                    _newImage.Dispose();
                    _newImage = null;
                }

                throw exception;
            }

            return this;
        }

        public ImageUtility Save(MediaResource mediaResource, List<ImageCreationTask> imageCreationTasks, IMediaFilePathFactory mediaFilePathFactory)
        {
            foreach (var imageCreationTask in imageCreationTasks)
            {
                dynamic imageFile = imageCreationTask.File;

                var basePath = mediaFilePathFactory.MakeMediaBasePath(mediaResource.Id);

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                var fullPath = mediaFilePathFactory.MakeMediaFilePath(mediaResource.Id, imageCreationTask.StoredRepresentation, MediaTypeUtility.GetStandardExtensionForMimeType(imageCreationTask.MimeType));

                Reset();

                if (!imageCreationTask.DoImageManipulation())
                {
                    SaveAs(imageCreationTask.MimeType, fullPath);
                }
                else
                {
                    Resize(new ImageDimensions(imageFile.Width, imageFile.Height), imageCreationTask.DetermineBestOrientation.Value, imageCreationTask.ImageResizeMode.Value);
                    SaveAs(imageCreationTask.MimeType, fullPath);
                }
            }

            return this;
        }

        public ImageUtility Resize(ImageDimensions targetImageDimensions, bool determineBestOrientation, ImageResizeMode imageResizeMode)
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

                            //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            //graphics.CompositingQuality = CompositingQuality.HighQuality;

                            graphics.DrawImage(_newImage,
                                new Rectangle((targetWidth - destWidth) / 2, (targetHeight - destHeight) / 2, destWidth, destHeight),
                                0, 0, _newImage.Width, _newImage.Height,
                                GraphicsUnit.Pixel,
                                attributes);
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
            }

            return this;
        }

        /// <summary>
        /// Gets image mimetype. Adapted from: http://stackoverflow.com/questions/210650/validate-image-from-file-in-c-sharp
        /// </summary>
        public string GetMimeType()
        {
            var bmp = Encoding.ASCII.GetBytes("BM");    // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");   // GIF
            var png = new byte[] { 137, 80, 78, 71 };   // PNG
            var tiff = new byte[] { 73, 73, 42, 0 };    // TIFF big endian
            var tiff2 = new byte[] { 77, 77, 0, 42 };   // TIFF little endian
            var jpeg = new byte[] { 255, 216, 255 };    // JPEG

            _imageStream.Seek(0, SeekOrigin.Begin);
            var reader = new BinaryReader(_imageStream);
            byte[] bytes = reader.ReadBytes(10);
            _imageStream.Seek(0, SeekOrigin.Begin);

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return Constants.ImageMimeTypes.Bmp;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return Constants.ImageMimeTypes.Gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return Constants.ImageMimeTypes.Png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)) || tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return Constants.ImageMimeTypes.Tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return Constants.ImageMimeTypes.Jpeg;

            return null;
        }

        /// <summary>
        /// Gets the manipulated image's dimensions (taking into account any modificatiions made)
        /// </summary>
        public ImageDimensions GetDimensions()
        {
            return ImageDimensions.MakeRectangle(_newImage.Width, _newImage.Height);
        }

        /// <summary>
        /// Attempt to get the EXIF data
        /// </summary>
        public IDictionary<string, object> GetExifData()
        {
            IDictionary<string, object> exifData = new Dictionary<string, object>();

            ExifReader reader = null;

            try
            {
                reader = new ExifReader(_imageStream);

                foreach (ushort tagID in Enum.GetValues(typeof (ExifTags)))
                {
                    object val;
                    if (reader.GetTagValue(tagID, out val))
                    {
                        // if val can be cast to double or float and the value is "NaN", then replace it with ""
                        if (val is double || val is float)
                        {
                            var possibleDoubleVal = val as double? ?? (double?)(val as float?);
                            if (possibleDoubleVal == null || Double.IsNaN((double)possibleDoubleVal) ||
                                Double.IsInfinity((double)possibleDoubleVal))
                            {
                                val = string.Empty;
                            }
                        }

                        exifData.Add(Enum.GetName(typeof (ExifTags), tagID), val);
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
            }

            return exifData;
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
            //graphics.CompositingMode = CompositingMode.SourceCopy;
            //graphics.CompositingQuality = CompositingQuality.HighQuality;
            //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;


            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality; 
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().SingleOrDefault(e => e.MimeType == mimeType);
        }

        public ImageUtility Reset()
        {
            _imageStream.Seek(0, SeekOrigin.Begin);

            _newImage = _sourceImage;

            return this;
        }

        #endregion

    }
}
