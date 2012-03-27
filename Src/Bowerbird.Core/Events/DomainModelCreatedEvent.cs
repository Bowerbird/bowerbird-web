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
    public class DomainModelCreatedEvent<T> : IDomainEvent
    {
        #region Members

        #endregion

        #region Constructors

        public DomainModelCreatedEvent(
            T domainModel,
            User createdByUser)
        {
            Check.RequireNotNull(domainModel, "domainModel");
            Check.RequireNotNull(createdByUser, "createdByUser");
            
            DomainModel = domainModel;
            CreatedByUser = createdByUser;
        }

        #endregion

        #region Properties

        public T DomainModel { get; private set; }

        public User CreatedByUser { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}