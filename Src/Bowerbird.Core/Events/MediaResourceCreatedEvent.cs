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

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Events
{
    public class MediaResourceCreatedEvent
    {
        #region Members

        #endregion

        #region Constructors

        public MediaResourceCreatedEvent(
            MediaResource domainModel,
            User createdByUser,
            MediaResource sender)
        {
            Check.RequireNotNull(domainModel, "domainModel");
            Check.RequireNotNull(sender, "sender");
            Check.RequireNotNull(createdByUser, "createdByUser");
            
            DomainModel = domainModel;
            User = createdByUser;
            Sender = sender;
        }

        #endregion

        #region Properties

        public MediaResource DomainModel { get; set; }

        public User User { get; private set; }

        public MediaResource Sender { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}