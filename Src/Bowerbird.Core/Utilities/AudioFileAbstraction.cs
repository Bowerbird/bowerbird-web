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
    public class AudioFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly Stream _stream;
        private readonly string _fileName;

        public AudioFileAbstraction(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
        }

        public string Name
        {
            get { return _fileName; }
        }

        public Stream ReadStream
        {
            get { return _stream; }
        }

        public Stream WriteStream
        {
            get { throw new NotImplementedException(); }
        }

        public void CloseStream(Stream stream)
        {
        }

    }
}
