/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Organisation Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
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

        #endregion

        #region Constructors

        public OrganisationViewModelBuilder(
            IDocumentSession documentSession,
            IMediaResourceFactory mediaResourceFactory,
            IUserViewFactory userViewFactory,
            IGroupViewFactory groupViewFactory)
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaResourceFactory, "mediaResourceFactory");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(groupViewFactory, "groupViewFactory");

            _documentSession = documentSession;
            _mediaResourceFactory = mediaResourceFactory;
            _userViewFactory = userViewFactory;
            _groupViewFactory = groupViewFactory;
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
                BackgroundId = string.Empty
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
                organisation.Background
            };
        }

        public dynamic BuildOrganisation(string organisationId)
        {
            Check.RequireNotNullOrWhitespace(organisationId, "organisationId");

            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .First(x => x.GroupId == organisationId);

            return _groupViewFactory.Make(organisation, true);
        }

        public object BuildOrganisationList(OrganisationsQueryInput organisationsQueryInput)
        {
            Check.RequireNotNull(organisationsQueryInput, "organisationsQueryInput");

            var query = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "organisation");

            return ExecuteQuery(organisationsQueryInput.Sort, organisationsQueryInput, query);
        }

        public object BuildUserOrganisationList(string userId, PagingInput pagingInput)
        {
            Check.RequireNotNullOrWhitespace(userId, "userId");
            Check.RequireNotNull(pagingInput, "pagingInput");

            var query = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.GroupType == "organisation" && x.UserIds.Any(y => y == userId));

            return ExecuteQuery("a-z", pagingInput, query);
        }

        private object ExecuteQuery(string sort, PagingInput pagingInput, IRavenQueryable<All_Groups.Result> query)
        {
            switch (sort.ToLower())
            {
                default:
                case "newest":
                    query = query.OrderByDescending(x => x.CreatedDateTime);
                    break;
                case "a-z":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "z-a":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case "oldest":
                    query = query.OrderBy(x => x.CreatedDateTime);
                    break;
            }

            RavenQueryStatistics stats;

            return query.Skip(pagingInput.GetSkipIndex())
                .Statistics(out stats)
                .Take(pagingInput.GetPageSize())
                .ToList()
                .Select(x => _groupViewFactory.Make(x, true))
                .ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults);
        }

        #endregion
    }
}