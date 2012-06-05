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
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public OrganisationCreateCommandHandler(
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

        /// <summary>
        /// Ancestry = The AppRoot.
        /// Is not a descendant of any parent object (we don't add descendants to the app root)
        /// </summary>
        public void Handle(OrganisationCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var parentGroup = _documentSession.Load<AppRoot>(Constants.AppRootId);

            var organisation = new Organisation(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Organisation) : _documentSession.Load<MediaResource>(command.AvatarId),
                DateTime.Now,
                parentGroup);

            //organisation.SetAncestry(parentGroup);
            _documentSession.Store(organisation);

            var organisationAdministrator = new Member(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<User>(command.UserId),
                organisation,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.Equals("roles/organisationadministrator") || x.Id.Equals("roles/organisationmember"))
                    .ToList(),
                false
                );

            _documentSession.Store(organisationAdministrator);
            
            var groupAssociation = new GroupAssociation(
                parentGroup,
                organisation,
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow
                );

            _documentSession.Store(groupAssociation);
        }

        #endregion
    }
}