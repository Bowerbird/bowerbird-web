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
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupId == command.GroupId)
                .FirstOrDefault();

            User targetUser = null;
            Group chatGroup = null;

            if(group.Group is Team)
            {
                chatGroup = _documentSession.Load<Team>(command.GroupId);
            }
            else if (group.Group is Project)
            {
                chatGroup = _documentSession.Load<Project>(command.GroupId);
            }

            if(!string.IsNullOrEmpty(command.TargetUserId))
            {
                targetUser = _documentSession.Load<User>(command.TargetUserId);
            }

            var chatMessage = new GroupChatMessage(
               _documentSession.Load<User>(command.UserId),
               chatGroup,
               targetUser,
               command.Timestamp,
               command.Message
               );
            
            _documentSession.Store(chatMessage);
        }

        #endregion
    }
}