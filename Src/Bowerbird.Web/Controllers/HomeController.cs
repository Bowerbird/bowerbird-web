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

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            if (_userContext.IsUserAuthenticated())
            {
                if (Request.IsAjaxRequest())
                {
                    return new JsonNetResult(GetClientUserContext());
                }

                return View("PrivateIndex");
            }

            return View("PublicIndex");
        }

        public ActionResult SetupTestData()
        {
            _commandProcessor.Process(new SetupTestDataCommand());

            return RedirectToAction("index");
        }

        #endregion
    }
}