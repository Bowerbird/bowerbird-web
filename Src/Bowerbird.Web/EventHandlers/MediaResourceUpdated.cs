/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Passes a model directly back to a particular client. This is used when media resources
    /// are uploaded in a disconnected, asynch process. Allows model to upload independently of
    /// web request and return back to same client.
    /// </summary>
    public class MediaResourceUpdated : 
        IEventHandler<DomainModelCreatedEvent<MediaResource>>,
        IEventHandler<MediaResourceCreateFailedEvent>
    {
        #region Members

        private readonly IBackChannelService _backChannelService;

        #endregion

        #region Constructors

        public MediaResourceUpdated(
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

        public void Handle(DomainModelCreatedEvent<MediaResource> domainEvent)
        {
            _backChannelService.NotifyMediaResourceUploadSuccessToUserChannel(domainEvent.User.Id, domainEvent.DomainModel);
        }

        public void Handle(MediaResourceCreateFailedEvent domainEvent)
        {
            _backChannelService.NotifyMediaResourceUploadFailureToUserChannel(domainEvent.User.Id, domainEvent.Key, domainEvent.Reason);
        }

        #endregion      
    }
}