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

namespace Bowerbird.Core.DomainModels
{
    public class Team : DomainModel, INamedDomainModel
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
            Organisation organisation = null)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                organisation);

            EventProcessor.Raise(new DomainModelCreatedEvent<Team>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Website { get; private set; }

        public DenormalisedNamedDomainModelReference<Organisation> Organisation { get; set; }

        #endregion

        #region Methods

        private void SetDetails(string name, string description, string website, Organisation organisation = null)
        {
            Name = name;
            Description = description;
            Website = website;
            if (organisation != null) Organisation = organisation;
        }

        public Team UpdateDetails(User updatedByUser, string name, string description, string website, Organisation organisation = null)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                organisation);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Team>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}