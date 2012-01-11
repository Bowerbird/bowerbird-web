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
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;

    #endregion

    public class ObservationUpdateCommandHandler : ICommandHandler<ObservationUpdateCommand>
    {
        #region Members

        private readonly IDefaultRepository<Observation> _observationRepository;
        private readonly IDefaultRepository<User> _userRepsitory;
        private readonly IDefaultRepository<MediaResource> _mediaResourceRepsitory;

        #endregion

        #region Constructors

        public ObservationUpdateCommandHandler(
            IDefaultRepository<Observation> observationRepository,
            IDefaultRepository<User> userRepsitory,
            IDefaultRepository<MediaResource> mediaResourceRepsitory)
        {
            Check.RequireNotNull(observationRepository, "observationRepository");
            Check.RequireNotNull(userRepsitory, "userRepsitory");
            Check.RequireNotNull(mediaResourceRepsitory, "mediaResourceRepsitory");

            _observationRepository = observationRepository;
            _userRepsitory = userRepsitory;
            _mediaResourceRepsitory = mediaResourceRepsitory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(ObservationUpdateCommand observationUpdateCommand)
        {
            Check.RequireNotNull(observationUpdateCommand, "observationUpdateCommand");

            var observation = _observationRepository.Load(observationUpdateCommand.Id);

            observation.UpdateDetails(
                _userRepsitory.Load(observationUpdateCommand.UserId),
                observationUpdateCommand.Title,
                observationUpdateCommand.ObservedOn,
                observationUpdateCommand.Latitude,
                observationUpdateCommand.Longitude,
                observationUpdateCommand.Address,
                observationUpdateCommand.IsIdentificationRequired,
                observationUpdateCommand.ObservationCategory,
                _mediaResourceRepsitory.Load(observationUpdateCommand.MediaResources));

            _observationRepository.Add(observation);
        }

        #endregion      
    }
}