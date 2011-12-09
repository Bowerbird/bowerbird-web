using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Commands;
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
            return new ObservationCreateCommand()
            {
                Title = observationCreateInput.Title,
                Latitude = observationCreateInput.Latitude,
                Longitude = observationCreateInput.Longitude,
                Address = observationCreateInput.Address
            };
        }

        #endregion      
      
    }
}
