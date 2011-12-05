using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;

namespace Bowerbird.Core.CommandHandlers
{
    public class ObservationUpdateCommandHandler : ICommandHandler<ObservationUpdateCommand>
    {

        #region Members

        private readonly IRepository<Observation> _observationRepository;
        private readonly IRepository<User> _userRepsitory;

        #endregion

        #region Constructors

        public ObservationUpdateCommandHandler(
            IRepository<Observation> observationRepository,
            IRepository<User> userRepsitory)
        {
            _observationRepository = observationRepository;
            _userRepsitory = userRepsitory;
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
                _userRepsitory.Load(observationUpdateCommand.Username),
                observationUpdateCommand.Title,
                observationUpdateCommand.ObservedOn,
                observationUpdateCommand.Latitude,
                observationUpdateCommand.Longitude,
                observationUpdateCommand.Address,
                observationUpdateCommand.IsIdentificationRequired,
                observationUpdateCommand.ObservationCategory,
                null); // TODO: Load media resources

            _observationRepository.Add(observation);
        }

        #endregion      
      
    }
}
