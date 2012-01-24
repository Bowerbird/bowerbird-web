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
using Bowerbird.Core.DomainModels.Members;
using Raven.Client;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamMemberDeleteCommandHandler : ICommandHandler<TeamMemberDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamMemberDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamMemberDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            _documentSession.Delete(_documentSession.LoadTeamMember(command.TeamId, command.MemberId));
        }

        #endregion

    }
}