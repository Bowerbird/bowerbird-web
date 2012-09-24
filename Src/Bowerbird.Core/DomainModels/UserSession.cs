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

namespace Bowerbird.Core.DomainModels
{
    public class UserSession
    {
        #region Fields

        #endregion

        #region Constructors

        protected UserSession()
            : base()
        {
        }

        public UserSession(
            string connectionId)
            : base()
        {
            ConnectionId = connectionId;
            CreatedDateTime = DateTime.UtcNow;

            SetDetails(DateTime.UtcNow, DateTime.UtcNow);
        }

        #endregion

        #region Properties

        public string ConnectionId { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public DateTime LatestHeartbeat { get; private set; }

        public DateTime LatestActivity { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(DateTime latestHeartbeat, DateTime latestActivity)
        {
            LatestHeartbeat = latestHeartbeat;
            LatestActivity = latestActivity;
        }

        public void UpdateLatestActivity(DateTime latestHeartbeat, DateTime latestActivity)
        {
            SetDetails(latestHeartbeat, latestActivity);
        }

        #endregion
    }
}