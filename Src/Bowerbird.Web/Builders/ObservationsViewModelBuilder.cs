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
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Builders
{
    public class ObservationsViewModelBuilder : IObservationsViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;

        #endregion

        #region Constructors

        public ObservationsViewModelBuilder(
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");

            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
        }

        #endregion

        #region Methods

        public object BuildObservation()
        {
            return MakeObservation();
        }

        public object BuildObservation(IdInput idInput)
        {
            var observation = _documentSession.Load<Observation>("observations/" + idInput.Id);

            return MakeObservation(observation);
        }

        public object BuildObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<Observation>()
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(MakeObservation)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }
        
        public object BuildGroupObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupId == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .Select(x => x.Observation)
                .ToList()
                .Select(MakeObservation)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        public object BuildUserObservationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == pagingInput.Id)
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .Select(x => x.Observation)
                .ToList()
                .Select(MakeObservation)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        private object MakeObservation()
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

        private object MakeObservation(Observation observation)
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
                Projects = observation.Groups.Select(x => x.GroupId)
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