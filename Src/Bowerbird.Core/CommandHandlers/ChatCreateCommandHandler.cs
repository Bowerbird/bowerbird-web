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
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class ChatCreateCommandHandler : ICommandHandler<ChatCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ChatCreateCommandHandler(
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

        public void Handle(ChatCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var createdByUser = _documentSession.Load<User>(command.CreatedByUserId);

            var chat = new Chat(
                command.ChatId,
                createdByUser,
                _documentSession.Load<User>(command.UserIds),
                command.CreatedDateTime,
                command.Message);

            _documentSession.Store(chat);
        }

        #endregion
    }
}