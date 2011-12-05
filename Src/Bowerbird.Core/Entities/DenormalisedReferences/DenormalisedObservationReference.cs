using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return new DenormalisedObservationReference
            {
                Id = observation.Id,
                Title = observation.Title
            };
        }

        #endregion

    }
}
