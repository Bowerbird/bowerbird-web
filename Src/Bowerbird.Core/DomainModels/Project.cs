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
                parentGroupId);

            var eventMessage = string.Format(
                ActivityMessage.CreatedAGroup,
                createdByUser.GetName(),
                GroupType(),
                Name
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<Project>(this, createdByUser, eventMessage));
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override string GroupType()
        {
            return "Project";
        }

        public Project UpdateDetails(User updatedByUser, string name, string description, string website, MediaResource avatar, string teamId = null)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNullOrWhitespace(name, "name");
            Check.RequireNotNullOrWhitespace(description, "description");

            SetDetails(
                name,
                description,
                website,
                avatar,
                teamId);

            var eventMessage = string.Format(
                ActivityMessage.UpdatedAGroup,
                updatedByUser.GetName(),
                Name,
                GroupType()
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<Project>(this, updatedByUser, eventMessage));

            return this;
        }

        #endregion

    }
}