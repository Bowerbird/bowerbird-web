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

        public object MakeObservation()
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

        public object MakeObservationForProject(string projectId)
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
                Projects = new string[] { },
                ProjectId = projectId
            };
        }

        public object Make(All_Contributions.Result result)
        {
            return Make(result.Observation, result.User);
        }

        public object Make(Observation observation, User user)
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
                Projects = observation.Groups.Select(x => x.Group.Id),
                User = _userViewFactory.Make(user),
                observation.Discussion.Comments
            };
        }

        #endregion  
 
    }
}
