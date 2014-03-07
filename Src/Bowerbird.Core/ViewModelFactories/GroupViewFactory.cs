using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.DomainModels;
using System.Dynamic;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class GroupViewFactory : IGroupViewFactory
    {
        #region Members

        private readonly IMediaResourceViewFactory _mediaResourceViewFactory;
        private readonly IMediaFilePathFactory _mediaFilePathFactory;

        #endregion

        #region Constructors

        public GroupViewFactory(
            IMediaResourceViewFactory mediaResourceViewFactory,
            IMediaFilePathFactory mediaFilePathFactory
        )
        {
            Check.RequireNotNull(mediaResourceViewFactory, "mediaResourceViewFactory");
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");

            _mediaResourceViewFactory = mediaResourceViewFactory;
            _mediaFilePathFactory = mediaFilePathFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(Group group, User authenticatedUser, bool fullDetails = false, int sightingCount = 0, int userCount = 0, int postCount = 0, IEnumerable<Observation> sampleObservations = null)
        {
            Check.RequireNotNull(group, "group");

            dynamic viewModel = new ExpandoObject();

            viewModel.Id = group.Id;
            viewModel.Name = group.Name;
            viewModel.GroupType = group.GroupType;

            if (group is IPublicGroup)
            {
                viewModel.Avatar = MakeAvatar(((IPublicGroup)group).Avatar);
                viewModel.CreatedBy = group.User.Id;
            }

            if (fullDetails)
            {
                viewModel.Created = group.CreatedDateTime.ToString("d MMM yyyy");
                viewModel.CreatedDateTimeOrder = group.CreatedDateTime.ToString("yyyyMMddHHmmss");

                if (group is IPublicGroup)
                {
                    viewModel.Background = ((IPublicGroup)group).Background;
                    viewModel.Website = ((IPublicGroup)group).Website;
                    viewModel.Description = ((IPublicGroup)group).Description;
                    viewModel.UserCount = userCount;
                    viewModel.PostCount = postCount;
                }
                if (group is Project)
                {
                    viewModel.SightingCount = sightingCount;
                    viewModel.Categories = ((Project)group).Categories;
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
                        viewModel.SampleObservations = new object[] {};
                    }
                }
                if (group is Organisation)
                {
                    viewModel.Categories = ((Organisation)group).Categories;
                }

                if (authenticatedUser != null)
                {
                    viewModel.IsMember = authenticatedUser.Memberships.Any(x => x.Group.Id == group.Id);
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
                if (avatar.Image.Original != null)
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