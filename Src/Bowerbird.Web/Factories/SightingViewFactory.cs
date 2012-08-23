using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class SightingViewFactory : ISightingViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;

        #endregion

        #region Constructors

        public SightingViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
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
                ObservedOn = DateTime.UtcNow,
                Address = string.Empty,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = string.Empty,
                IsIdentificationRequired = false,
                AnonymiseLocation = false,
                Media = new ObservationMedia[] { },
                ProjectIds = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object MakeNewRecord(string projectId = null)
        {
            return new
            {
                ObservedOn = DateTime.UtcNow,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = string.Empty,
                AnonymiseLocation = false,
                ProjectIds = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object Make(All_Contributions.Result result)
        {
            return Make(result.Contribution as Sighting, result.User, result.Projects);
        }

        public object Make(Sighting sighting, User user, IEnumerable<Project> projects)
        {
            if (sighting is Observation)
            {
                var observation = sighting as Observation;

                return new
                {
                    observation.Id,
                    observation.Title,
                    observation.ObservedOn,
                    observation.Address,
                    observation.Latitude,
                    observation.Longitude,
                    observation.Category,
                    observation.IsIdentificationRequired,
                    observation.AnonymiseLocation,
                    observation.Media,
                    observation.PrimaryMedia,
                    Projects = projects.Select(_groupViewFactory.Make),
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
                    record.ObservedOn,
                    record.Latitude,
                    record.Longitude,
                    record.Category,
                    record.AnonymiseLocation,
                    Projects = projects.Select(_groupViewFactory.Make),
                    User = _userViewFactory.Make(user),
                    record.Discussion.Comments
                };
            }
        }

        #endregion  
 
    }
}
