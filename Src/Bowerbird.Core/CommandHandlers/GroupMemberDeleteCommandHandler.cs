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
using Bowerbird.Core.Repositories;
using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    public class GroupMemberDeleteCommandHandler : ICommandHandler<GroupMemberDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public GroupMemberDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(GroupMemberDeleteCommand groupMemberDeleteCommand)
        {
            Check.RequireNotNull(groupMemberDeleteCommand, "groupMemberDeleteCommand");

            var groupMember = _documentSession.LoadGroupMember(groupMemberDeleteCommand.GroupId, groupMemberDeleteCommand.UserId);

            _documentSession.Delete(groupMember);
        }

        #endregion
    }
}