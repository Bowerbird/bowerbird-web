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
using Bowerbird.Core.Indexes;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            // Get parent group
            Group parentGroup = null;

            if (!string.IsNullOrWhiteSpace(command.TeamId))
            {
                parentGroup = _documentSession
                    .Query<All_Groups.Result, All_Groups>()
                    .Where(x => x.GroupId == command.TeamId)
                    .ToList()
                    .First()
                    .Team;
            }
            else
            {
                parentGroup = _documentSession.Load<AppRoot>(Constants.AppRootId);
            }
            
            // Make project
            var project = new Project(
                _documentSession.Load<User>(command.UserId),
                command.Name,
                command.Description,
                command.Website,
                string.IsNullOrWhiteSpace(command.AvatarId) ? _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Project) : _documentSession.Load<MediaResource>(command.AvatarId),
                DateTime.UtcNow,
                parentGroup);
            _documentSession.Store(project);

            // Add administrator membership to creating user
            var user = _documentSession.Load<User>(command.UserId);
            user.AddMembership(
                user,
                project,
                _documentSession
                    .Query<Role>()
                    .Where(x => x.Id.In("roles/projectadministrator", "roles/projectmember"))
                    .ToList());
            _documentSession.Store(user);

            _documentSession.SaveChanges();
        }

        #endregion
    }
}