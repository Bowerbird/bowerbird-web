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
using Raven.Client;
using System;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Core.Config;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var parentGroup = _documentSession.Load<dynamic>(command.TeamId);
            
            var project = new Project(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _avatarFactory.MakeDefaultAvatar(AvatarDefaultType.Project) : _documentSession.Load<MediaResource>(command.AvatarId),
                DateTime.UtcNow,
                parentGroup
                );

            _documentSession.Store(project);

            if (!string.IsNullOrEmpty(command.TeamId))
            {
                parentGroup.AddDescendant(project);
                _documentSession.Store(parentGroup);

                if(((Group)parentGroup).Ancestry.Any(x => x.GroupType == "organisation"))
                {
                    var grandParent = _documentSession.Load<Organisation>(((Group)parentGroup).Ancestry.Single(x => x.GroupType == "organisation").Id);
                    grandParent.AddDescendant(project);
                    _documentSession.Store(grandParent);
                }
            }

            var projectAdministrator = new Member(
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<User>(command.UserId),
                project,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.Equals("roles/projectadministrator") || x.Id.Equals("roles/projectmember"))
                    .ToList(),
                false);

            _documentSession.Store(projectAdministrator);

            var groupAssociation = new GroupAssociation(
                parentGroup,
                project,
                _documentSession.Load<User>(command.UserId),
                DateTime.UtcNow
                );

            _documentSession.Store(groupAssociation);
        }

        #endregion
    }
}