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

namespace Bowerbird.Core.DomainModels
{
    public class Notification : DomainModel
    {
        #region Fields

        #endregion

        #region Constructors

        public Notification(
            Activity activity,
            DateTime timeStamp,
            IEnumerable<string> userIds 
            )
        {
            Check.RequireNotNull(activity, "activity");
            Check.RequireNotNull(userIds, "userIds");

            Activity = activity;
            TimeStamp = timeStamp;
            UserIds = userIds;
        }

        #endregion

        #region Properties

        public Activity Activity { get; set; }

        public DateTime TimeStamp { get; set; }

        public IEnumerable<string> UserIds { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}