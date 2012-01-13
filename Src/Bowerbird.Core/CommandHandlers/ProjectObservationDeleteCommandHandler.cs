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

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;

    #endregion

    public class ProjectObservationDeleteCommandHandler
    {
        #region Fields

        private readonly IRepository<ProjectObservation> _projectObservationRepository;
        private readonly IRepository<User> _userRepository;
 
        #endregion

        #region Constructors

        public ProjectObservationDeleteCommandHandler(
            IRepository<ProjectObservation> projectObservationRepository
            , IRepository<User> userRepository
            )
        {
            Check.RequireNotNull(projectObservationRepository, "projectObservationRepository");
            Check.RequireNotNull(userRepository, "userRepository");

            _projectObservationRepository = projectObservationRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion
         
        #region Methods

        public void Handle(ProjectObservationDeleteCommand projectObservationDeleteCommand)
        {
            Check.RequireNotNull(projectObservationDeleteCommand, "projectObservationDeleteCommand");
        }

        #endregion
    }
}