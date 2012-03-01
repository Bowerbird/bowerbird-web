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

            var message = createdByUser.GetName().AppendWith(" created project ").AppendWith(name);

            EventProcessor.Raise(new DomainModelCreatedEvent<Project>(this, createdByUser, message));
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

            var message = updatedByUser.GetName().AppendWith(" updated project ").AppendWith(name);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Project>(this, updatedByUser, message));

            return this;
        }

        #endregion

    }
}