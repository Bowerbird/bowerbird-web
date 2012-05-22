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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.Builders;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult PublicIndex()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.PublicIndex);
        }

        [HttpGet]
        public ActionResult PrivateIndex()
        {
            if (!_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("PublicIndex");
            }

            ViewBag.Model = new
            {
                Stream = true,
                StreamItems = new object [] {}
            };

            ViewBag.PrerenderedView = "home"; // HACK: Need to rethink this

            return View();
        }

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            var data = new
            {
                Model = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return new JsonNetResult(data);
        }

        #endregion
    }
}