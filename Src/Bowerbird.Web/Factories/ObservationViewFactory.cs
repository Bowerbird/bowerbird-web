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
using System;

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

        public object Make()
        {
            return new
            {
                Title = string.Empty,
                ObservedOn = DateTime.Now.ToString("d MMM yyyy"),
                Address = string.Empty,
                Latitude = string.Empty,
                Longitude = string.Empty,
                Category = string.Empty,
                IsIdentificationRequired = false,
                AnonymiseLocation = false,
                Media = new ObservationMedia[] { },
                Projects = new string[] { }
            };
        }

        public object Make(Observation observation)
        {
            return new
            {
                Id = observation.ShortId(),
                Title = observation.Title,
                ObservedOn = observation.ObservedOn.ToString("d MMM yyyy"),
                Address = observation.Address,
                Latitude = observation.Latitude,
                Longitude = observation.Longitude,
                Category = observation.Category,
                IsIdentificationRequired = observation.IsIdentificationRequired,
                AnonymiseLocation = observation.AnonymiseLocation,
                Media = MakeObservationMediaItems(observation.Media),
                Projects =  observation.Groups.Select(x => x.GroupId)
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
                    x.MediaResource.Metadata,
                    x.MediaResource.Type,
                    CreatedByUser = x.MediaResource.CreatedByUser.Id,
                    UploadedOn = x.MediaResource.UploadedOn.ToString("d MMM yyyy"),
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