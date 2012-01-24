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

using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Posts;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class TeamPostUpdateCommandHandler : ICommandHandler<TeamPostUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public TeamPostUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(TeamPostUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var teamPost = _documentSession.Load<TeamPost>(command.Id);

            teamPost.UpdateDetails(
                _documentSession.Load<User>(command.UserId),
                command.Timestamp,
                command.Message,
                command.Subject,
                _documentSession.Load<MediaResource>(command.MediaResources)
                );

            _documentSession.Store(teamPost);
        }

        #endregion

    }
}