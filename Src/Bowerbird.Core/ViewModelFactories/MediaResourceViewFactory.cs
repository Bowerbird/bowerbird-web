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
    public class MediaResourceViewFactory : IMediaResourceViewFactory
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(dynamic mediaResource)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = mediaResource.Id;
            viewModel.Key = mediaResource.Key;
            viewModel.MediaResourceType = mediaResource.MediaResourceType;
            viewModel.UploadedOn = mediaResource.UploadedOn;

            viewModel.User = new
                {
                    mediaResource.User.Id,
                    mediaResource.User.Name
                };

            viewModel.Metadata = mediaResource.Metadata;

            if (viewModel.MediaResourceType == Constants.MediaResourceTypes.Image)
            {
                viewModel.Image = mediaResource.Image;
            }
            if (viewModel.MediaResourceType == Constants.MediaResourceTypes.Video)
            {
                viewModel.Video = mediaResource.Video;
            }
            if (viewModel.MediaResourceType == Constants.MediaResourceTypes.Audio)
            {
                viewModel.Audio = mediaResource.Audio;
            }

            return viewModel;
        }

        #endregion  
 
    }
}
