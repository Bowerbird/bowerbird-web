using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
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
        private readonly IRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public ObservationCreateCommandHandler(
            IRepository<Observation> observationRepository,
            IRepository<User> userRepository,
            IRepository<MediaResource> mediaResourceRepository)
        {
            Check.RequireNotNull(observationRepository, "observationRepository");
            Check.RequireNotNull(userRepository, "userRepository");
            Check.RequireNotNull(mediaResourceRepository, "mediaResourceRepository");

            _observationRepository = observationRepository;
            _userRepository = userRepository;
            _mediaResourceRepository = mediaResourceRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationCreateCommand observationCreateCommand)
        {
            Check.RequireNotNull(observationCreateCommand, "observationCreateCommand");

            var observation = new Observation(
                _userRepository.Load(observationCreateCommand.Username),
                observationCreateCommand.Title,
                observationCreateCommand.ObservedOn,
                observationCreateCommand.Latitude,
                observationCreateCommand.Longitude,
                observationCreateCommand.Address,
                observationCreateCommand.IsIdentificationRequired,
                observationCreateCommand.ObservationCategory,
                observationCreateCommand.MediaResources.IsNotNullAndHasItems()
                    ? _mediaResourceRepository.Load(observationCreateCommand.MediaResources)
                    : new List<MediaResource>());

            _observationRepository.Add(observation);
        }

        #endregion      
      
    }
}
