using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;
using System.Dynamic;
using Bowerbird.Core.Events;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModelFactories
{
    public class ActivityViewFactory : IActivityViewFactory
    {
        #region Members

        private readonly IMediaFilePathFactory _mediaFilePathFactory;

        #endregion

        #region Constructors

        public ActivityViewFactory(
           IMediaFilePathFactory mediaFilePathFactory
           )
        {
            Check.RequireNotNull(mediaFilePathFactory, "mediaFilePathFactory");

            _mediaFilePathFactory = mediaFilePathFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Activity MakeActivity(
            IDomainEvent domainEvent,
            string type,
            DateTime created,
            string description,
            IEnumerable<dynamic> groups,
            string contributionId = (string)null,
            string subContributionId = (string)null)
        {
            return new Activity(
                type,
                created,
                description,
                new
                {
                    domainEvent.User.Id,
                    domainEvent.User.Name,
                    Avatar = MakeAvatar(domainEvent.User.Avatar)
                },
                groups,
                contributionId,
                subContributionId);
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