/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using System.Linq;

namespace Bowerbird.Web.Builders
{
    public class OrganisationsViewModelBuilder : IOrganisationsViewModelBuilder
    {
        #region Fields

        private readonly IUserContext _userContext;
        private readonly IUsersGroupsQuery _usersGroupsQuery;
        private readonly IOrganisationViewFactory _organisationViewFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationsViewModelBuilder(
            IUserContext userContext,
            IUsersGroupsQuery usersGroupsQuery,
            IOrganisationViewFactory organisationViewFactory,
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(usersGroupsQuery, "usersGroupsQuery");
            Check.RequireNotNull(organisationViewFactory, "organisationViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");

            _userContext = userContext;
            _usersGroupsQuery = usersGroupsQuery;
            _organisationViewFactory = organisationViewFactory;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object BuildItem(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _organisationViewFactory.Make(_documentSession.Load<Organisation>(idInput.Id));
        }

        public object BuildList(OrganisationListInput listInput)
        {
            Check.RequireNotNull(listInput, "listInput");

            if(listInput.HasAddTeamPermission)
            {
                return BuildOrganisationsWhereUserHasAddTeamPermission();
            }

            return BuildOrganisations(listInput);
        }

        private object BuildOrganisations(OrganisationListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Organisation>()
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToList()
                .Select(x => _organisationViewFactory.Make(x));

            return new
            {
                listInput.Page,
                listInput.PageSize,
                Organisations = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private object BuildOrganisationsWhereUserHasAddTeamPermission()
        {
            var loggedInUserId = _userContext.GetAuthenticatedUserId();

            return _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x =>
                       x.Id.In(_usersGroupsQuery.GetUsersGroupsHavingPermission(loggedInUserId, "createteam")) &&
                       x.GroupType == "organisation"
                )
                .ToList()
                .Select(x => _organisationViewFactory.Make(x.Organisation));
        }

        #endregion
    }
}