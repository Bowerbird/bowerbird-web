using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.Entities.DenormalisedReferences
{
    public class DenormalisedObservationReference
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string Title { get; set; }

        #endregion

        #region Methods

        public static implicit operator DenormalisedObservationReference(Observation observation)
        {
            Check.RequireNotNull(observation, "observation");

            return new DenormalisedObservationReference
            {
                Id = observation.Id,
                Title = observation.Title
            };
        }

        #endregion

    }
}