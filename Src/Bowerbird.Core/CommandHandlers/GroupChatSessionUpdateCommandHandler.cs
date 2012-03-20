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
    public class GroupChatSessionUpdateCommandHandler : ICommandHandler<GroupChatSessionUpdateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupChatSessionUpdateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(GroupChatSessionUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var chatSession = _documentSession.Query<GroupChatSession>()
                .Where(x => x.ClientId == command.ClientId && x.Group.Id == command.GroupId)
                .FirstOrDefault();

            chatSession.UpdateDetails(command.Status);

            _documentSession.Store(chatSession);
        }

        #endregion
    }
}