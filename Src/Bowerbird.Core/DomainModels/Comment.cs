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
using Bowerbird.Core.DomainModels.DenormalisedReferences;

namespace Bowerbird.Core.DomainModels
{
    public class Comment : DomainModel
    {
        #region Members

        #endregion

        #region Constructors

        protected Comment() : base() { }

        public Comment(
            User createdByUser,
            DateTime commentedOn,
            string message)
            : this()
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNullOrWhitespace(message, "message");

            CommentedOn = commentedOn;
            User = createdByUser;

            SetDetails(
                message,
                CommentedOn);
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public DateTime CommentedOn { get; private set; }

        public DateTime EditedOn { get; private set; }

        public string Message { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(string message, DateTime editedOn)
        {
            Message = message;
            EditedOn = editedOn;
        }

        public Comment UpdateDetails(User updatedByUser, DateTime editedOn, string message)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            SetDetails(
                message,
                editedOn);

            return this;
        }

        #endregion      
    }
}