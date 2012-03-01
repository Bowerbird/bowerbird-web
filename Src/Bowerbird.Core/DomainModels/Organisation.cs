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
using Bowerbird.Core.Extensions;

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

            var message = createdByUser.GetName().AppendWith(" created organisation ").AppendWith(name);

            EventProcessor.Raise(new DomainModelCreatedEvent<Organisation>(this, createdByUser, message));
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

            var message = updatedByUser.GetName().AppendWith(" updated organisation ").AppendWith(name);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser, message));

            return this;
        }

        #endregion
    }
}