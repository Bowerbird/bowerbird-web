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
            EnableEvents();
        }

        public Organisation(
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
            SetDetails(
                description,
                website,
                avatar);

            EnableEvents();
            FireEvent(new DomainModelCreatedEvent<Organisation>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

        public override string GroupType
        {
            get { return "organisation"; }
        }

        #endregion

        #region Methods

        private void SetDetails(string description, string website, MediaResource avatar)
        {
            Description = description;
            Website = website;
            Avatar = avatar;
        }

        public Organisation UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");

            base.SetDetails(name);

            this.SetDetails(
                description,
                website,
                avatar);

            FireEvent(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser, this));

            return this;
        }

        #endregion

    }
}