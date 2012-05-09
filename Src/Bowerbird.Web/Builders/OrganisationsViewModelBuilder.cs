/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
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

        private readonly IOrganisationViewFactory _organisationViewFactory;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public OrganisationsViewModelBuilder(
            IOrganisationViewFactory organisationViewFactory,
            IDocumentSession documentSession
        )
        {
            Check.RequireNotNull(organisationViewFactory, "organisationViewFactory");
            Check.RequireNotNull(documentSession, "documentSession");

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

            var organisation = _documentSession
                .Query<All_Groups.Result, All_Groups>()
                .AsProjection<All_Groups.Result>()
                .Where(x => x.Id == idInput.Id)
                .FirstOrDefault();

            return _organisationViewFactory.Make(organisation);
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
                .Select(x => _organisationViewFactory.Make(x));


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