using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Web.Factories
{
    public class ObservationViewFactory : IObservationViewFactory
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make()
        {
            return new
            {
                Title = string.Empty,
                ObservedOn = DateTime.UtcNow.ToString("d MMM yyyy"),
                Address = string.Empty,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = string.Empty,
                IsIdentificationRequired = false,
                AnonymiseLocation = false,
                Media = new ObservationMedia[] { },
                Projects = new string[] { }
            };
        }

        public object Make(Observation observation)
        {
            return new
            {
                observation.Id,
                observation.Title,
                ObservedOnDate = observation.ObservedOn.ToString("d MMM yyyy"),
                ObservedOnTime = observation.ObservedOn.ToShortTimeString(),
                observation.Address,
                observation.Latitude,
                observation.Longitude,
                observation.Category,
                observation.IsIdentificationRequired,
                observation.AnonymiseLocation,
                observation.Media,
                PrimaryMedia = observation.GetPrimaryMedia(),
                Projects = observation.Groups.Select(x => x.Group.Id)
            };
        }

        #endregion  
 
    }
}
