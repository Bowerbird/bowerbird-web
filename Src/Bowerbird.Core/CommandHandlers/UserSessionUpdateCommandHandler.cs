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

namespace Bowerbird.Core.CommandHandlers
{
    public class UserSessionUpdateCommandHandler : ICommandHandler<UserSessionUpdateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserSessionUpdateCommandHandler(
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
        public void Handle(UserSessionUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var userSession = _documentSession.Query<UserSession>()
                .Where(x => x.ClientId == command.ClientId)
                .FirstOrDefault();

            if (userSession != null)
            {
                userSession.LatestActivity = command.LatestActivity;
                userSession.Status = command.Status;

                _documentSession.Store(userSession);
            }
        }

        #endregion
    }
}