using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectPostDeleteCommandHandler : ICommandHandler<ProjectPostDeleteCommand>
    {
        #region Fields

        private readonly IDefaultRepository<ProjectPost> _projectPostRepository;

        #endregion

        #region Constructors

        public ProjectPostDeleteCommandHandler(
            IDefaultRepository<ProjectPost> projectPostRepository
            )
        {
            Check.RequireNotNull(projectPostRepository, "projectPostRepository");

            _projectPostRepository = projectPostRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostDeleteCommand projectPostDeleteCommand)
        {
            Check.RequireNotNull(projectPostDeleteCommand, "projectPostDeleteCommand");

            _projectPostRepository.Remove(_projectPostRepository.Load(projectPostDeleteCommand.Id));
        }

        #endregion
    }
}