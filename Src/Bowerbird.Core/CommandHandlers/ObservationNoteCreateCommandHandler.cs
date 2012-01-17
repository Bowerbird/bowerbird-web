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

    public class ObservationNoteCreateCommandHandler : ICommandHandler<ObservationNoteCreateCommand>
    {
        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Observation> _observationRepository;
        private readonly IRepository<ObservationNote> _observationNoteRepository;

        #endregion

        #region Constructors

        public ObservationNoteCreateCommandHandler(
            IRepository<User> userRepository
            , IRepository<Observation> observationRepository
            , IRepository<ObservationNote> observationNoteRepository
            )
        {
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(observationRepository, "observationRepository");
            Check.RequireNotNull(observationNoteRepository, "observationNoteRepository");

            _userRepository = userRepository;
            _observationRepository = observationRepository;
            _observationNoteRepository = observationNoteRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationNoteCreateCommand command)
        {
            Check.RequireNotNull(command, "command");

            var observationNote = new ObservationNote(
                _userRepository.Load(command.UserId),
                _observationRepository.Load(command.ObservationId),
                command.CommonName,
                command.ScientificName,
                command.Taxonomy,
                command.Tags,
                command.Descriptions,
                command.References,
                command.Notes
                );

            _observationNoteRepository.Add(observationNote);
        }

        #endregion

    }
}