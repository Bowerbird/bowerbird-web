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

using System;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamMemberCreateCommandHandler : ICommandHandler<TeamMemberCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamMemberCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamMemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var teamMember = new TeamMember(
                _documentSession.Load<User>(command.CreatedByUserId),
                _documentSession.Load<Team>(command.TeamId),
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<Role>(command.Roles)
                );

            _documentSession.Store(teamMember);
        }

        #endregion

    }
}