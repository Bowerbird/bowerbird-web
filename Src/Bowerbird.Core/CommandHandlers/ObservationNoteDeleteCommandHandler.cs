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

    public class ObservationNoteDeleteCommandHandler : ICommandHandler<ObservationNoteDeleteCommand>
    {
        #region Fields

        private readonly IRepository<ObservationNote> _observationNoteRepository;

        #endregion

        #region Constructors

        public ObservationNoteDeleteCommandHandler(
            IRepository<ObservationNote> observationNoteRepository
            )
        {
            Check.RequireNotNull(observationNoteRepository, "observationNoteRepository");

            _observationNoteRepository = observationNoteRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationNoteDeleteCommand command)
        {
            Check.RequireNotNull(command, "command");
        }

        #endregion

    }
}