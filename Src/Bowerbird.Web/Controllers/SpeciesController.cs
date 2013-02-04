/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Queries;
using Bowerbird.Web.Config;
using Bowerbird.Core.ViewModels;

namespace Bowerbird.Web.Controllers
{
    public class SpeciesController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISpeciesViewModelQuery _speciesViewModelQuery;

        #endregion

        #region Constructors

        public SpeciesController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISpeciesViewModelQuery speciesViewModelQuery
        )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(speciesViewModelQuery, "speciesViewModelQuery");

            _messageBus = messageBus;
            _userContext = userContext;
            _speciesViewModelQuery = speciesViewModelQuery;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(SpeciesQueryInput speciesQueryInput, PagingInput pagingInput)
        {
            var viewModel = new
            {
                Species = _speciesViewModelQuery.BuildSpeciesList(speciesQueryInput, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "species",
                "list");
        }

        #endregion
    }
}