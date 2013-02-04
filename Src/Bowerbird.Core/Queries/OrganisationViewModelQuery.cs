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
using Bowerbird.Core.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.DomainModelFactories;
using Bowerbird.Core.ViewModelFactories;

namespace Bowerbird.Core.Queries
{
    public class OrganisationViewModelQuery : IOrganisationViewModelQuery
    {
        #region Fields

        private readonly IDocumentSession _documentSession;
        private readonly IMediaResourceFactory _mediaResourceFactory;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IGroupViewFactory _groupViewFactory;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public OrganisationViewModelQuery(
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
                case "a-z":
                    query = query.AddOrder(x => x.SortName, false);
                    break;
                case "z-a":
                    query = query.AddOrder(x => x.SortName, true);
                    break;
                case "newest":
                    query = query.AddOrder(x => x.CreatedDateTime, true);
                    break;
                case "oldest":
                    query = query.AddOrder(x => x.CreatedDateTime, false);
                    break;
                default:
                case "popular":
                    query = query.AddOrder(x => x.UserCount, true);
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