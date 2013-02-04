/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.ComponentModel;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserFollowUpdateCommandHandler : ICommandHandler<UserFollowUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserFollowUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserFollowUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var follower = _documentSession.Load<User>(command.FollowerUserId);
            var followee = _documentSession.Load<User>(command.FolloweeUserId);

            // Under no cicrumstances should you be able to unfollow your self. Madness will ensue.
            if (follower.Id == followee.Id) return;

            var userProjectToFollow = _documentSession.Load<UserProject>(followee.UserProject.Id);

            if (follower.Memberships.Any(x => x.Group.Id == userProjectToFollow.Id))
            {
                // Unfollow
                follower.RemoveMembership(follower, userProjectToFollow);
            }
            else
            {
                // Follow
                follower.UpdateMembership(follower, userProjectToFollow, new [] { _documentSession.Load<Role>("roles/userprojectmember") });
            }

            _documentSession.Store(follower);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}