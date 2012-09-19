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
using System.Threading.Tasks;
using Bowerbird.Core.Extensions;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Indexes;
using System.Linq;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.Builders;

namespace Bowerbird.Web.Hubs
{
    public class UserHub : Hub, IDisconnect, IConnected
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewModelBuilder _userViewModelBuilder;

        #endregion

        #region Constructors

        public UserHub(
            IDocumentSession documentSession,
            IUserViewModelBuilder userViewModelBuilder)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");

            _documentSession = documentSession;
            _userViewModelBuilder = userViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void RegisterUserClient(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            // Add user to their own group
            Groups.Add(Context.ConnectionId, "user-" + userId);

            // Update user's status to online
            user.AddSession(Context.ConnectionId);
            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        public void UpdateUserClientStatus(string userId)
        {
            var user = GetUserByConnectionId(Context.ConnectionId);

            if (user != null)
            {
                user.UpdateSessionLatestActivity(Context.ConnectionId);

                _documentSession.Store(user);
                _documentSession.SaveChanges();
            }
        }

        public Task Disconnect()
        {
            // Remove this connection session from user
            var user = GetUserByConnectionId(Context.ConnectionId);
            user.RemoveSession(Context.ConnectionId);
            _documentSession.Store(user);
            _documentSession.SaveChanges();

            Groups.Remove(Context.ConnectionId, "online-users");
            Groups.Remove(Context.ConnectionId, "user-" + user.Id);

            return Task.Factory.StartNew(() => { });
        }

        private User GetUserByConnectionId(string connectionId)
        {
            var result = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.ConnectionIds.Any(y => y == connectionId))
                .FirstOrDefault();

            return result != null ? result.User : null;
        }

        public Task Connect()
        {
            return null;
        }

        public Task Reconnect(IEnumerable<string> groups)
        {
            var user = GetUserByConnectionId(Context.ConnectionId);

            if (user != null)
            {
                user.UpdateSessionLatestActivity(Context.ConnectionId);

                _documentSession.Store(user);
                _documentSession.SaveChanges();
            }

            return null;
        }

        #endregion

    }
}