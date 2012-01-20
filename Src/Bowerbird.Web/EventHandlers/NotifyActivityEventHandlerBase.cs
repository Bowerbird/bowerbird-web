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

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.EventHandlers
{
    public abstract class NotifyActivityEventHandlerBase
    {
        #region Members

        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        protected NotifyActivityEventHandlerBase(
            IUserContext userContext)
        {
            Check.RequireNotNull(userContext, "userContext");

            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected void Notify(string type, User user, object data)
        {
            Check.RequireNotNullOrWhitespace(type, "type");
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(data, "data");

            var activity = new Activity(
                type,
                user,
                data);

            var clients = _userContext.GetChannel();

            clients.activityOccurred(activity);
        }

        #endregion
    }
}