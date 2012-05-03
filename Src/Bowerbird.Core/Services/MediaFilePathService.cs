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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Services
{
    public class MediaFilePathService : IMediaFilePathService
    {
        #region Members

        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public MediaFilePathService(
            IConfigService configService)
        {
            Check.Require(configService != null, "configService may not be null");

            _configService = configService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string MakeMediaFileUri(MediaResource mediaResource, string storedRepresentation)
        {
            return MakeMediaFileUri(
                mediaResource.Id,
                mediaResource.Type,
                storedRepresentation,
                mediaResource.Metadata["format"]);
        }

        public string MakeMediaFileUri(string mediaResourceId, string mediaType, string storedRepresentation, string extension)
        {
            return string.Format(
                "{0}/{1}/{2}/{3}-{4}.{5}", 
                GetCleanMediaRootUri(),
                mediaType,
                GetDirectoryName(RecordIdPart(mediaResourceId)),
                RecordIdPart(mediaResourceId),
                storedRepresentation,
                extension);
        }

        public string MakeMediaBasePath(int recordId, string mediaType)
        {
            var environmentRootPath = _configService.GetEnvironmentRootPath();
            var mediaRelativePath = _configService.GetMediaRelativePath();

            var path = string.Format("{0}{1}{2}\\{3}",
                environmentRootPath,
                mediaRelativePath,
                mediaType,
                GetDirectoryName(recordId));

            return path;
        }

        public string MakeMediaFilePath(string recordId, string mediaType, string storedRepresentation, string extension)
        {
            string mediaPath = MakeMediaBasePath(RecordIdPart(recordId), mediaType);
            string filename = string.Format("{0}-{1}.{2}", RecordIdPart(recordId), storedRepresentation, extension);

            return Path.Combine(mediaPath, filename);
        }

        private static int GetDirectoryName(int mediaResourceId)
        {
            int x = Round(mediaResourceId, 3);

            if(x == 0)
            {
                return 0;
            }

            return x/1000;
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
            string mediaRootUri = _configService.GetMediaRootUri();

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