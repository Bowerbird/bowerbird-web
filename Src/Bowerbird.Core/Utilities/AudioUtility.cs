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
using System.IO;

namespace Bowerbird.Core.Utilities
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
            //_audioFile = TagLib.File.Create(new AudioFileAbstraction(stream, originalFileName), mimeType, TagLib.ReadStyle.None);
            _audioFile = TagLib.File.Create(new AudioFileAbstraction(stream, originalFileName));
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static AudioUtility Load(Stream stream, string originalFileName, string mimeType)
        {
            return new AudioUtility(stream, originalFileName, mimeType);
        }

        public static bool TryLoad(Stream stream, string originalFileName, string mimeType, out AudioUtility audioUtility)
        {
            try
            {
                audioUtility = new AudioUtility(stream, originalFileName, mimeType);
                return true;
            }
            catch (Exception)
            {
            }

            audioUtility = null;
            return false;
        }

        public bool IsValidAudioFile()
        {
            return !_audioFile.PossiblyCorrupt;
        }

        /// <summary>
        /// Gets the mime type that taglib has best guessed
        /// </summary>
        public string GetMimeType()
        {
            return _audioFile.MimeType;
        }

        public string GetTitleTag()
        {
            return _audioFile.Tag.Title;
        }

        public string GetCopyrightTag()
        {
            return _audioFile.Tag.Copyright;
        }

        public int GetDuration()
        {
            return (int)_audioFile.Properties.Duration.TotalSeconds;
        }

        public string GetCommentTag()
        {
            return _audioFile.Tag.Comment;
        }

        public AudioUtility SaveAs(string filePath)
        {
            using (var fileStream = File.Create(filePath))
            {
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.CopyTo(fileStream);
                _stream.Seek(0, SeekOrigin.Begin);
            }

            return this;
        }

        #endregion
    }
}
