/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ProjectMemberDeleteCommandHandler : ICommandHandler<ProjectMemberDeleteCommand>
    {
        #region Fields

        private readonly IRepository<ProjectMember> _projectMemberRepository;
 
        #endregion

        #region Constructors

        public ProjectMemberDeleteCommandHandler(
            IRepository<ProjectMember> projectMemberRepository
            )
        {
            Check.RequireNotNull(projectMemberRepository, "projectMemberRepository");

            _projectMemberRepository = projectMemberRepository;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(ProjectMemberDeleteCommand projectMemberDeleteCommand)
        {
            Check.RequireNotNull(projectMemberDeleteCommand, "projectMemberDeleteCommand");

            _projectMemberRepository.Remove(_projectMemberRepository.Load(projectMemberDeleteCommand.ProjectId, projectMemberDeleteCommand.UserId));
        }

        #endregion
    }
}