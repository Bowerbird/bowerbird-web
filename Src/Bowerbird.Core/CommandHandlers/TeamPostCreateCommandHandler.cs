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
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamPostCreateCommandHandler : ICommandHandler<TeamPostCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamPostCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamPostCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var teamPost = new TeamPost(
                _documentSession.Load<Team>(command.TeamId),
                _documentSession.Load<User>(command.UserId),
                command.PostedOn,
                command.Subject,
                command.Message,
                _documentSession.Load<MediaResource>(command.MediaResources).ToList());

            _documentSession.Store(teamPost);
        }

        #endregion

    }
}