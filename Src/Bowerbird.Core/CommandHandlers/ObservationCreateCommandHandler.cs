using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationCreateCommandHandler : ICommandHandler<ObservationCreateCommand>
    {

        #region Members

        private readonly IRepository<Observation> _observationRepository;
        private readonly IRepository<User> _userRepository;

        #endregion

        #region Constructors

        public ObservationCreateCommandHandler(
            IRepository<Observation> observationRepository,
            IRepository<User> userRepository)
        {
            _observationRepository = observationRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCreateCommand observationCreateCommand)
        {
            Check.RequireNotNull(observationCreateCommand, "observationCreateCommand");

            var observation = new Observation(
                _userRepository.Load("users/" + observationCreateCommand.Username),
                observationCreateCommand.Title,
                observationCreateCommand.ObservedOn,
                observationCreateCommand.Latitude,
                observationCreateCommand.Longitude,
                observationCreateCommand.Address,
                observationCreateCommand.IsIdentificationRequired,
                observationCreateCommand.ObservationCategory,
                new List<MediaResource>()); // TODO: Load media resources

            _observationRepository.Add(observation);
        }

        #endregion      
      
    }
}
