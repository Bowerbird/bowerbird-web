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
using Bowerbird.Core.DomainModels.Sessions;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class UserSessionDeleteCommandHandler : ICommandHandler<UserSessionDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserSessionDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        // todo: probably masking an error here
        public void Handle(UserSessionDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var session = _documentSession
                .Query<UserSession>()
                .Where(x => x.ClientId == command.ClientId)
                .FirstOrDefault();

            if(session != null)
            {
                _documentSession.Delete(session);
            }
        }

        #endregion
    }
}