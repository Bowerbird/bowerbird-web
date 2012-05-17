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
    public class MemberDeleteCommandHandler : ICommandHandler<MemberDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(MemberDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");

            var group = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id == command.GroupId)
                .FirstOrDefault();

            if (group.Project != null)
            {
                DeleteMember(
                    command.UserId,
                    group.Project.Id
                    );
            }

            if (group.Team != null)
            {
                DeleteMember(
                    command.UserId,
                    group.Team.Id
                    );

                foreach (var childGroupId in group.ChildGroupIds)
                {
                    DeleteMember(
                        command.UserId,
                        childGroupId
                        );
                }
            }

            if (group.Organisation != null)
            {
                DeleteMember(
                    command.UserId,
                    group.Organisation.Id
                    );

                foreach (var childGroupId in group.ChildGroupIds)
                {
                    DeleteMember(
                        command.UserId,
                        childGroupId
                        );
                }
            }
        }

        private void DeleteMember(
            string userId,
            string groupId
            )
        {
            var member = _documentSession
                .Query<Member>()
                .Where(x => x.Group.Id == groupId && x.User.Id == userId);

            if (member != null)
            {
                _documentSession.Delete(member);
            }
        }

        #endregion
    }
}