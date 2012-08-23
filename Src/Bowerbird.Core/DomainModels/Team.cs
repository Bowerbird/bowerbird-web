/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Team Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Team : Group, IPublicGroup
    {
        #region Members

        #endregion

        #region Constructors

        protected Team()
            : base()
        {
        }

        public Team(
            User createdByUser,
            string name,
            string description,
            string website,
            MediaResource avatar,
            DateTime createdDateTime,
            Group parentGroup)
            : base(
            createdByUser,
            name,
            createdDateTime,
            parentGroup)
        {
            SetTeamDetails(
                description,
                website,
                avatar);

            ApplyEvent(new DomainModelCreatedEvent<Team>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

        public override string GroupType
        {
            get { return "team"; }
        }

        #endregion

        #region Methods

        private void SetTeamDetails(string description, string website, MediaResource avatar)
        {
            Description = description;
            Website = website;
            Avatar = avatar;
        }

        public Team UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");

            SetGroupDetails(name);

            SetTeamDetails(
                description,
                website,
                avatar);

            ApplyEvent(new DomainModelUpdatedEvent<Team>(this, updatedByUser, this));

            return this;
        }

        #endregion
    }
}