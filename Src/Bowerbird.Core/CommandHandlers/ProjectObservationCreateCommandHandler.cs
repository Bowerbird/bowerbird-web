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

    public class ProjectObservationCreateCommandHandler
    {
        #region Fields

        private readonly IRepository<ProjectObservation> _projectObservationRepository;
        private readonly IRepository<User> _userRepository;
 
        #endregion

        #region Constructors

        public ProjectObservationCreateCommandHandler(
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

        public void Handle(ProjectObservationCreateCommand projectObservationCreateCommand)
        {
            Check.RequireNotNull(projectObservationCreateCommand, "projectObservationCreateCommand");
        }

        #endregion
    }
}