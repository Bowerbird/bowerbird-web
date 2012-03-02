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

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Organisation : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected Organisation()
            : base()
        {
        }

        public Organisation(
            User createdByUser,
            string name,
            string description,
            string website,
            MediaResource avatar)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                avatar);

            var eventMessage = string.Format(
                ActivityMessages.CreatedAGroup,
                createdByUser.GetName(),
                GroupType(),
                Name
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<Organisation>(this, createdByUser, eventMessage));
        }

        #endregion
         
        #region Properties

        #endregion

        #region Methods


        public override string GroupType()
        {
            return "Organisation";
        }

        public Organisation UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                avatar);

            var eventMessage = string.Format(
                ActivityMessages.UpdatedAGroup,
                updatedByUser.GetName(),
                Name,
                GroupType()
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser, eventMessage));

            return this;
        }

        #endregion
    }
}