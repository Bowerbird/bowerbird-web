/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.DomainModels.Posts
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Events;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;

    #endregion

    public class TeamPost : Post
    {
        #region Members

        #endregion

        #region Constructors

        protected TeamPost() : base()
        {
        }

        public TeamPost(
            Team team,
            User createdByUser,
            DateTime postedOn,
            string subject,
            string message,
            IList<MediaResource> mediaResources
            )
            : base(createdByUser,
            postedOn,
            subject,
            message,
            mediaResources)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(team, "team");

            Team = team;

            EventProcessor.Raise(new DomainModelCreatedEvent<TeamPost>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Team> Team { get; private set; }

        #endregion

        #region Methods

        public TeamPost UpdateDetails(
            User updatedByUser,
            DateTime updatedOn,
            string message,
            string subject,
            IList<MediaResource> mediaResources
            )
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            UpdateDetails(
                updatedByUser,
                message,
                subject,
                mediaResources
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<TeamPost>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}