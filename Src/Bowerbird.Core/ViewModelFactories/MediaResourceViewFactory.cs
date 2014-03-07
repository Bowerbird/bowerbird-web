using System.Dynamic;
using System.Linq;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.ViewModelFactories
{
    public class MediaResourceViewFactory : IMediaResourceViewFactory
    {

        #region Members

        private readonly IMediaFilePathFactory _mediaFilePathFactory;

        #endregion

        #region Constructors

        public MediaResourceViewFactory(
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

        public object Make(MediaResource mediaResource)
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Id = mediaResource.Id;
            viewModel.Key = mediaResource.Key;
            viewModel.MediaResourceType = mediaResource.MediaResourceType;
            viewModel.UploadedOn = mediaResource.UploadedOn;

            viewModel.User = new
                {
                    mediaResource.CreatedByUser.Id,
                    mediaResource.CreatedByUser.Name
                };

            if (mediaResource.Metadata != null)
            {
                viewModel.Metadata = mediaResource.Metadata.Select(x => new
                    {
                        x.Key,
                        x.Value
                    });
            }

            if (mediaResource is ImageMediaResource)
            {
                var imageMediaResource = mediaResource as ImageMediaResource;
                viewModel.Image = new ExpandoObject();
                    
                if (imageMediaResource.Image.Original != null)
                {
                    var uri = _mediaFilePathFactory.MakeMediaUri(imageMediaResource.Image.Original.Uri);

                    viewModel.Image.Original = new
                        {
                            imageMediaResource.Image.Original.ExifData,
                            imageMediaResource.Image.Original.Filename,
                            imageMediaResource.Image.Original.Height,
                            imageMediaResource.Image.Original.MimeType,
                            imageMediaResource.Image.Original.Size,
                            Uri = uri,
                            imageMediaResource.Image.Original.Width
                        };
                }
                if (imageMediaResource.Image.Square50 != null) viewModel.Image.Square50 = MakeDerivedFile(imageMediaResource.Image.Square50);
                if (imageMediaResource.Image.Square100 != null) viewModel.Image.Square100 = MakeDerivedFile(imageMediaResource.Image.Square100);
                if (imageMediaResource.Image.Square200 != null) viewModel.Image.Square200 = MakeDerivedFile(imageMediaResource.Image.Square200);
                if (imageMediaResource.Image.Constrained240 != null) viewModel.Image.Constrained240 = MakeDerivedFile(imageMediaResource.Image.Constrained240);
                if (imageMediaResource.Image.Constrained480 != null) viewModel.Image.Constrained480 = MakeDerivedFile(imageMediaResource.Image.Constrained480);
                if (imageMediaResource.Image.Constrained600 != null) viewModel.Image.Constrained600 = MakeDerivedFile(imageMediaResource.Image.Constrained600);
                if (imageMediaResource.Image.Full640 != null) viewModel.Image.Full640 = MakeDerivedFile(imageMediaResource.Image.Full640);
                if (imageMediaResource.Image.Full800 != null) viewModel.Image.Full800 = MakeDerivedFile(imageMediaResource.Image.Full800);
                if (imageMediaResource.Image.Full1024 != null) viewModel.Image.Full1024 = MakeDerivedFile(imageMediaResource.Image.Full1024);
                if (imageMediaResource.Image.Small != null) viewModel.Image.Small = MakeDerivedFile(imageMediaResource.Image.Small);
                if (imageMediaResource.Image.Large != null) viewModel.Image.Large = MakeDerivedFile(imageMediaResource.Image.Large);
                    
            }
            if (mediaResource is VideoMediaResource)
            {
                var videoMediaResource = mediaResource as VideoMediaResource;
                viewModel.Video = new ExpandoObject();

                if (videoMediaResource.Video.Original != null)
                {
                    viewModel.Video.Original = new
                    {
                        videoMediaResource.Video.Original.Height,
                        videoMediaResource.Video.Original.Provider,
                        videoMediaResource.Video.Original.ProviderData,
                        videoMediaResource.Video.Original.Uri,
                        videoMediaResource.Video.Original.VideoId,
                        videoMediaResource.Video.Original.Width
                    };
                }
                if (videoMediaResource.Video.OriginalImage != null)
                {
                    viewModel.Video.OriginalImage = new
                    {
                        videoMediaResource.Video.OriginalImage.ExifData,
                        videoMediaResource.Video.OriginalImage.Filename,
                        videoMediaResource.Video.OriginalImage.Height,
                        videoMediaResource.Video.OriginalImage.MimeType,
                        videoMediaResource.Video.OriginalImage.Size,
                        videoMediaResource.Video.OriginalImage.Uri,
                        videoMediaResource.Video.OriginalImage.Width
                    };
                }
                if (videoMediaResource.Video.Square50 != null) viewModel.Video.Square50 = MakeDerivedFile(videoMediaResource.Video.Square50);
                if (videoMediaResource.Video.Square100 != null) viewModel.Video.Square100 = MakeDerivedFile(videoMediaResource.Video.Square100);
                if (videoMediaResource.Video.Square200 != null) viewModel.Video.Square200 = MakeDerivedFile(videoMediaResource.Video.Square200);
                if (videoMediaResource.Video.Constrained240 != null) viewModel.Video.Constrained240 = MakeDerivedFile(videoMediaResource.Video.Constrained240);
                if (videoMediaResource.Video.Constrained480 != null) viewModel.Video.Constrained480 = MakeDerivedFile(videoMediaResource.Video.Constrained480);
                if (videoMediaResource.Video.Constrained600 != null) viewModel.Video.Constrained600 = MakeDerivedFile(videoMediaResource.Video.Constrained600);
                if (videoMediaResource.Video.Full640 != null) viewModel.Video.Full640 = MakeDerivedFile(videoMediaResource.Video.Full640);
                if (videoMediaResource.Video.Full800 != null) viewModel.Video.Full800 = MakeDerivedFile(videoMediaResource.Video.Full800);
                if (videoMediaResource.Video.Full1024 != null) viewModel.Video.Full1024 = MakeDerivedFile(videoMediaResource.Video.Full1024);
            }
            if (mediaResource is AudioMediaResource)
            {
                var audioMediaResource = mediaResource as AudioMediaResource;
                viewModel.Audio = new ExpandoObject();

                if (audioMediaResource.Audio.Original != null)
                {
                    viewModel.Audio.Original = new
                    {
                        audioMediaResource.Audio.Original.MimeType
                    };
                }

                if (audioMediaResource.Audio.Square50 != null) viewModel.Audio.Square50 = MakeDerivedFile(audioMediaResource.Audio.Square50);
                if (audioMediaResource.Audio.Square100 != null) viewModel.Audio.Square100 = MakeDerivedFile(audioMediaResource.Audio.Square100);
                if (audioMediaResource.Audio.Square200 != null) viewModel.Audio.Square200 = MakeDerivedFile(audioMediaResource.Audio.Square200);
                if (audioMediaResource.Audio.Constrained240 != null) viewModel.Audio.Constrained240 = MakeDerivedFile(audioMediaResource.Audio.Constrained240);
                if (audioMediaResource.Audio.Constrained480 != null) viewModel.Audio.Constrained480 = MakeDerivedFile(audioMediaResource.Audio.Constrained480);
                if (audioMediaResource.Audio.Constrained600 != null) viewModel.Audio.Constrained600 = MakeDerivedFile(audioMediaResource.Audio.Constrained600);
                if (audioMediaResource.Audio.Full640 != null) viewModel.Audio.Full640 = MakeDerivedFile(audioMediaResource.Audio.Full640);
                if (audioMediaResource.Audio.Full800 != null) viewModel.Audio.Full800 = MakeDerivedFile(audioMediaResource.Audio.Full800);
                if (audioMediaResource.Audio.Full1024 != null) viewModel.Audio.Full1024 = MakeDerivedFile(audioMediaResource.Audio.Full1024);
            }

            return viewModel;
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