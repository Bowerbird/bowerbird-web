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

namespace Bowerbird.Core.DomainModels.Sessions
{
    public class GroupChatSession : Session
    {
        #region Fields

        #endregion

        #region Constructors

        protected GroupChatSession()
        {
        }

        public GroupChatSession(
            User user,
            string clientId,
            Group group
            )
            : base ( user, clientId)
        {
            Check.RequireNotNull(group, "group");

            GroupId = group.Id;
        }

        #endregion

        #region Properties

        public string GroupId { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}