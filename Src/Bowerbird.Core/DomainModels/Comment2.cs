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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Comment2 : DomainModel, IContribution, ISubContribution
    {
        #region Members

        #endregion

        #region Constructors

        protected Comment2()
            : base()
        {
        }

        public Comment2(
            User createdByUser,
            DateTime createdOn,
            string message) 
            : base()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(message, "message");

            User = createdByUser;
            CreatedOn = createdOn;

            SetCommentDetails(
                message,
                createdByUser,
                createdOn);
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public DenormalisedUserReference EditedBy { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        protected void SetCommentDetails(string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Message = message;
            EditedBy = modifiedByUser;
            EditedOn = modifiedDateTime;
        }

        public Comment2 UpdateDetails(string message, User modifiedByUser, DateTime modifiedDateTime)
        {
            Check.RequireNotNull(modifiedByUser, "modifiedByUser");

            SetCommentDetails(message, modifiedByUser, modifiedDateTime);

            return this;
        }

        public ISubContribution GetSubContribution(string type, string id)
        {
            return null;
        }

        #endregion

    }
}