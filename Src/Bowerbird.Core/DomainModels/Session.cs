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
    public abstract class Session : ValueObject
    {
        #region Fields

        #endregion

        #region Constructors

        protected Session()
        {
        }

        protected Session(
            User user,
            string clientId
            )
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNullOrWhitespace(clientId, "clientId");

            User = user;
            ClientId = clientId;
            CreatedDateTime = DateTime.UtcNow;
            
            SetDetails((int)Connection.ConnectionStatus.Online);
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; private set; }

        public string ClientId { get; private set; }

        public DateTime CreatedDateTime { get; private set; }

        public int Status { get; private set; }

        #endregion

        #region Methods

        protected void SetDetails(int status)
        {
            Status = status;
        }

        public void UpdateDetails(int status)
        {
            SetDetails(status);
        }

        #endregion
    }
}