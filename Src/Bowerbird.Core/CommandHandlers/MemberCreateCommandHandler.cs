/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.CommandHandlers
{
    public class MemberCreateCommandHandler : ICommandHandler<MemberCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public MemberCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(MemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var group = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.ClientResult>()
                .Where(x => x.GroupId == command.GroupId)
                .FirstOrDefault();

            var user = _documentSession.Load<User>(command.UserId);
            var createdByUser = _documentSession.Load<User>(command.CreatedByUserId);

            if(group.Project != null)
            {
                CreateNewMember(
                    createdByUser, 
                    user, 
                    group.Project, 
                    GetRoles(command.Roles));

                var team = group
                    .AncestorGroups
                    .Where(x => x.GroupType == "team")
                    .FirstOrDefault();

                if(team != null)
                {
                    CreateNewMember(
                        createdByUser,
                        user,
                        _documentSession.Load<Team>(team.Id),
                        GetRoles(new[] { RoleNames.TeamMember })
                        );
                }

                var organisation = group
                    .AncestorGroups
                    .Where(x => x.GroupType == "organisations")
                    .FirstOrDefault();

                if (organisation != null)
                {
                    CreateNewMember(
                        createdByUser,
                        user,
                        _documentSession.Load<Organisation>(organisation.Id),
                        GetRoles(new[] { RoleNames.OrganisationMember })
                        );
                }
            }

            if (group.Team != null)
            {
                CreateNewMember(
                    createdByUser,
                    user,
                    group.Team,
                    GetRoles(command.Roles));

                var organisation = group
                    .AncestorGroups
                    .Where(x => x.GroupType == "organisations")
                    .FirstOrDefault();

                if (organisation != null)
                {
                    CreateNewMember(
                        createdByUser,
                        user,
                        _documentSession.Load<Organisation>(organisation.Id),
                        GetRoles(new[] { RoleNames.OrganisationMember })
                        );
                }
            }

            if (group.Organisation != null)
            {
                CreateNewMember(
                   createdByUser,
                   user,
                   group.Organisation,
                   GetRoles(command.Roles));
            }
        }

        private void CreateNewMember(
            User createdByUser,
            User user,
            Group group,
            IEnumerable<Role> roles)
        {
            Check.RequireNotNull(createdByUser, "createdByUser");
            Check.RequireNotNull(user, "user");
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(roles, "roles");

            _documentSession.Store(
                    new Member(
                        createdByUser,
                        user,
                        group,
                        roles));
        }

        private IEnumerable<Role> GetRoles(IEnumerable<string> roles)
        {
            return _documentSession.Query<Role>()
                .Where(x => x.Id.In(roles))
                .ToList();
        }

        #endregion

    }
}