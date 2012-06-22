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
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.CommandHandlers
{
    public class ChatUpdateCommandHandler : ICommandHandler<ChatUpdateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ChatUpdateCommandHandler(
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

        public void Handle(ChatUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var chat = _documentSession.Load<Chat>(command.ChatId);

            var addUsers = _documentSession.Load<User>(command.AddUserIds);
            foreach(var user in addUsers)
            {
                chat.AddUser(user);
            }

            var removeUsers = _documentSession.Load<User>(command.RemoveUserIds);
            foreach (var user in removeUsers)
            {
                chat.RemoveUser(user);
            }

            _documentSession.Store(chat);
        }

        #endregion

    }
}
