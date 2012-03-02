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
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class ClientSessionUpdateCommandHandler : ICommandHandler<ClientSessionUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ClientSessionUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ClientSessionUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var clientSession = _documentSession.Query<ClientSession>()
                .Where(x => x.ClientId == command.ClientId)
                .FirstOrDefault();

            clientSession.ConnectionCreated = command.Timestamp;    

            _documentSession.Store(clientSession);
        }

        #endregion
    }
}