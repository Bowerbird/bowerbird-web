/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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
    public class ProjectMemberDeleteCommandHandler : ICommandHandler<ProjectMemberDeleteCommand>
    {
        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public ProjectMemberDeleteCommandHandler(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(ProjectMemberDeleteCommand projectMemberDeleteCommand)
        {
            Check.RequireNotNull(projectMemberDeleteCommand, "projectMemberDeleteCommand");

            var projectMember = _documentSession.LoadProjectMember(projectMemberDeleteCommand.ProjectId, projectMemberDeleteCommand.UserId);

            _documentSession.Delete(projectMember);
        }

        #endregion
    }
}