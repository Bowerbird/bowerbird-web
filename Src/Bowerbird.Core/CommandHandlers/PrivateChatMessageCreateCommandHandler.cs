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
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class PrivateChatMessageCreateCommandHandler : ICommandHandler<PrivateChatMessageCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PrivateChatMessageCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(PrivateChatMessageCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var targetUser = _documentSession.Load<User>(command.TargetUserId);

            var chatMessage = new PrivateChatMessage(
               _documentSession.Load<User>(command.UserId),
               command.ChatId,
               targetUser,
               command.Message,
               command.Timestamp
                );

            _documentSession.Store(chatMessage);
        }

        #endregion
    }
}