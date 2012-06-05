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
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Events
{
    public abstract class DomainEventBase : IDomainEvent
    {
        #region Members

        #endregion

        #region Constructors

        protected DomainEventBase(
            User user, 
            DomainModel sender)
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(sender, "sender");

            User = user;
            Sender = sender;
        }

        #endregion

        #region Properties

        public User User { get; private set; }

        public DomainModel Sender { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}