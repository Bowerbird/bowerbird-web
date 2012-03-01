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
    public class ClientSession
    {
        #region Fields

        #endregion

        #region Constructors

        protected ClientSession()
        {
        }

        public ClientSession(
         User user,
            Guid clientId,
            DateTime timestamp
            )
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNullOrWhitespace(clientId.ToString(), "clientId");

            User = user;
            ClientId = clientId;
            ConnectionCreated = timestamp;
        }

        #endregion

        #region Properties

        public DenormalisedUserReference User { get; set; }

        public Guid ClientId { get; set; }

        public DateTime ConnectionCreated { get; set; }

        public string Uri { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}