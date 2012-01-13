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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels.Posts
{
    #region Namespaces

    

    #endregion

    public class TeamPost : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected TeamPost() : base()
        {
        }

        public TeamPost(
            User createdByUser
            //,DateTime timestamp
            )
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");

            //PostedOn = timestamp;
            User = createdByUser;

            SetDetails(
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<TeamPost>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        #endregion

        #region Methods

        private void SetDetails()
        {

        }

        public TeamPost UpdateDetails(
            User updatedByUser
            )
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<TeamPost>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}