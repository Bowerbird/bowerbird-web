/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Raven.Client;
using System.Collections.Generic;
using System.Linq;

namespace Bowerbird.Web.Factories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Fields

        protected readonly IAvatarFactory _avatarFactory;
        protected readonly IDocumentSession _documentSession;
        protected readonly IProjectViewFactory _projectViewFactory;
        protected readonly ITeamViewFactory _teamViewFactory;

        #endregion

        #region Constructors

        public UserViewFactory(
            IAvatarFactory avatarFactory,
            IDocumentSession documentSession,
            IProjectViewFactory projectViewFactory,
            ITeamViewFactory teamViewFactory)
        {
            Check.RequireNotNull(avatarFactory, "avatarFactory");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(projectViewFactory, "projectViewFactory");
            Check.RequireNotNull(teamViewFactory, "teamViewFactory");

            _avatarFactory = avatarFactory;
            _documentSession = documentSession;
            _projectViewFactory = projectViewFactory;
            _teamViewFactory = teamViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(string userId)
        {
            var user = _documentSession.Load<User>(userId);

            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        public object Make(User user)
        {
            Check.RequireNotNull(user, "user");

            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        public object Make(User user, IEnumerable<Member> memberships)
        {
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(memberships, "memberships");

            return new
            {
                Avatar = _avatarFactory.Make(user),
                user.Id,
                user.LastLoggedIn,
                Name = user.GetName(),
                Projects = _documentSession.Load<Project>(memberships.Where(x => x.Group.Id.StartsWith("projects/")).Select(x => x.Group.Id)).Select(x => _projectViewFactory.Make(x)),
                Teams = _documentSession.Load<Team>(memberships.Where(x => x.Group.Id.StartsWith("teams/")).Select(x => x.Group.Id)).Select(x => _teamViewFactory.Make(x))
            };
        }

        #endregion
    }
}