using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.Config
{
    public class MediaTypeUtility
    {
        #region Members

        #endregion

        #region Constructors

        private MediaTypeUtility()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static IEnumerable<MediaTypeInfo> SupportedMediaTypeInfo()
        {
            return new[] 
            {
                // Audio
                new MediaTypeInfo(
                    "audio", 
                    new Dictionary<string, string>
                    {
                        { Constants.AudioMimeTypes.Mp3, "mp3" },
                        { Constants.AudioMimeTypes.M4a, "m4a" },
                        { Constants.AudioMimeTypes.Wav, "wav" }
                    }, 
                    new Dictionary<string, string>
                    {
                        { Constants.AudioMimeTypes.Mp3, Constants.AudioMimeTypes.Mp3 },
                        { "audio/x-mpeg", Constants.AudioMimeTypes.Mp3 },
                        { "audio/mp3", Constants.AudioMimeTypes.Mp3 },
                        { "audio/x-mp3", Constants.AudioMimeTypes.Mp3 },
                        { "audio/mpeg3", Constants.AudioMimeTypes.Mp3 },
                        { "audio/x-mpeg3", Constants.AudioMimeTypes.Mp3 },
                        { "audio/x-mpg", Constants.AudioMimeTypes.Mp3 },
                        { "audio/x-mpegaudio", Constants.AudioMimeTypes.Mp3 },
                        { "audio/mp4", Constants.AudioMimeTypes.M4a },
                        { Constants.AudioMimeTypes.M4a, Constants.AudioMimeTypes.M4a },
                        { "audio/x-m4a", Constants.AudioMimeTypes.M4a },
                        { Constants.AudioMimeTypes.Wav, Constants.AudioMimeTypes.Wav },
                        { "audio/x-wav", Constants.AudioMimeTypes.Wav },
                        { "x-pn-wav", Constants.AudioMimeTypes.Wav }
                    },
                    new Dictionary<string, string>
                    {
                        { "mp3", Constants.AudioMimeTypes.Mp3 },
                        { "mp4", Constants.AudioMimeTypes.M4a },
                        { "m4a", Constants.AudioMimeTypes.M4a },
                        { "wav", Constants.AudioMimeTypes.Wav }
                    }),


                // Images
                new MediaTypeInfo(
                    "image",
                    new Dictionary<string, string>
                    {
                        { Constants.ImageMimeTypes.Jpeg, "jpg" },
                        { Constants.ImageMimeTypes.Tiff, "tiff" },
                        { Constants.ImageMimeTypes.Png, "png" },
                        { Constants.ImageMimeTypes.Bmp, "bmp" },
                        { Constants.ImageMimeTypes.Gif, "gif" }
                    },
                    new Dictionary<string, string>
                    {   
                        { Constants.ImageMimeTypes.Jpeg, Constants.ImageMimeTypes.Jpeg },
                        { "image/jpg", Constants.ImageMimeTypes.Jpeg }, 
                        { "image/jp_", Constants.ImageMimeTypes.Jpeg }, 
                        { "application/jpg", Constants.ImageMimeTypes.Jpeg }, 
                        { "application/x-jpg", Constants.ImageMimeTypes.Jpeg }, 
                        { "image/pjpeg", Constants.ImageMimeTypes.Jpeg }, 
                        { "image/pipeg", Constants.ImageMimeTypes.Jpeg }, 
                        { "image/vnd.swiftview-jpeg", Constants.ImageMimeTypes.Jpeg }, 
                        { Constants.ImageMimeTypes.Png, Constants.ImageMimeTypes.Png },
                        { "application/png", Constants.ImageMimeTypes.Png },
                        { "application/x-png", Constants.ImageMimeTypes.Png },
                        { Constants.ImageMimeTypes.Tiff, Constants.ImageMimeTypes.Tiff },
                        { "image/tif", Constants.ImageMimeTypes.Tiff },
                        { "image/x-tif", Constants.ImageMimeTypes.Tiff },
                        { "image/x-tiff", Constants.ImageMimeTypes.Tiff },
                        { "application/tif", Constants.ImageMimeTypes.Tiff },
                        { "application/x-tif", Constants.ImageMimeTypes.Tiff },
                        { "application/tiff", Constants.ImageMimeTypes.Tiff },
                        { "application/x-tiff", Constants.ImageMimeTypes.Tiff },
                        { Constants.ImageMimeTypes.Bmp, Constants.ImageMimeTypes.Bmp },
                        { "image/x-bmp", Constants.ImageMimeTypes.Bmp },
                        { "image/x-bitmap", Constants.ImageMimeTypes.Bmp },
                        { "image/x-xbitmap", Constants.ImageMimeTypes.Bmp },
                        { "image/x-win-bitmap", Constants.ImageMimeTypes.Bmp },
                        { "image/x-windows-bmp", Constants.ImageMimeTypes.Bmp },
                        { "image/ms-bmp", Constants.ImageMimeTypes.Bmp },
                        { "image/x-ms-bmp", Constants.ImageMimeTypes.Bmp },
                        { "application/bmp", Constants.ImageMimeTypes.Bmp },
                        { "application/x-bmp", Constants.ImageMimeTypes.Bmp },
                        { Constants.ImageMimeTypes.Gif, Constants.ImageMimeTypes.Gif },
                        { "image/gi_", Constants.ImageMimeTypes.Gif },
                    },
                    new Dictionary<string, string>
                    {
                        { "jpg", Constants.ImageMimeTypes.Jpeg },
                        { "jpeg", Constants.ImageMimeTypes.Jpeg },
                        { "tiff", Constants.ImageMimeTypes.Tiff },
                        { "tif", Constants.ImageMimeTypes.Tiff },
                        { "png", Constants.ImageMimeTypes.Png },
                        { "bmp", Constants.ImageMimeTypes.Bmp },
                        { "gif", Constants.ImageMimeTypes.Gif }
                    })
                
            };
        }

        public static bool IsSupportedMimeType(string mimeType)
        {
            return SupportedMediaTypeInfo()
                .Any(x => x.ContainsMimeType(mimeType));
        }

        public static bool IsSupportedExtension(string fileExtension)
        {
            return SupportedMediaTypeInfo()
                .Any(x => x.ContainsFileExtension(fileExtension));
        }

        public static bool IsSupportedFile(Stream stream)
        {
            var mimeType = GetImageMimeTypeForStream(stream) ?? GetAudioMimeTypeForStream(stream);
            return !string.IsNullOrWhiteSpace(mimeType);
        }

        public static MediaTypeInfo GetMediaTypeInfoForMediaType(string mediaType)
        {
            return SupportedMediaTypeInfo()
                .Single(x => x.MediaType == mediaType);
        }

        public static MediaTypeInfo GetMediaTypeInfoForMimeType(string mimeType)
        {
            return SupportedMediaTypeInfo()
                .Single(x => x.ContainsMimeType(mimeType));
        }

        public static MediaTypeInfo GetMediaTypeInfoForExtension(string fileExtension)
        {
            return SupportedMediaTypeInfo()
                .Single(x => x.ContainsFileExtension(fileExtension));
        }

        public static MediaTypeInfo GetMediaTypeInfoForFile(Stream stream)
        {
            var mimeType = GetStandardMimeTypeForFile(stream);
            return SupportedMediaTypeInfo()
                .Single(x => x.ContainsMimeType(mimeType));
        }

        /// <summary>
        /// Gets the standard file extension for the specified mimetype  (which may be a variant of a valid file extension)
        /// </summary>
        public static string GetStandardExtensionForMimeType(string mimeType)
        {
            return SupportedMediaTypeInfo()
                .Single(x => x.ContainsMimeType(mimeType))
                .GetStandardExtensionForMimeType(mimeType);
        }

        /// <summary>
        /// Gets the standard mimetype for a given mimetype (which may be a variant of a valid mimetype)
        /// </summary>
        public static string GetStandardMimeTypeForMimeType(string mimeType)
        {
            return SupportedMediaTypeInfo()
                .Single(x => x.ContainsMimeType(mimeType))
                .GetStandardMimeTypeForMimeType(mimeType);
        }

        /// <summary>
        /// Gets the standard mimetype for a given stream
        /// </summary>
        public static string GetStandardMimeTypeForFile(Stream stream)
        {
            var mimeType = GetImageMimeTypeForStream(stream) ?? GetAudioMimeTypeForStream(stream);
            return GetStandardMimeTypeForMimeType(mimeType);
        }

        /// <summary>
        /// Determines image mimetype. Adapted from: http://stackoverflow.com/questions/210650/validate-image-from-file-in-c-sharp
        /// </summary>
        private static string GetImageMimeTypeForStream(Stream stream)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");        // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");       // GIF
            var png = new byte[] { 137, 80, 78, 71 };       // PNG
            var tiff = new byte[] { 73, 73, 42 };           // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };          // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 };   // JPEG
            var jpeg2 = new byte[] { 255, 216, 255, 225 };  // JPEG Canon

            stream.Seek(0, SeekOrigin.Begin);
            var reader = new BinaryReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = reader.ReadBytes(10);

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return Constants.ImageMimeTypes.Bmp;

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return Constants.ImageMimeTypes.Gif;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return Constants.ImageMimeTypes.Png;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)) || tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return Constants.ImageMimeTypes.Tiff;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)) || jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return Constants.ImageMimeTypes.Jpeg;

            return null;
        }

        /// <summary>
        /// Determines audio mimetype
        /// </summary>
        private static string GetAudioMimeTypeForStream(Stream stream)
        {
            return null;
        }

        #endregion

        public class MediaTypeInfo
        {
            public MediaTypeInfo(
                string mediaType,
                Dictionary<string, string> mimeTypes,
                IDictionary<string, string> mimeTypeStandardMap,
                IDictionary<string, string> mimeTypeExtensionMap)
            {
                MediaType = mediaType;
                MimeTypes = mimeTypes;
                MimeTypeStandardMap = mimeTypeStandardMap;
                MimeTypeExtensionMap = mimeTypeExtensionMap;
            }

            public string MediaType { get; private set; }

            /// <summary>
            /// The supported standard mimetypes and their corresponding file extensions
            /// </summary>
            public IDictionary<string, string> MimeTypes { get; private set; }

            /// <summary>
            /// A map of standard mimetypes to non-standard types
            /// </summary>
            public IDictionary<string, string> MimeTypeStandardMap { get; private set; }

            /// <summary>
            /// A map of standard mimetypes to possible filenames
            /// </summary>
            private IDictionary<string, string> MimeTypeExtensionMap { get; set; }

            /// <summary>
            /// Is the mimetype recognised
            /// </summary>
            public bool ContainsMimeType(string mimeType)
            {
                return MimeTypeStandardMap.ContainsKey(mimeType.ToLower());
            }

            /// <summary>
            /// Is the file extension recognised
            /// </summary>
            public bool ContainsFileExtension(string fileExtension)
            {
                return MimeTypeExtensionMap.ContainsKey(fileExtension.ToLower());
            }

            /// <summary>
            /// Gets the standard file extension for the specified mimetype
            /// </summary>
            public string GetStandardExtensionForMimeType(string mimeType)
            {
                var standardMimeType = MimeTypeStandardMap[mimeType.ToLower()];
                return MimeTypes[standardMimeType];
            }

            /// <summary>
            /// Gets the standard mimetype for a given mimetype (which may be a variant of a valid mimetype)
            /// </summary>
            public string GetStandardMimeTypeForMimeType(string mimeType)
            {
                return MimeTypeStandardMap[mimeType.ToLower()];
            }

        }
    }
}
