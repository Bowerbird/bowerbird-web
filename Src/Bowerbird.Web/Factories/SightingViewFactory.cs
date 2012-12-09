using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly ISightingNoteViewFactory _sightingNoteViewFactory;

        #endregion

        #region Constructors

        public SightingViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            ISightingNoteViewFactory sightingNoteViewFactory)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(sightingNoteViewFactory, "sightingNoteViewFactory");

            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _sightingNoteViewFactory = sightingNoteViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeNewObservation(string category = "", string projectId = "")
        {
            return new
            {
                Title = string.Empty,
                ObservedOn = DateTime.UtcNow,
                Address = string.Empty,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = category,
                IsIdentificationRequired = false,
                AnonymiseLocation = false,
                Media = new ObservationMedia[] { },
                ProjectIds = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object MakeNewRecord(string category = "", string projectId = "")
        {
            return new
            {
                ObservedOn = DateTime.UtcNow,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = category,
                AnonymiseLocation = false,
                ProjectIds = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public dynamic Make(All_Contributions.Result result)
        {
            return Make(result.Contribution as Sighting, result.User, result.Projects);
        }

        public dynamic Make(Sighting sighting, User user, IEnumerable<Project> projects)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sighting.Id;
            viewModel.ObservedOn = sighting.ObservedOn;
            viewModel.Latitude = sighting.Latitude;
            viewModel.Longitude = sighting.Longitude;
            viewModel.Category = sighting.Category;
            viewModel.AnonymiseLocation = sighting.AnonymiseLocation;
            viewModel.Projects = projects.Select(_groupViewFactory.Make);
            viewModel.User = _userViewFactory.Make(user);
            viewModel.Comments = sighting.Discussion.Comments;
            viewModel.ObservedOnDescription = sighting.ObservedOn.ToString("d MMM yyyy h:mm") + sighting.ObservedOn.ToString("tt").ToLower();
            //viewModel.Notes = sighting.Notes.Select(_sightingNoteViewFactory.Make);
            viewModel.CommentCount = sighting.Discussion.CommentCount;
            viewModel.ProjectCount = projects.Count();
            viewModel.NoteCount = sighting.Notes.Count();

            if (sighting is Observation)
            {
                var observation = sighting as Observation;

                viewModel.Title = observation.Title;
                viewModel.Address = observation.Address;
                viewModel.IsIdentificationRequired = observation.IsIdentificationRequired;
                viewModel.Media = observation.Media;
                viewModel.PrimaryMedia = observation.PrimaryMedia;
                viewModel.MediaCount = observation.Media.Count();
                viewModel.ShowMediaThumbnails = observation.Media.Count() > 1;
            }

            return viewModel;
        }

        #endregion  
 
    }
}
