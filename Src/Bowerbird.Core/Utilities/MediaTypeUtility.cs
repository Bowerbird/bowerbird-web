/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.Utilities
{
    /// <summary>
    /// Information about the supported media types
    /// </summary>
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
                        { "taglib/mp3", Constants.AudioMimeTypes.Mp3 }, // Returned by the taglib library that is used to parse the audio file
                        { Constants.AudioMimeTypes.M4a, Constants.AudioMimeTypes.M4a },
                        { "audio/x-m4a", Constants.AudioMimeTypes.M4a },
                        { "taglib/m4a", Constants.AudioMimeTypes.M4a }, // Returned by the taglib library that is used to parse the audio file
                        { Constants.AudioMimeTypes.Wav, Constants.AudioMimeTypes.Wav },
                        { "audio/x-wav", Constants.AudioMimeTypes.Wav },
                        { "x-pn-wav", Constants.AudioMimeTypes.Wav },
                        { "taglib/wav", Constants.AudioMimeTypes.Wav } // Returned by the taglib library that is used to parse the audio file
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
                        { Constants.ImageMimeTypes.Tiff, "tif" },
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

        //public static MediaTypeInfo GetMediaTypeInfoForFile(Stream stream)
        //{
        //    var mimeType = GetStandardMimeTypeForFile(stream);
        //    return SupportedMediaTypeInfo()
        //        .Single(x => x.ContainsMimeType(mimeType));
        //}

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

        ///// <summary>
        ///// Gets the standard mimetype for a given image file.
        ///// </summary>
        //public static string GetStandardMimeTypeForImageFile(Stream stream)
        //{
        //    return GetStandardMimeTypeForMimeType(ImageUtility.Load(stream).GetMimeType());
        //}

        ///// <summary>
        ///// Gets the standard mimetype for a given audio file. The provided filename and mimetypes are used by the audio
        ///// detector to assist in detection.
        ///// </summary>
        //public static string GetStandardMimeTypeForAudioFile(Stream stream, string filename, string mimeType)
        //{
        //    return GetStandardMimeTypeForMimeType(AudioUtility.Load(stream, filename, mimeType).GetMimeType());
        //}

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
