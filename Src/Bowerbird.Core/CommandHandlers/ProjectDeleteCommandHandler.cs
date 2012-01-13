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

    public class ProjectDeleteCommandHandler
    {
        #region Fields

        private IRepository<Project> _projectRepository;

        #endregion

        #region Constructors

        public ProjectDeleteCommandHandler(
            IRepository<Project> projectRepository
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