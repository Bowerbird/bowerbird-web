/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Posts;

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using System.Linq;

    using Commands;
    using DesignByContract;
    using DomainModels;
    using Repositories;

    #endregion

    public class ProjectPostUpdateCommandHandler : ICommandHandler<ProjectPostUpdateCommand>
    {
        #region Fields

        private readonly IRepository<ProjectPost> _projectPostRepository;
        private readonly IRepository<MediaResource> _mediaResourceRepository;
        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ProjectPostUpdateCommandHandler(
            IRepository<ProjectPost> projectPostRepository,
            IRepository<MediaResource> mediaResourceRepository,
            IRepository<User> userRepository
            )
        {
            Check.RequireNotNull(projectPostRepository, "projectPostRepository");
            Check.RequireNotNull(mediaResourceRepository, "mediaResourceRepository");
            Check.RequireNotNull(userRepository, "userRepository");

            _projectPostRepository = projectPostRepository;
            _mediaResourceRepository = mediaResourceRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ProjectPostUpdateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var projectPost = _projectPostRepository.Load(command.Id);

            projectPost.UpdateDetails(
                _userRepository.Load(command.UserId),
                command.Subject,
                command.Message,
                _mediaResourceRepository.Load(command.MediaResources).ToList()
                );

            _projectPostRepository.Add(projectPost);
        }

        #endregion

    }
}