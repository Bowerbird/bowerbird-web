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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Sessions;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class GroupChatSessionCreateCommandHandler : ICommandHandler<GroupChatSessionCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupChatSessionCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(GroupChatSessionCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var group = _documentSession
                .Query<Group, All_Groups>()
                .Where(x => x.Id == command.GroupId)
                .FirstOrDefault();

            if (group != null)
            {
                var groupChatSession = new GroupChatSession(
                   _documentSession.Load<User>(command.UserId),
                   command.ClientId,
                   group
                );

                _documentSession.Store(groupChatSession);
            }
        }

        #endregion
    }
}