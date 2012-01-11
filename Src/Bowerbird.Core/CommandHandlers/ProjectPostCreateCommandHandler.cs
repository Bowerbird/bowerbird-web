using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectPostCreateCommandHandler : ICommandHandler<ProjectPostCreateCommand>
    {
        #region Fields

        private readonly IDefaultRepository<User> _userRepository;
        private readonly IDefaultRepository<ProjectPost> _projectPostRepository;
        private readonly IDefaultRepository<Project> _projectRepository;
        private readonly IDefaultRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public ProjectPostCreateCommandHandler(
            IDefaultRepository<Project> projectRepository,
            IDefaultRepository<ProjectPost> projectPostRepository,
            IDefaultRepository<User> userRepository,
            IDefaultRepository<MediaResource> mediaResourceRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(projectPostRepository, "projectPostRepository");
            Check.RequireNotNull(projectRepository, "projectRepository");
            Check.RequireNotNull(mediaResourceRepository, "mediaResourceRepository");

            _userRepository = userRepository;
            _projectPostRepository = projectPostRepository;
            _projectRepository = projectRepository;
            _mediaResourceRepository = mediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostCreateCommand projectPostCreateCommand)
        {
            Check.RequireNotNull(projectPostCreateCommand, "projectPostCreateCommand");

            var projectPost = new ProjectPost(
                _projectRepository.Load(projectPostCreateCommand.ProjectId),
                _userRepository.Load(projectPostCreateCommand.UserId),
                projectPostCreateCommand.Timestamp,
                projectPostCreateCommand.Subject,
                projectPostCreateCommand.Message,
                _mediaResourceRepository.Load(projectPostCreateCommand.MediaResources).ToList()
                );

            _projectPostRepository.Add(projectPost);
        }

        #endregion
    }
}