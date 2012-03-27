/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Threading;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Web.Config;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Public
{
    public class HomeController : ControllerBase
    {

        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly ICommandProcessor _commandProcessor;

        #endregion

        #region Constructors

        public HomeController(
            IDocumentSession documentSession,
            ICommandProcessor commandProcessor)  
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            
            _documentSession = documentSession;
            _commandProcessor = commandProcessor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home", new {area = "members"});
            }

            return View();
        }

        #endregion      

    }
}