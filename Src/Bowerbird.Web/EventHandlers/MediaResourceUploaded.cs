/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.VideoUtilities;
using Bowerbird.Core.EventHandlers;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Passes a model directly back to a particular client. This is used when media resources
    /// are uploaded in a disconnected, asynch process. Allows model to upload independently of
    /// web request and return back to same client.
    /// </summary>
    public class MediaResourceUploaded : IEventHandler<MediaResourceUploadedEvent>
    {
        #region Members

        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public MediaResourceUploaded(
            IBackChannelService backChannelService
            )
        {
            Check.RequireNotNull(backChannelService, "backChannelService");

            _backChannelService = backChannelService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MediaResourceUploadedEvent domainEvent)
        {
            // this somehow needs to extract the mediaResource properties from the event. Cast as dynamic perhaps?
            var mediaResourceSender = ((dynamic) domainEvent.Sender);

            var mediaResource = new
            {
                mediaResourceSender.Id,
                mediaResourceSender.Metadata,
                mediaResourceSender.MediaType,
                mediaResourceSender.Key,
                mediaResourceSender.Image,
                mediaResourceSender.UploadedOn,
                mediaResourceSender.User,
                mediaResourceSender.Video
            };

            _backChannelService.SendUploadedMediaResourceToUserChannel(domainEvent.User.Id, mediaResource);
        }

        #endregion      
    }
}