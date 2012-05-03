/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System.Collections.Generic;
using System.Linq;
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

        public object Make(Observation observation)
        {
            return new
            {
                observation.Id,
                observation.Title,
                observation.ObservedOn,
                observation.Address,
                observation.Latitude,
                observation.Longitude,
                observation.ObservationCategory,
                observation.IsIdentificationRequired,
                ObservationMedia = MakeObservationMediaItems(observation.Media)
            };
        }

        private IEnumerable<object> MakeObservationMediaItems(IEnumerable<ObservationMedia> observationMedia)
        {
            return observationMedia.Select(x =>
                new 
                {
                    MediaResourceId = x.MediaResource.Id,
                    x.Description,
                    x.Licence,
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