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
    public class Activity
    {
        #region Members

        #endregion

        #region Constructors

        protected Activity()
            : base()
        {
        }

        public Activity(
            User user,
            DateTime occurredOn,
            string sender,
            string action,
            string groupId,
            string watchlistId,
            string message)
            : this()
        {
            Check.RequireNotNull(user, "user");

            User = user;
            OccurredOn = occurredOn;
            Sender = sender;
            Action = action;
            GroupId = groupId;
            WatchlistId = watchlistId;
            Message = message;
        }

        #endregion

        #region Properties

        public DateTime OccurredOn { get; private set; }

        public DenormalisedUserReference User { get; private set; }

        public string Sender { get; set; }

        public string Action { get; set; }

        public string GroupId { get; set; }

        public string WatchlistId { get; set; }

        public string Message { get; set; }

        #endregion

        #region Methods

        #endregion      
    }
}