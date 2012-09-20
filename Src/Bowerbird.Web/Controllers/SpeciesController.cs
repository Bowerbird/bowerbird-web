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
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Controllers
{
    public class SpeciesController : ControllerBase
    {
        #region Fields

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly ISpeciesViewModelBuilder _speciesViewModelBuilder;

        #endregion

        #region Constructors

        public SpeciesController(
            IMessageBus messageBus,
            IUserContext userContext,
            ISpeciesViewModelBuilder speciesViewModelBuilder
        )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(speciesViewModelBuilder, "speciesViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _speciesViewModelBuilder = speciesViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(SpeciesQueryInput query, PagingInput pagingInput)
        {
            var viewModel = new
            {
                Species = _speciesViewModelBuilder.BuildSpeciesList(query, pagingInput)
            };

            return RestfulResult(
                viewModel,
                "species",
                "list");
        }

        #endregion
    }
}