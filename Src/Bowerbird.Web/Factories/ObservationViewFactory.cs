using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Web.ViewModels.Shared;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class ObservationViewFactory : IObservationViewFactory
    {

        #region Members

        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public ObservationViewFactory(
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ObservationView Make(Observation observation)
        {
            return new ObservationView()
            {
                Id = observation.Id,
                Title = observation.Title,
                ObservedOn = observation.ObservedOn,
                Address = observation.Address,
                Latitude = observation.Latitude,
                Longitude = observation.Longitude,
                ObservationCategory = observation.ObservationCategory,
                IsIdentificationRequired = observation.IsIdentificationRequired,
                ObservationMedia = MakeObservationMediaItems(observation.Media)
            };
        }

        private IEnumerable<ObservationMediaItem> MakeObservationMediaItems(IEnumerable<ObservationMedia> observationMedia)
        {
            return observationMedia.Select(x =>
                new ObservationMediaItem()
                {
                    MediaResourceId = x.MediaResource.Id,
                    Description = x.Description,
                    Licence = x.Licence,
                    OriginalImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "original"),
                    LargeImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "large"),
                    MediumImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "medium"),
                    SmallImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "small"),
                    ThumbnailImageUri = _mediaFilePathService.MakeMediaFileUri(x.MediaResource, "thumbnail")
                });
        }

        #endregion      
      
    }
}
