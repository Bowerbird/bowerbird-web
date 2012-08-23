/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using Bowerbird.Core.Config;
using System;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;

namespace Bowerbird.Core.CommandHandlers
{
    public class OrganisationCreateCommandHandler : ICommandHandler<OrganisationCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public OrganisationCreateCommandHandler(
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

        /// <summary>
        /// Ancestry = The AppRoot.
        /// Is not a descendant of any parent object (we don't add descendants to the app root)
        /// </summary>
        public void Handle(OrganisationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var appRoot = _documentSession.Load<AppRoot>(Constants.AppRootId);

            // Make organisation
            var organisation = new Organisation(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Organisation) : _documentSession.Load<MediaResource>(command.AvatarId),
                DateTime.UtcNow,
                appRoot);
            _documentSession.Store(organisation);

            // Add administrator membership to creating user
            var user = _documentSession.Load<User>(command.UserId);
            user.AddMembership(
                user,
                organisation,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.In("roles/organisationadministrator", "roles/organisationmember"))
                    .ToList());
            _documentSession.Store(user);
        }

        #endregion
    }
}