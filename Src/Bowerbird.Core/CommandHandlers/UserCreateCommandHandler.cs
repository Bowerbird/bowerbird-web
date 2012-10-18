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
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public UserCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
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
                userCreateCommand.Name,
                _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.User),
                userCreateCommand.DefaultLicence,
                userCreateCommand.Timezone);
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

            // Add calls to action
            user
                .AddCallToAction("welcome")
                .AddCallToAction("first-project")
                .AddCallToAction("first-observation")
                .AddCallToAction("first-record");

            //// HACK: Registers user in small number of initial projects for now
            //// 7, 8, 3, 4
            //var projects = _documentSession.Load<Project>(new[] { "projects/4", "projects/7" });

            //var role = _documentSession.Query<Role>().Where(x => x.Id == "roles/projectmember");

            //foreach (var project in projects)
            //{
            //    if (project != null)
            //    {
            //        user.AddMembership(
            //            user,
            //            project,
            //            role);
            //    }
            //}

            _documentSession.Store(user);

            return user;
        }

        #endregion      
    }
}
