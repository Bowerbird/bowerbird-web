using System;
using System.IO;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.DomainModelFactories
{
    public class MediaFilePathFactory : IMediaFilePathFactory
    {
        #region Members

        private readonly IConfigSettings _configSettings;

        #endregion

        #region Constructors

        public MediaFilePathFactory(
            IConfigSettings configService)
        {
            Check.Require(configService != null, "configService may not be null");

            _configSettings = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods
        
        public string MakeMediaUri(string uri)
        {
            // don't alter relative application paths
            if (uri.StartsWith("/img/")) return uri;

            // legacy path from pre-aws hosting
            if (uri.StartsWith("/media/")) uri = uri.Replace("/media", "");

            return string.Format(
                "{0}/{1}",
                GetCleanMediaRootUri(),
                uri
            );
        }

        public string MakeRelativeMediaFileUri(string mediaResourceId, string storedRepresentation, string extension)
        {
            return string.Format(
                "{0}/{1}/{2}", 
                GetCleanMediaRootUri(),
                GetDirectoryName(mediaResourceId),
                MakeMediaFileName(mediaResourceId, storedRepresentation, extension));
        }

        public string MakeMediaFileName(string mediaResourceId, string storedRepresentation, string extension)
        {
            return string.Format(
                "{0}-{1}.{2}",
                RecordIdPart(mediaResourceId),
                storedRepresentation,
                extension);
        }

        // There is some weirdness in Path.Combine, in that if a path contains an absolute path, only that path is returned..
        // http://stackoverflow.com/questions/53102/why-does-path-combine-not-properly-concatenate-filenames-that-start-with-path-di
        public string MakeMediaBasePath(string mediaResourceId)
        {
            var relativePath = Path.Combine(
                _configSettings.GetMediaRelativePath(),
                GetDirectoryName(mediaResourceId));

            return string.Format(
                "{0}{1}", 
                _configSettings.GetEnvironmentRootPath(), 
                relativePath);
        }

        public string MakeMediaFilePath(string mediaResourceId, string storedRepresentation, string extension)
        {
            string mediaPath = MakeMediaBasePath(mediaResourceId);
            string filename = MakeMediaFileName(mediaResourceId, storedRepresentation, extension);

            return Path.Combine(mediaPath, filename);
        }

        private static string GetDirectoryName(string mediaResourceId)
        {
            int x = Round(RecordIdPart(mediaResourceId), 3);

            if(x == 0)
            {
                return 0.ToString();
            }

            return (x/1000).ToString();
        }

        /// <summary>
        /// Round the id to the nearest 1000 ans store blocks of 1000 images in directories. This is to 
        /// ensure we don't exceed any OS limitations/performance thresholds.
        /// </summary>
        private static int Round(int number, int place)
        {
            int roundTo = 1;
            if (place <= 0)
                return (int)number;
            while (place > 0)
            {
                roundTo = roundTo * 10;
                place--;
            }
            return ((number + (roundTo / 2)) / roundTo) * roundTo;
        }

        private string GetCleanMediaRootUri()
        {
            string mediaRootUri = _configSettings.GetMediaRootUri();

            if (mediaRootUri.EndsWith("/"))
            {
                mediaRootUri = mediaRootUri.Remove(mediaRootUri.Length - 1);
            }

            return mediaRootUri;
        }

        private static int RecordIdPart(string recordId)
        {
            int recordIdInt;

            if (Int32.TryParse(recordId.Split('/')[1], out recordIdInt))
                return recordIdInt;

            throw new ApplicationException("Record doesn't have a valid id");
        }

        #endregion


    }
}