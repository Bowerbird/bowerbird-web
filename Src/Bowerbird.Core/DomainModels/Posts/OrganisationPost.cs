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
using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels.Posts
{
    public class OrganisationPost : Post
    {
        #region Members

        #endregion

        #region Constructors

        protected OrganisationPost() : base() { }

        public OrganisationPost(
            Organisation organisation,
            User createdByUser,
            DateTime timestamp,
            string subject,
            string message,
            IList<MediaResource> mediaResources
            )
            : base(createdByUser,
            timestamp,
            subject,
            message,
            mediaResources)
        {
            Check.RequireNotNull(organisation, "organisation");

            Organisation = organisation;

            EventProcessor.Raise(new DomainModelCreatedEvent<OrganisationPost>(this, createdByUser));
        }

        #endregion

        #region Properties

        public DenormalisedNamedDomainModelReference<Organisation> Organisation { get; private set; }

        #endregion

        #region Methods

        public OrganisationPost UpdateDetails(User updatedByUser,
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

            EventProcessor.Raise(new DomainModelUpdatedEvent<OrganisationPost>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}