/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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

    using System.Linq;

    using Commands;
    using DesignByContract;
    using DomainModels;
    using Repositories;

    #endregion

    public class ProjectUpdateCommandHandler
    {
        #region Fields

        private IRepository<Project> _projectRepository;
        private IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ProjectUpdateCommandHandler(
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