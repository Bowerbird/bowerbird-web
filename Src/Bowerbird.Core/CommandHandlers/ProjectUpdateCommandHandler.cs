using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectUpdateCommandHandler
    {
        #region Fields

        private IDefaultRepository<Project> _projectRepository;
        private IDefaultRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ProjectUpdateCommandHandler(
            IDefaultRepository<Project> projectRepository,
            IDefaultRepository<User> userRepository
            )
        {
            Check.RequireNotNull(projectRepository, "projectRepository");
            Check.RequireNotNull(userRepository, "userRepository");

            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectUpdateCommand projectUpdateCommand)
        {
            Check.RequireNotNull(projectUpdateCommand, "projectUpdateCommand");

            var project = _projectRepository.Load(projectUpdateCommand.Id);

            project.UpdateDetails(
                _userRepository.Load(projectUpdateCommand.UserId),
                projectUpdateCommand.Name,
                projectUpdateCommand.Description
                );

            _projectRepository.Add(project);
        }

        #endregion				
    }
}