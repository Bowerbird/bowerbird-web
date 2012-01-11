using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectDeleteCommandHandler
    {
        #region Fields

        private IDefaultRepository<Project> _projectRepository;

        #endregion

        #region Constructors

        public ProjectDeleteCommandHandler(
            IDefaultRepository<Project> projectRepository
            )
        {
            Check.RequireNotNull(projectRepository, "projectRepository");

            _projectRepository = projectRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectDeleteCommand projectDeleteCommand)
        {
            Check.RequireNotNull(projectDeleteCommand, "projectDeleteCommand");

            _projectRepository.Remove(_projectRepository.Load(projectDeleteCommand.Id));
        }

        #endregion				
    }
}