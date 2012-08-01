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

using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;
using Bowerbird.Core.Config;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using System.Collections.Generic;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>, ICommandHandler<UserCreateCommand, User>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public UserCreateCommandHandler(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(UserCreateCommand userCreateCommand)
        {
            HandleReturn(userCreateCommand);
        }

        public User HandleReturn(UserCreateCommand userCreateCommand)
        {
            Check.RequireNotNull(userCreateCommand, "userCreateCommand");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            // Make user
            var user = new User(
                userCreateCommand.Password,
                userCreateCommand.Email,
                userCreateCommand.FirstName,
                userCreateCommand.LastName,
                _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.User),
                Constants.DefaultLicence);
            _documentSession.Store(user);

            // Make user project
            var userProject = new UserProject(user, DateTime.UtcNow, appRoot);
            _documentSession.Store(userProject);

            // Add app root membership
            user.AddMembership(
                user,
                appRoot,
                _documentSession.Query<Role>().Where(x => x.Id.In(userCreateCommand.Roles)).ToList());

            // Add administrator membership to user project
            user.AddMembership(
                user, 
                userProject, 
                _documentSession.Query<Role>().Where(x => x.Id == "roles/userprojectadministrator" || x.Id == "roles/userprojectmember"));
            _documentSession.Store(user);

            return user;
        }

        #endregion      
    }
}