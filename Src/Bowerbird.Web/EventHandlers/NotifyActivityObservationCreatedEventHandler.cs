/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
				
namespace Bowerbird.Web.EventHandlers
{
    public class NotifyActivityObservationCreatedEventHandler : NotifyActivityEventHandlerBase, IEventHandler<DomainModelCreatedEvent<Observation>>
    {
        #region Members

        #endregion

        #region Constructors

        public NotifyActivityObservationCreatedEventHandler(
            IUserContext userContext)
            : base(userContext)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> observationCreatedEvent)
        {
            Check.RequireNotNull(observationCreatedEvent, "observationCreatedEvent");

            // TODO: Find all users that need to be notified of this new observation

            Notify(
                "observationcreated",
                observationCreatedEvent.CreatedByUser,
                observationCreatedEvent.DomainModel);
        }

        #endregion
    }
}