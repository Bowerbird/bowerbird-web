using System.Collections.Generic;
using System.Drawing.Printing;
using System.Dynamic;
using System.Linq;
using System.Web.UI.WebControls;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client.Linq.Indexing;

namespace Bowerbird.Core.ViewModelFactories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Members

        private readonly IMediaFilePathFactory _mediaFilePathFactory;
        private readonly IMediaResourceViewFactory _mediaResourceViewFactory;

        #endregion

        #region Constructors

        public UserViewFactory(
           IMediaFilePathFactory mediaFilePathFactory,
        IMediaResourceViewFactory mediaResourceViewFactory
           )
        {
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");
            Check.RequireNotNull(mediaResourceViewFactory, "mediaResourceViewFactory");

            _mediaFilePathFactory = mediaFilePathFactory;
            _mediaResourceViewFactory = mediaResourceViewFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(User user, User authenticatedUser, bool fullDetails = false, int? sightingCount = 0, IEnumerable<Observation> sampleObservations = null, int? followerCount = 0)
        {
            Check.RequireNotNull(user, "user");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = user.Id;
            viewModel.Avatar = MakeAvatar(user.Avatar);
            viewModel.Name = user.Name;
            viewModel.LatestActivity = user.SessionLatestActivity;
            viewModel.LatestHeartbeat = user.SessionLatestHeartbeat;

            if (fullDetails)
            {
                viewModel.Joined = user.Joined.ToString("d MMM yyyy");
                viewModel.Description = user.Description;
                viewModel.ProjectCount = user.Memberships.Where(x => x.Group.GroupType == "project").Count();
                viewModel.OrganisationCount = user.Memberships.Where(x => x.Group.GroupType == "organisation").Count();
                viewModel.SightingCount = sightingCount;

                if (sampleObservations != null)
                {
                    viewModel.SampleObservations = sampleObservations.Select(x =>
                                                                    new
                                                                    {
                                                                        x.Id,
                                                                        Media = MakePrimaryMedia(x.PrimaryMedia)
                                                                    });
                }
                else
                {
                    viewModel.SampleObservations = new object[] { };
                }

                viewModel.FollowingCount = user.FollowingUsers.Count();
                viewModel.FollowerCount = followerCount;

                if (authenticatedUser != null)
                {
                    if (authenticatedUser.Id == user.Id)
                    {
                        viewModel.IsFollowing = false;
                        viewModel.IsFollowed = false;
                    }
                    else
                    {
                        viewModel.IsFollowing = authenticatedUser.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == user.UserProject.Id);
                        viewModel.IsFollowed = user.Memberships.Any(x => x.Group.GroupType == "userproject" && x.Group.Id == authenticatedUser.UserProject.Id);
                    }
                }
            }

            return viewModel;
        }

        private dynamic MakePrimaryMedia(ObservationMedia observationMedia)
        {
            dynamic primaryMedia = new ExpandoObject();

            primaryMedia.Description = observationMedia.Description;
            primaryMedia.Licence = observationMedia.Licence;
            primaryMedia.IsPrimaryMedia = observationMedia.IsPrimaryMedia;
            primaryMedia.MediaResource = _mediaResourceViewFactory.Make(observationMedia.MediaResource);

            return primaryMedia;
        }

        private dynamic MakeAvatar(MediaResource mediaResource)
        {
            var avatar = mediaResource as ImageMediaResource;

            dynamic viewModelAvatar = new ExpandoObject();
            viewModelAvatar.Image = new ExpandoObject();

            if (avatar != null && avatar.Image != null) 
            {
                if(avatar.Image.Original != null)
                {
                    var uri = _mediaFilePathFactory.MakeMediaUri(avatar.Image.Original.Uri);

                    viewModelAvatar.Image.Original = new
                    {
                        avatar.Image.Original.ExifData,
                        avatar.Image.Original.Filename,
                        avatar.Image.Original.Height,
                        avatar.Image.Original.MimeType,
                        avatar.Image.Original.Size,
                        Uri = uri,
                        avatar.Image.Original.Width        
                    };
                }

                if (avatar.Image.Square50 != null) viewModelAvatar.Image.Square50 = MakeDerivedFile(avatar.Image.Square50);
                if (avatar.Image.Square100 != null) viewModelAvatar.Image.Square100 = MakeDerivedFile(avatar.Image.Square100);
                if (avatar.Image.Square200 != null) viewModelAvatar.Image.Square200 = MakeDerivedFile(avatar.Image.Square200);
            }
            
            return viewModelAvatar;
        }

        private object MakeDerivedFile(DerivedMediaResourceFile derivedMediaResourceFile)
        {
            var uri = _mediaFilePathFactory.MakeMediaUri(derivedMediaResourceFile.Uri);
            
            return new
            {
                derivedMediaResourceFile.Height,
                Uri = uri,
                derivedMediaResourceFile.Width
            };
        }

        #endregion   
    }
}