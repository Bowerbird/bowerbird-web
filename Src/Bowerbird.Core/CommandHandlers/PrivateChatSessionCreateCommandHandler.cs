/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.DomainModels.Sessions;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class PrivateChatSessionCreateCommandHandler : ICommandHandler<PrivateChatSessionCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PrivateChatSessionCreateCommandHandler(
            IDocumentSession documentSession
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PrivateChatSessionCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var privateChatSession = new PrivateChatSession(
                _documentSession.Load<User>(command.UserId),
                command.ClientId,
                command.ChatId
                );

            _documentSession.Store(privateChatSession);
        }

        #endregion
    }
}