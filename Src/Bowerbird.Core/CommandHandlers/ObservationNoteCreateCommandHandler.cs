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

    public class ObservationNoteCreateCommandHandler : ICommandHandler<ObservationNoteCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ObservationNote> _observationNoteRepository;

        #endregion

        #region Constructors

        public ObservationNoteCreateCommandHandler(
            IRepository<User> userRepository
            , IRepository<ObservationNote> observationNoteRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(observationNoteRepository, "observationNoteRepository");

            _userRepository = userRepository;
            _observationNoteRepository = observationNoteRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationNoteCreateCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}