/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Core.Config;
using Bowerbird.Core.Indexes;

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

        public void Handle(OrganisationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            // Get parent group
            Group parentGroup = null;

            parentGroup = _documentSession.Load<AppRoot>(Constants.AppRootId);

            // Make organisation
            var organisation = new Organisation(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Organisation) : _documentSession.Load<MediaResource>(command.AvatarId),
                string.IsNullOrWhiteSpace(command.BackgroundId) ? _mediaResourceFactory.MakeDefaultBackgroundImage("organisation") : _documentSession.Load<MediaResource>(command.BackgroundId),
                DateTime.UtcNow,
                parentGroup);
            _documentSession.Store(organisation);

            // Add administrator membership to creating user
            var user = _documentSession.Load<User>(command.UserId);
            user.UpdateMembership(
                user,
                organisation,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.In("roles/organisationadministrator", "roles/organisationmember"))
                    .ToList());

            _documentSession.Store(user);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}