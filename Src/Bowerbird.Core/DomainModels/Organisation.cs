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
            MediaResource avatar,
            DateTime createdDateTime)
            : base(
            createdByUser,
            name,
            createdDateTime)
        {
            SetDetails(
                description,
                website,
                avatar);

            EventProcessor.Raise(new DomainModelCreatedEvent<Organisation>(this, createdByUser.Id));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

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

            EventProcessor.Raise(new DomainModelUpdatedEvent<Organisation>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}