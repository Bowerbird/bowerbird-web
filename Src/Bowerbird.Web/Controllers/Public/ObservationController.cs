/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Web.ViewModels.Public;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.Controllers.Public
{
    #region Namespaces

    using System;
    using System.Web.Mvc;

    using Bowerbird.Core;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.Config;

    #endregion

    public class ObservationController : Controller
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IViewModelRepository _viewModelRepository;

        #endregion

        #region Constructors

        public ObservationController(
            ICommandProcessor commandProcessor,
            IViewModelRepository viewModelRepository
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(viewModelRepository, "viewModelRepository");

            _commandProcessor = commandProcessor;
            _viewModelRepository = viewModelRepository;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            var viewModel = _viewModelRepository.Load<IdInput, ObservationIndex>(idInput);

            if (Request.IsAjaxRequest())
            {
                return Json(viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }

        #endregion      
    }
}