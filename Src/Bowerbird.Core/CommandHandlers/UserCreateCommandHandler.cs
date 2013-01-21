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
    public class UserCreateCommandHandler : ICommandHandler<UserCreateCommand>
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
            Check.RequireNotNull(userCreateCommand, "userCreateCommand");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);
            var defaultAvatarImage = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.User);
            var defaultBackgroundImage = _mediaResourceFactory.MakeDefaultBackgroundImage("user");

            // Make user
            var user = new User(
                userCreateCommand.Password,
                userCreateCommand.Email,
                userCreateCommand.Name,
                defaultAvatarImage,
                Constants.DefaultLicence,
                userCreateCommand.Timezone,
                DateTime.UtcNow);
            _documentSession.Store(user);

            // Make user's project group
            var userProject = new UserProject(
                user, 
                userCreateCommand.Name, 
                string.Empty,
                string.Empty,
                defaultAvatarImage,
                defaultBackgroundImage,
                DateTime.UtcNow, 
                appRoot);
            _documentSession.Store(userProject);

            // Make user's favourites group
            var favourites = new Favourites(
                user,
                DateTime.UtcNow,
                appRoot);
            _documentSession.Store(favourites);

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

            // Add administrator membership to favourites
            user.AddMembership(
                user,
                favourites,
                _documentSession.Query<Role>().Where(x => x.Id == "roles/favouritesadministrator" || x.Id == "roles/favouritesmember"));

            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        #endregion      
    }
}
