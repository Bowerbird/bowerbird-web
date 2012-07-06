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
    public class SightingViewModelBuilder : ISightingViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public SightingViewModelBuilder(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Methods

        public object BuildNewObservation()
        {
            return MakeObservation();
        }

        public object BuildSighting(string sightingId)
        {
            return MakeObservation(_documentSession.Load<Observation>(sightingId));
        }

        public object BuildGroupSightingList(string groupId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupId == groupId)
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .Select(x => x.Observation)
                .ToList()
                .Select(MakeObservation)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        public object BuildUserSightingList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Include(x => x.UserId)
                .Where(x => x.UserId == userId)
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
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
                ObservedOn = DateTime.UtcNow.ToString("d MMM yyyy"),
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
                observation.Id,
                Title = observation.Title,
                ObservedOn = observation.ObservedOn.ToString("d MMM yyyy"),
                Address = observation.Address,
                Latitude = observation.Latitude,
                Longitude = observation.Longitude,
                Category = observation.Category,
                IsIdentificationRequired = observation.IsIdentificationRequired,
                AnonymiseLocation = observation.AnonymiseLocation,
                observation.Media,
                PrimaryMedia = observation.GetPrimaryMedia(),
                Projects = observation.Groups.Select(x => x.Group.Id)
            };
        }

        #endregion
    }
}