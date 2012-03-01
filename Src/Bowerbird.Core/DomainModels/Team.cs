/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
    public class Team : Group
    {

        #region Members

        #endregion

        #region Constructors

        protected Team() : base() {}

        public Team(
            User createdByUser,
            string name,
            string description,
            string website,
            MediaResource avatar,
            string organisationId = null)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                avatar,
                organisationId);

            var message = createdByUser.GetName()
                .AppendWith(" created the ")
                .AppendWith(Name).AppendWith(" team");

            EventProcessor.Raise(new DomainModelCreatedEvent<Team>(this, createdByUser, message));
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override string GroupType()
        {
            return "Team";
        }

        public Team UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar, string organisationId = null)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                avatar,
                organisationId);

            var message = updatedByUser.GetName()
                .AppendWith(" created the ")
                .AppendWith(Name).AppendWith(" team");

            EventProcessor.Raise(new DomainModelUpdatedEvent<Team>(this, updatedByUser, message));

            return this;
        }

        #endregion

    }
}