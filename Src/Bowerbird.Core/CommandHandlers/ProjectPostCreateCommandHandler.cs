using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.CommandHandlers
{
    public class ProjectPostCreateCommandHandler : ICommandHandler<ProjectPostCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ProjectPost> _projectPostRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public ProjectPostCreateCommandHandler(
            IRepository<Project> projectRepository,
            IRepository<ProjectPost> projectPostRepository,
            IRepository<User> userRepository,
            IRepository<MediaResource> mediaResourceRepository
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