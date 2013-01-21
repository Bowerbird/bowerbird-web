/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
    public class UserProject : Group, IPublicGroup
    {
        #region Members

        #endregion

        #region Constructors

        protected UserProject()
            : base()
        {
        }

        public UserProject(
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
            SetUserProjectDetails(
                description,
                website,
                avatar,
                background);

            ApplyEvent(new DomainModelCreatedEvent<UserProject>(this, createdByUser, this));
        }

        #endregion

        #region Properties

        public string Description { get; private set; }

        public string Website { get; private set; }

        public MediaResource Avatar { get; private set; }

        public MediaResource Background { get; private set; }

        public override string GroupType
        {
            get { return "userproject"; }
        }
        
        #endregion

        #region Methods

        private void SetUserProjectDetails(string description, string website, MediaResource avatar, MediaResource background)
        {
            Description = description;
            Website = website;
            Avatar = avatar;
            Background = background;
        }

        public UserProject UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar, MediaResource background)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(name, "avatar");
            Check.RequireNotNullOrWhitespace(name, "background");

            SetGroupDetails(name);

            SetUserProjectDetails(
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