using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class SightingViewFactory : ISightingViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;

        #endregion

        #region Constructors

        public SightingViewFactory(IUserViewFactory userViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");

            _userViewFactory = userViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeNewObservation(string projectId = null)
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
                Projects = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object MakeNewRecord(string projectId = null)
        {
            return new
            {
                ObservedOn = DateTime.UtcNow.ToString("d MMM yyyy"),
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = string.Empty,
                AnonymiseLocation = false,
                Projects = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object Make(All_Contributions.Result result)
        {
            return Make(result.Contribution as Sighting, result.User);
        }

        public object Make(Sighting sighting, User user)
        {
            if (sighting is Observation)
            {
                var observation = sighting as Observation;

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
                    observation.PrimaryMedia,
                    Projects = observation.Groups.Select(x => x.Group.Id),
                    User = _userViewFactory.Make(user),
                    observation.Discussion.Comments
                };
            }
            else
            {
                var record = sighting as Record;
                return new
                {
                    record.Id,
                    ObservedOnDate = record.ObservedOn.ToString("d MMM yyyy"),
                    ObservedOnTime = record.ObservedOn.ToShortTimeString(),
                    record.Latitude,
                    record.Longitude,
                    record.Category,
                    record.AnonymiseLocation,
                    Projects = record.Groups.Select(x => x.Group.Id),
                    User = _userViewFactory.Make(user),
                    record.Discussion.Comments
                };
            }
        }

        #endregion  
 
    }
}
