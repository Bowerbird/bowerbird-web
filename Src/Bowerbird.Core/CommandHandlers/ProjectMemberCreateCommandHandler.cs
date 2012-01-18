/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Raven.Client;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using System;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DomainModels.Members;

    #endregion

    public class ProjectMemberCreateCommandHandler : ICommandHandler<ProjectMemberCreateCommand>
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectMemberCreateCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectMemberCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectMember = new ProjectMember(
                _documentSession.Load<User>(command.CreatedByUserId),
                _documentSession.Load<Project>(command.ProjectId),
                _documentSession.Load<User>(command.UserId),
                _documentSession.Load<Role>(command.Roles)
                );

            _documentSession.Store(projectMember);
        }

        #endregion
		
    }
}