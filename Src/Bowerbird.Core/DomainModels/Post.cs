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

using System.Collections.Generic;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using System;
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Post : Contribution
    {
        #region Members

        #endregion

        #region Constructors

        protected Post() : base() { }

        public Post(
            User createdByUser,
            DateTime createdOn,
            string subject,
            string message,
            IList<MediaResource> mediaResources)
            : base(
            createdByUser,
            createdOn)
        {
            Check.RequireNotNullOrWhitespace(subject, "subject");
            Check.RequireNotNullOrWhitespace(message, "message");
            Check.RequireNotNull(mediaResources, "mediaResources");

            SetDetails(
                subject,
                message,
                mediaResources);

            EventProcessor.Raise(new DomainModelCreatedEvent<Post>(this, createdByUser));
        }

        #endregion

        #region Properties

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public IList<MediaResource> MediaResources { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string subject, string message, IList<MediaResource> mediaResources)
        {
            Subject = subject;
            Message = message;
            MediaResources = mediaResources;
        }

        public Post UpdateDetails(User updatedByUser, string subject, string message, IList<MediaResource> mediaResources)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                subject,
                message,
                mediaResources);

            EventProcessor.Raise(new DomainModelUpdatedEvent<Post>(this, updatedByUser));

            return this;
        }

        #endregion
    }
}