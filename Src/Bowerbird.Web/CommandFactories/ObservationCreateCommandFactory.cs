using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.CommandFactories
{
    public class ObservationCreateCommandFactory : ICommandFactory<ObservationCreateInput, ObservationCreateCommand>
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ObservationCreateCommand Make(ObservationCreateInput observationCreateInput)
        {
            Check.RequireNotNull(observationCreateInput, "observationCreateInput");

            return new ObservationCreateCommand()
            {
                Title = observationCreateInput.Title,
                Latitude = observationCreateInput.Latitude,
                Longitude = observationCreateInput.Longitude,
                Address = observationCreateInput.Address,
                IsIdentificationRequired = observationCreateInput.IsIdentificationRequired,
                MediaResources = observationCreateInput.MediaResources,
                ObservationCategory = observationCreateInput.ObservationCategory,
                ObservedOn = observationCreateInput.ObservedOn,
                Username = observationCreateInput.Username
            };
        }

        #endregion      
      
    }
}