using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Utilities
{
    public class AudioUtility
    {
        #region Fields

        private Stream _stream;
        private TagLib.File _audioFile;

        #endregion

        #region Constructors

        private AudioUtility(Stream stream, string originalFileName, string mimeType)
        {
            _stream = stream;
            _audioFile = TagLib.File.Create(new AudioFileAbstraction(stream, originalFileName), mimeType, TagLib.ReadStyle.None);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static AudioUtility Load(Stream stream, string originalFileName, string mimeType)
        {
            return new AudioUtility(stream, originalFileName, mimeType);
        }

        public bool IsValidAudioFile()
        {
            return !_audioFile.PossiblyCorrupt && MediaTypeUtility.IsSupportedMimeType(_audioFile.MimeType);
        }

        public string GetFileExtension()
        {
            return MediaTypeUtility.GetMediaTypeInfoForMimeType(_audioFile.MimeType).GetStandardExtensionForMimeType(_audioFile.MimeType);
        }

        public string GetMimeType()
        {
            return _audioFile.MimeType;
        }

        public string GetTitleTagValue()
        {
            return _audioFile.Tag.Title;
        }

        public string GetCopyrightTagValue()
        {
            return _audioFile.Tag.Copyright;
        }

        public string GetCommentTagValue()
        {
            return _audioFile.Tag.Comment;
        }

        public AudioUtility Save(string filePath)
        {
            using (var fileStream = File.Create(filePath))
            {
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.CopyTo(fileStream);
            }

            return this;
        }

        #endregion
    }
}
