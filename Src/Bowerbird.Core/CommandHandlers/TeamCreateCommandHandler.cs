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
using Bowerbird.Core.Factories;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamCreateCommandHandler : ICommandHandler<TeamCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public TeamCreateCommandHandler(
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
        /// If Team has a parent OrganisationId: 
        ///     - Add Organisation to Team Ancestry
        ///     - Add Team to Organisation Descendants
        /// </summary>
        public void Handle(TeamCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            // Get parent group
            var parentGroup =
                !string.IsNullOrEmpty(command.OrganisationId)
                    ? (Group)_documentSession.Load<Organisation>(command.OrganisationId)
                    : (Group)_documentSession.Load<AppRoot>(Constants.AppRootId);

            // Make team
            var team = new Team(
                _documentSession.Load<User>(command.UserId), 
                command.Name, 
                command.Description, 
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Team) : _documentSession.Load<MediaResource>(command.AvatarId),
                DateTime.UtcNow,
                parentGroup);
            _documentSession.Store(team);
            
            // If team is in an organisation, add team to organisation's Descendants
            if(!string.IsNullOrEmpty(command.OrganisationId))
            {
                parentGroup.AddChildGroup(team);
                _documentSession.Store(parentGroup);
            }

            // Add administrator membership to creating user
            var user = _documentSession.Load<User>(command.UserId);
            user.AddMembership(
                user,
                team,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.In("roles/teamadministrator", "roles/teammember"))
                    .ToList());
            _documentSession.Store(user);
        }

        #endregion

    }
}