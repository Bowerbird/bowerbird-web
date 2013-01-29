/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Castle.Components.DictionaryAdapter;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Factories;
using Bowerbird.Web.Factories;
using System.Collections.Generic;

namespace Bowerbird.Web.Builders
{
    public class OrganisationViewModelBuilder : IOrganisationViewModelBuilder
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public OrganisationViewModelBuilder(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory,
            IUserContext userContext)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
            _userContext = userContext;
        }

        #endregion

        #region Methods

        public object BuildCreateOrganisation()
        {
            return new
            {
                Name = string.Empty,
                Description = string.Empty,
                Website = string.Empty,
                Avatar = _mediaResourceFactory.MakeDefaultAvatarImage(AvatarDefaultType.Organisation),
                Background = _mediaResourceFactory.MakeDefaultBackgroundImage("organisation"),
                AvatarId = string.Empty,
                BackgroundId = string.Empty,
                Categories = new string [] {}
            };
        }

        public object BuildUpdateOrganisation(string organisationId)
        {
            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == organisationId)
                .Organisation;

            return new
            {
                organisation.Id,
                organisation.Name,
                organisation.Description,
                organisation.Website,
                AvatarId = organisation.Avatar.Id,
                BackgroundId = organisation.Background.Id,
                organisation.Avatar,
                organisation.Background,
                organisation.Categories
            };
        }

        public object BuildOrganisation(string organisationId)
        {
            Check.RequireNotNullOrWhitespace(organisationId, "organisationId");

            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == organisationId);

            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            return _groupViewFactory.Make(organisation.Group, authenticatedUser, true, organisation.SightingCount, organisation.UserCount, organisation.PostCount);
        }

        public object BuildOrganisationList(OrganisationsQueryInput organisationsQueryInput)
        {
            Check.RequireNotNull(organisationsQueryInput, "organisationsQueryInput");

            return ExecuteQuery(organisationsQueryInput);
        }

        public object BuildUserOrganisationList(string userId, PagingInput pagingInput)
        {
            throw new NotImplementedException();
            //Check.RequireNotNullOrWhitespace(userId, "userId");
            //Check.RequireNotNull(pagingInput, "pagingInput");

            //var query = _documentSession
            //    .Query<All_Groups.Result, All_Groups>()
            //    .AsProjection<All_Groups.Result>()
            //    .Where(x => x.GroupType == "organisation" && x.UserIds.Any(y => y == userId));

            //return ExecuteQuery("a-z", pagingInput, query);
        }

        //private object ExecuteQuery(string sort, PagingInput pagingInput, IQueryable<All_Groups.Result> query)
        //{
        //    RavenQueryStatistics stats;
        //    query = ((IRavenQueryable<All_Groups.Result>) query).Statistics(out stats);

        //    switch (sort.ToLower())
        //    {
        //        default:
        //        case "popular":
        //            query = query.OrderByDescending(x => x.UserCount).ThenBy(x => x.Name);
        //            break;
        //        case "newest":
        //            query = query.OrderByDescending(x => x.CreatedDateTime).ThenBy(x => x.Name);
        //            break;
        //        case "oldest":
        //            query = query.OrderBy(x => x.CreatedDateTime).ThenBy(x => x.Name);
        //            break;
        //        case "a-z":
        //            query = query.OrderBy(x => x.Name);
        //            break;
        //        case "z-a":
        //            query = query.OrderByDescending(x => x.Name);
        //            break;
        //    }

        //    return query.Skip(pagingInput.GetSkipIndex())
        //        .Take(pagingInput.GetPageSize())
        //        .ToList()
        //        .Select(x => _groupViewFactory.Make(x, true))
        //        .ToPagedList(
        //            pagingInput.Page,
        //            pagingInput.PageSize,
        //            stats.TotalResults);
        //}

        private object ExecuteQuery(OrganisationsQueryInput organisationsQueryInput)
        {
            RavenQueryStatistics stats;
            User authenticatedUser = null;

            if (_userContext.IsUserAuthenticated())
            {
                authenticatedUser = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
            }

            var query = _documentSession
                .Advanced
                .LuceneQuery<All_Groups.Result, All_Groups>()
                .Statistics(out stats)
                .SelectFields<All_Groups.Result>("GroupType", "GroupId", "CreatedDateTime", "UserCount", "SightingCount", "PostCount", "VoteCount")
                .WhereEquals("GroupType", "organisation");

            if (!string.IsNullOrWhiteSpace(organisationsQueryInput.Category))
            {
                query = query
                    .AndAlso()
                    .WhereIn("Categories", new[] { organisationsQueryInput.Category });
            }

            if (!string.IsNullOrWhiteSpace(organisationsQueryInput.Query))
            {
                var field = "AllFields";

                if (organisationsQueryInput.Field.ToLower() == "name")
                {
                    field = "Name";
                }
                if (organisationsQueryInput.Field.ToLower() == "description")
                {
                    field = "Description";
                }

                query = query
                    .AndAlso()
                    .Search(field, organisationsQueryInput.Query);
            }

            switch (organisationsQueryInput.Sort.ToLower())
            {
                default:
                case "popular":
                    query = query.AddOrder(x => x.UserCount, true).AddOrder(x => x.Name, false);
                    break;
                case "newest":
                    query = query.AddOrder(x => x.CreatedDateTime, true).AddOrder(x => x.Name, false);
                    break;
                case "a-z":
                    query = query.AddOrder(x => x.Name, false);
                    break;
                case "z-a":
                    query = query.AddOrder(x => x.Name, true);
                    break;
                case "oldest":
                    query = query.AddOrder(x => x.CreatedDateTime, false).AddOrder(x => x.Name, false);
                    break;
            }

            return query
                .Skip(organisationsQueryInput.GetSkipIndex())
                .Take(organisationsQueryInput.GetPageSize())
                .ToList()
                .Select(x => _groupViewFactory.Make(x.Group, authenticatedUser, true, x.SightingCount, x.UserCount, x.PostCount))
                .ToPagedList(
                    organisationsQueryInput.GetPage(),
                    organisationsQueryInput.GetPageSize(),
                    stats.TotalResults
                );
        }

        #endregion
    }
}