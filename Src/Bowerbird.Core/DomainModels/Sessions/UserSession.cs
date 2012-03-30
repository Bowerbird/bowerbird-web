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

namespace Bowerbird.Core.DomainModels.Sessions
{
    public class UserSession : Session
    {
        #region Fields

        #endregion

        #region Constructors

        protected UserSession()
        {
        }

        public UserSession(
            User user,
            string clientId
            )
            : base(user, clientId)
        {
            LatestActivity = DateTime.UtcNow;

            SetDetails(
                DateTime.UtcNow,
                (int)Connection.ConnectionStatus.Online);
        }

        #endregion

        #region Properties

        public DateTime LatestActivity { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(DateTime latestActivity, int status)
        {
            LatestActivity = latestActivity;
            base.SetDetails(status);
        }

        public void UpdateDetails(
            DateTime latestActivity,
            int status)
        {
            SetDetails(latestActivity,status);
        }

        #endregion
    }
}