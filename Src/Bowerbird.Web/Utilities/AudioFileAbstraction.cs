using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bowerbird.Web.Utilities
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
