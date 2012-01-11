using System.Collections.Generic;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationCreateCommandHandler : ICommandHandler<ObservationCreateCommand>
    {

        #region Members

        private readonly IDefaultRepository<Observation> _observationRepository;
        private readonly IDefaultRepository<User> _userRepository;
        private readonly IDefaultRepository<MediaResource> _mediaResourceRepository;

        #endregion

        #region Constructors

        public ObservationCreateCommandHandler(
            IDefaultRepository<Observation> observationRepository,
            IDefaultRepository<User> userRepository,
            IDefaultRepository<MediaResource> mediaResourceRepository)
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
                _userRepository.Load(observationCreateCommand.UserId),
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
