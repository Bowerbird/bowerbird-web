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
using Bowerbird.Core.VideoUtilities;
using Nustache.Core;
using System.Collections;

namespace Bowerbird.Web.Builders
{
    public class SightingViewModelBuilder : ISightingViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly ISightingViewFactory _sightingViewFactory;

        #endregion

        #region Constructors

        public SightingViewModelBuilder(
            IDocumentSession documentSession,
            ISightingViewFactory sightingViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(sightingViewFactory, "sightingViewFactory");

            _documentSession = documentSession;
            _sightingViewFactory = sightingViewFactory;
        }

        #endregion

        #region Methods

        public object BuildNewObservation()
        {
            return _sightingViewFactory.MakeObservation();
        }

        public object BuildSighting(string sightingId)
        {
            var result = _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.ContributionId == sightingId)
                .ToList()
                .First();

            return _sightingViewFactory.Make(result);
        }

        public object BuildGroupSightingList(string groupId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(groupId, "groupId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            RavenQueryStatistics stats;

            return _documentSession
                .Query<All_Contributions.Result, All_Contributions>()
                .AsProjection<All_Contributions.Result>()
                .Where(x => x.GroupIds.Any(y => y == groupId))
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_sightingViewFactory.Make)
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
                .Where(x => x.UserId == userId)
                .Statistics(out stats)
                .Skip(pagingInput.GetSkipIndex())
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(_sightingViewFactory.Make)
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}