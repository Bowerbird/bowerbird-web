/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Web.Script.Serialization;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;

namespace Bowerbird.Web.EventHandlers
{
    public abstract class NotifyActivityEventHandlerBase
    {
        #region Members

        protected readonly IUserContext _userContext;

        #endregion

        #region Constructors

        protected NotifyActivityEventHandlerBase(
            IUserContext userContext
            )
        {
            Check.RequireNotNull(userContext, "userContext");

            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected void Notify(ActivityMessage message, List<string> clientsToNotify)
        {
            Check.RequireNotNull(message, "message");

            var clients = _userContext.GetChannel();

            foreach (var clientId in clientsToNotify)
            {
                clients[clientId].activityOccurred(new JavaScriptSerializer().Serialize(message));
            }
        }

        #endregion
    }
}