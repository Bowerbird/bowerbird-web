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
    public class GroupChatMessageCreateCommandHandler : ICommandHandler<GroupChatMessageCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupChatMessageCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(GroupChatMessageCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var group = _documentSession
                .Query<Group, All_Groups>()
                .Where(x => x.Id == command.GroupId)
                .FirstOrDefault();

            User targetUser = null;

            if(!string.IsNullOrEmpty(command.TargetUserId))
            {
                targetUser = _documentSession.Load<User>(command.TargetUserId);
            }

            var chatMessage = new GroupChatMessage(
               _documentSession.Load<User>(command.UserId),
               group,
               targetUser,
               command.Timestamp,
               command.Message
                );

            _documentSession.Store(chatMessage);
        }

        #endregion
    }
}