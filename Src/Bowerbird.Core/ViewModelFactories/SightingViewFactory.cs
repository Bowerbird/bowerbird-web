using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class SightingViewFactory : ISightingViewFactory
    {

        #region Members

        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IMediaResourceViewFactory _mediaResourceViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public SightingViewFactory(
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IMediaResourceViewFactory mediaResourceViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(mediaResourceViewFactory, "mediaResourceViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _mediaResourceViewFactory = mediaResourceViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object MakeCreateObservation(string category = "", string projectId = "")
        {
            return new
            {
                Title = string.Empty,
                ObservedOn = DateTime.UtcNow,
                Address = string.Empty,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = category,
                AnonymiseLocation = false,
                Media = new ObservationMedia[] { },
                ProjectIds = string.IsNullOrWhiteSpace(projectId) ? new string[] { } : new string[] { projectId }
            };
        }

        public object MakeCreateRecord(string category = "", string projectId = "")
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

        public object Make(Sighting sighting, User user, IEnumerable<Group> projects, User authenticatedUser)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = sighting.Id;
            viewModel.CreatedOn = sighting.CreatedOn;
            viewModel.ObservedOn = sighting.ObservedOn;
            viewModel.Latitude = sighting.Latitude;
            viewModel.Longitude = sighting.Longitude;
            viewModel.Category = sighting.Category;
            viewModel.AnonymiseLocation = sighting.AnonymiseLocation;
            viewModel.Projects = projects.Where(x => x.GroupType =="project").Select(y => _groupViewFactory.Make(y, authenticatedUser));
            viewModel.User = _userViewFactory.Make(user, authenticatedUser);
            viewModel.ObservedOnDescription = sighting.ObservedOn.ToString("d MMM yyyy");
            viewModel.CreatedOnDescription = sighting.CreatedOn.ToString("d MMM yyyy");
            viewModel.CommentCount = sighting.Discussion.CommentCount;
            viewModel.ProjectCount = projects.Count(x => x.GroupType == "project");
            viewModel.NoteCount = sighting.Notes.Count();
            viewModel.IdentificationCount = sighting.Identifications.Count();
            viewModel.FavouritesCount = sighting.Groups.Count(x => x.Group.GroupType == "favourites");
            viewModel.TotalVoteScore = sighting.Votes.Sum(x => x.Score);

            // Current user-specific properties
            if (authenticatedUser != null)
            {
                var userId = authenticatedUser.Id;
                var favouritesId = authenticatedUser.Memberships.Single(x => x.Group.GroupType == "favourites").Group.Id;
                
                viewModel.UserVoteScore = sighting.Votes.Any(x => x.User.Id == userId) ? sighting.Votes.Single(x => x.User.Id == userId).Score : 0;
                viewModel.UserFavourite = sighting.Groups.Any(x => x.Group.Id == favouritesId);
                viewModel.IsOwner = sighting.User.Id == authenticatedUser.Id;
            }

            if (sighting is Observation)
            {
                var observation = sighting as Observation;

                viewModel.Title = observation.Title;
                viewModel.Address = observation.Address;
                viewModel.Media = observation.Media.Select(MakeObservationMedia);
                viewModel.PrimaryMedia = MakeObservationMedia(observation.PrimaryMedia);
                viewModel.MediaCount = observation.Media.Count();
                viewModel.ShowMediaThumbnails = observation.Media.Count() > 1;
            }

            return viewModel;
        }

        private object MakeObservationMedia(ObservationMedia observationMedia)
        {
            return new
            {
                observationMedia.Description,
                observationMedia.IsPrimaryMedia,
                observationMedia.Licence,
                MediaResource = _mediaResourceViewFactory.Make(observationMedia.MediaResource)
            };
        }

        #endregion  
 
    }
}
