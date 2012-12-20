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
using System.Linq;
using System.Threading.Tasks;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Web.Services;
using Raven.Client.Linq;
using SignalR.Hubs;
using Bowerbird.Core.DesignByContract;
using Raven.Client;
using Bowerbird.Web.Factories;
using System.Collections.Generic;

namespace Bowerbird.Web.Hubs
{
    public class GroupHub : Hub
    {
        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupHub(
            IUserViewFactory userViewFactory,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _userViewFactory = userViewFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void JoinGroups(string userId)
        {
            // Register user with all groups they are a member of
            foreach (var groupId in GetUserGroups(userId))
            {
                Groups.Add(Context.ConnectionId, "group-" + groupId);
            }
        }

        public void JoinGroup(string userId, string groupId)
        {
            Groups.Add(Context.ConnectionId, "group-" + groupId);
        }

        public void LeaveGroup(string userId, string groupId)
        {
            Groups.Remove(Context.ConnectionId, "group-" + groupId);
        }

        //public Task Disconnect()
        //{
        //    // Unregister user from all groups they are a member of
        //    var user = GetUserByConnectionId(Context.ConnectionId);
        //    foreach (var groupId in GetUserGroups(user.Id))
        //    {
        //        Groups.Remove(Context.ConnectionId, "group-" + groupId);
        //    }

        //    return Task.Factory.StartNew(() => { });
        //}

        private IEnumerable<string> GetUserGroups(string userId)
        {
            return _documentSession.Query<All_Groups.Result, All_Groups>()
                .Where(x => x.UserIds.Any(y => y == userId))
                .AsProjection<All_Groups.Result>()
                .ToList()
                .Select(x => x.GroupId);
        }

        //private User GetUserByConnectionId(string connectionId)
        //{
        //    return _documentSession
        //        .Query<All_Users.Result, All_Users>()
        //        .AsProjection<All_Users.Result>()
        //        .Where(x => x.ConnectionIds.Any(y => y == connectionId))
        //        .ToList()
        //        .Select(x => x.User)
        //        .First();
        //}

        #endregion

    }
}