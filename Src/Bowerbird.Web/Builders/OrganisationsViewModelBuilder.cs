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

        public object BuildOrganisation(IdInput idInput)
        {
            Check.RequireNotNull(idInput, "idInput");

            return _organisationViewFactory.Make(_documentSession.Load<Organisation>(idInput.Id));
        }

        public object BuildOrganisationList(PagingInput pagingInput)
        {
            RavenQueryStatistics stats;


            var results = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Customize(x => x.WaitForNonStaleResults())
                .Include(x => x.Id)
                .Where(x => x.GroupType == "organisation")
                .Statistics(out stats)
                .Skip(pagingInput.Page)
                .Take(pagingInput.PageSize)
                .ToList()
                .Select(x => _organisationViewFactory.Make(x.Organisation));


            return new
            {
                pagingInput.Page,
                pagingInput.PageSize,
                Organisations = results.ToPagedList(
                    pagingInput.Page,
                    pagingInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        #endregion
    }
}