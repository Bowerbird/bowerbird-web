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

namespace Bowerbird.Core.CommandHandlers
{
    #region Namespaces

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ProjectCreateCommandHandler : ICommandHandler<ProjectCreateCommand>
    {
        #region Fields

        private IRepository<Project> _projectRepository;
        private IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ProjectCreateCommandHandler(
            IRepository<Project> projectRepository,
            IRepository<User> userRepository
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

        public void Handle(ProjectCreateCommand projectCreateCommand)
        {
            Check.RequireNotNull(projectCreateCommand, "projectCreateCommand");

            var project = new Project(
                _userRepository.Load(projectCreateCommand.UserId),
                projectCreateCommand.Name,
                projectCreateCommand.Description
                );

            _projectRepository.Add(project);
        }

        #endregion
		
    }
}