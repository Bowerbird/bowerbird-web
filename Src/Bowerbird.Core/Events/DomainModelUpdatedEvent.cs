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
    public class DomainModelUpdatedEvent<T> : IDomainEvent
    {

        #region Members

        #endregion

        #region Constructors

        public DomainModelUpdatedEvent(
            T domainModel,
            User user)
        {
            Check.RequireNotNull(domainModel, "domainModel");
            Check.RequireNotNull(user, "user");

            DomainModel = domainModel;
            User = user;
        }

        #endregion

        #region Properties

        public T DomainModel { get; private set; }

        public User User { get; private set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}