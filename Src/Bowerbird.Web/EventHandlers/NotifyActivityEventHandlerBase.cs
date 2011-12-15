using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
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