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
using Bowerbird.Core.DomainModels.Sessions;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class PrivateChatSessionDeleteCommandHandler : ICommandHandler<PrivateChatSessionDeleteCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public PrivateChatSessionDeleteCommandHandler(
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

        public void Handle(PrivateChatSessionDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var privateChatSession = _documentSession.Query<PrivateChatSession>()
                .Where(x => x.ChatId == command.ChatId && x.ClientId == command.ClientId)
                .FirstOrDefault();

            _documentSession.Delete(privateChatSession);
        }

        #endregion
    }
}