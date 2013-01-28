/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Organisation : Group, IPublicGroup
    {
        #region Members

        #endregion

        #region Constructors

        protected Organisation()
            : base()
        {
            InitMembers();
        }

        public Organisation(
            User createdByUser,
            string name,
            string description,
            string website,
            MediaResource avatar,
            MediaResource background,
            IEnumerable<string> categories,
            DateTime createdDateTime,
            Group parentGroup)
            : base(
            createdByUser,
            name,
            createdDateTime,
            parentGroup)
        {
            Check.RequireNotNull(categories != null, "categories");

            InitMembers();

            SetOrganisationDetails(
                description,
                website,
                avatar,
                background,
                categories);

            ApplyEvent(new DomainModelCreatedEvent<Organisation>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

        public MediaResource Background { get; private set; }

        public IEnumerable<string> Categories { get; private set; }

        public override string GroupType
        {
            get { return "organisation"; }
        }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Categories = new List<string>();
        }

        private void SetOrganisationDetails(string description, string website, MediaResource avatar,
            MediaResource background, IEnumerable<string> categories)
        {
            Description = description;
            Website = website;
            Avatar = avatar;
            Background = background;
            Categories = categories;
        }

        public Organisation UpdateDetails(User updatedByUser, string name, string description, string website,
            MediaResource avatar, MediaResource background, IEnumerable<string> categories)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNull(categories != null, "categories");

            SetGroupDetails(name);

            SetOrganisationDetails(
                description,
                website,
                avatar,
                background, 
                categories);

            ApplyEvent(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser, this));

            return this;
        }

        #endregion

    }
}