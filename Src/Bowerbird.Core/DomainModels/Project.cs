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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class Project : Group, IPublicGroup
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
            MediaResource background,
            DateTime createdDateTime,
            Group parentGroup)
            : base(
            createdByUser,
            name,
            createdDateTime,
            parentGroup)
        {
            SetProjectDetails(
                description,
                website,
                avatar,
                background);

            ApplyEvent(new DomainModelCreatedEvent<Project>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

        public MediaResource Background { get; private set; }

        public override string GroupType
        {
            get { return "project"; }
        }

        #endregion

        #region Methods

        private void SetProjectDetails(string description, string website, MediaResource avatar, MediaResource background)
        {
            Description = description;
            Website = website;
            Avatar = avatar;
            Background = background;
        }

        public Project UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar, MediaResource background)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");

            SetGroupDetails(name);

            SetProjectDetails(
                description,
                website,
                avatar,
                background);

            ApplyEvent(new DomainModelUpdatedEvent<Group>(this, updatedByUser, this));

            return this;
        }

        #endregion

    }
}