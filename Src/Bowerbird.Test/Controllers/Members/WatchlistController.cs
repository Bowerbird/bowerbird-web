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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Raven.Client;
using ControllerBase = Bowerbird.Web.Controllers.ControllerBase;

namespace Bowerbird.Test.Controllers.Members
{
    public class WatchlistController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IDocumentSession _documentSession;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public WatchlistController(
            ICommandProcessor commandProcessor,
            IDocumentSession documentSession,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _documentSession = documentSession;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        //[HttpGet]
        //[Authorize]
        //public ActionResult List(WatchlistListInput listInput)
        //{
        //    if (listInput.ProjectId == null)
        //    {
        //        return Json(MakeWatchlistList(listInput), JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(MakeWatchlistList(listInput), JsonRequestBehavior.AllowGet);
        //}

        //private object MakeWatchlistList(WatchlistListInput listInput)
        //{
        //    throw new System.NotImplementedException();
        //}

        #endregion
    }
}