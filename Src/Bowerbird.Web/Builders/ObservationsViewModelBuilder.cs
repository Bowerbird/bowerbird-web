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
using Nustache.Core;
using System.Collections;

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
                Media = observation.Media.Select(x => MakeObservationMediaItem(x, observation.GetPrimaryImage() == x)),
                PrimaryImage = MakeObservationMediaItem(observation.GetPrimaryImage(), true),
                Projects = observation.Groups.Select(x => x.Group.Id)
            };
        }

        private object MakeObservationMediaItem(ObservationMedia observationMedia, bool isPrimaryImage)
        {
            if(observationMedia.MediaResource.Type == "image")
            {
                return new
                    {
                        IsPrimaryImage = isPrimaryImage,
                        MediaResourceId = observationMedia.MediaResource.Id,
                        observationMedia.MediaResource.Type,
                        observationMedia.Description,
                        observationMedia.Licence,
                        CreatedByUser = observationMedia.MediaResource.CreatedByUser.Id,
                        UploadedOn = observationMedia.MediaResource.UploadedOn,
                        OriginalImage = observationMedia.MediaResource.Files["original"],
                        LargeImage = observationMedia.MediaResource.Files["large"],
                        MediumImage = observationMedia.MediaResource.Files["medium"],
                        SmallImage = observationMedia.MediaResource.Files["small"],
                        ThumbnailImage = observationMedia.MediaResource.Files["thumbnail"]
                    };
            }

            throw new NotImplementedException();
        }

        #endregion
    }
}