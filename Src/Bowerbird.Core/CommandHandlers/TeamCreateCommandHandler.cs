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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Raven.Client;
using System;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamCreateCommandHandler : ICommandHandler<TeamCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var team = new Team(
                _documentSession.Load<User>(command.UserId), 
                command.Name, 
                command.Description, 
                command.Website,
                command.AvatarId != null ? _documentSession.Load<MediaResource>(command.AvatarId) : null,
                DateTime.Now);

            _documentSession.Store(team);

            var teamAdministrator = new Member(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<User>(command.UserId),
                team,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.Equals("roles/teamadministrator") || x.Id.Equals("roles/teammember"))
                    .ToList()
                );

            _documentSession.Store(teamAdministrator);

            Group parentGroup = null;
            if (string.IsNullOrEmpty(command.OrganisationId))
            {
                parentGroup = _documentSession.Load<AppRoot>(Constants.AppRootId);
            }
            else
            {
                parentGroup = _documentSession.Load<Organisation>(command.OrganisationId);
            }

            var groupAssociation = new GroupAssociation(
                parentGroup,
                team,
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow
                );

            _documentSession.Store(groupAssociation);
        }

        #endregion

    }
}