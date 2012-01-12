using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
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