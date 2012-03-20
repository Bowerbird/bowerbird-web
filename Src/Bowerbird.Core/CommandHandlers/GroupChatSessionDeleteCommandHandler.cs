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
    public class GroupChatSessionDeleteCommandHandler : ICommandHandler<GroupChatSessionDeleteCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupChatSessionDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(GroupChatSessionDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var chatSession = _documentSession.Query<GroupChatSession>()
                .Where(x => x.ClientId == command.ClientId && x.Group.Id == command.GroupId)
                .FirstOrDefault();

            _documentSession.Delete(chatSession);
        }

        #endregion
    }
}