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
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.CommandHandlers
{
    public class ChatDeleteCommandHandler : ICommandHandler<ChatDeleteCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ChatDeleteCommandHandler(
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

        public void Handle(ChatDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var chat = _documentSession.Load<Chat>(command.ChatId);

            _documentSession.Delete(chat);
            _documentSession.SaveChanges();
        }

        #endregion
    }
}