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

using System;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class Project : Group
    {
        #region Members

        #endregion

        #region Constructors

        protected Project()
            : base()
        {
        }

        public Project(
            User createdByUser,
            string name,
            string description,
            string website,
            MediaResource avatar,
            string parentGroupId = null)
            : base(
            createdByUser,
            name,
            parentGroupId)
        {
            SetDetails(
                description,
                website,
                avatar);

            EventProcessor.Raise(new DomainModelCreatedEvent<Project>(this, createdByUser));
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

        public Project UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");

            base.SetDetails(name);

            this.SetDetails(
                description,
                website,
                avatar);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Project>(this, updatedByUser));

            return this;
        }

        #endregion

    }
}