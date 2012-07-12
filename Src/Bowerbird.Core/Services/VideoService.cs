/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.VideoUtilities;
using Raven.Client;

namespace Bowerbird.Core.Services
{
    public class VideoService : IVideoService
    {
        #region Fields

        private readonly IVideoUtility _videoUtility;

        #endregion

        #region Constructors

        public VideoService(
            IVideoUtility videoUtility
            )
        {
            Check.RequireNotNull(videoUtility, "videoUtility");
            
            _videoUtility = videoUtility;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Save(MediaResourceCreateCommand command, MediaResource mediaResource)
        {
            string provider; // playback service - youtube, vimeo et al
            string videoId; // unique identifier for video on playback service
            string embedString; // the embed html tags with format options for video id and sizes

            if (_videoUtility.IsValidVideo(command.LinkUri, out embedString, out videoId, out provider))
            {
                mediaResource
                    .AddMetadata("Url", command.LinkUri)
                    .AddMetadata("Provider", provider)
                    .AddMetadata("VideoId", videoId);

                MakeVideoMediaResourceFiles(
                    mediaResource,
                    embedString.AppendWith(videoId),
                    command.LinkUri,
                    provider,
                    videoId);
            }
        }

        private void MakeVideoMediaResourceFiles(MediaResource mediaResource, string embedScript, string linkUri, string provider, string videoId)
        {
            mediaResource.AddVideoFile("Fullsize", linkUri, embedScript, provider, videoId, "672", "400");
            mediaResource.AddVideoFile("Preview", linkUri, embedScript, provider, videoId, "220", "200");
            mediaResource.AddVideoFile("Small", linkUri, embedScript, provider, videoId, "120", "80");
            mediaResource.AddVideoFile("Thumb", linkUri, embedScript, provider, videoId, "60", "40");
        }

        #endregion
    }
}