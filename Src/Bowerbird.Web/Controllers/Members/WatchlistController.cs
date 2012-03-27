/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Paging;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers.Members
{
    public class WatchlistController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public WatchlistController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(WatchlistListInput listInput)
        {
            return Json(MakeWatchlistList(listInput), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index(IdInput idInput)
        {
            if (Request.IsAjaxRequest())

                return Json(MakeIndex(idInput));

            return View(MakeIndex(idInput));
        }

        [Transaction]
        [Authorize]
        [HttpPost]
        public ActionResult Create(WatchlistCreateInput createInput)
        {
            if (!_userContext.HasGlobalPermission(PermissionNames.CreateWatchlist))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeCreateCommand(createInput));

            return Json("success");
        }

        [Transaction]
        [HttpPut]
        public ActionResult Update(WatchlistUpdateInput updateInput)
        {
            if (!_userContext.HasPermissionToUpdate<Watchlist>(updateInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeUpdateCommand(updateInput));

            return Json("Success");
        }

        [Transaction]
        [HttpDelete]
        public ActionResult Delete(IdInput deleteInput)
        {
            if (!_userContext.HasPermissionToDelete<Watchlist>(deleteInput.Id))
            {
                return HttpUnauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Json("Failure");
            }

            _commandProcessor.Process(MakeDeleteCommand(deleteInput));

            return Json("success");
        }

        private WatchlistIndex MakeIndex(IdInput idInput)
        {
            var watchlist = _documentSession.Load<Watchlist>(idInput.Id);

            return new WatchlistIndex()
            {
                Watchlist = watchlist
            };
        }

        private WatchlistList MakeWatchlistList(WatchlistListInput listInput)
        {
            RavenQueryStatistics stats;

            var results = _documentSession
                .Query<Watchlist>()
                .Where(x => x.User.Id == listInput.UserId)
                .Statistics(out stats)
                .Skip(listInput.Page)
                .Take(listInput.PageSize)
                .ToArray(); // HACK: Due to deferred execution (or a RavenDB bug) need to execute query so that stats actually returns TotalResults - maybe fixed in newer RavenDB builds

            return new WatchlistList()
            {
                Page = listInput.Page,
                PageSize = listInput.PageSize,
                Watchlists = results.ToPagedList(
                    listInput.Page,
                    listInput.PageSize,
                    stats.TotalResults,
                    null)
            };
        }

        private WatchlistCreateCommand MakeCreateCommand(WatchlistCreateInput createInput)
        {
            return new WatchlistCreateCommand()
            {
                JsonQuerystring = createInput.JsonQuerystring,
                Name = createInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private WatchlistDeleteCommand MakeDeleteCommand(IdInput deleteInput)
        {
            return new WatchlistDeleteCommand()
            {
                Id = deleteInput.Id,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        private WatchlistUpdateCommand MakeUpdateCommand(WatchlistUpdateInput updateInput)
        {
            return new WatchlistUpdateCommand()
            {
                JsonQuerystring = updateInput.JsonQuerystring,
                Name = updateInput.Name,
                UserId = _userContext.GetAuthenticatedUserId()
            };
        }

        #endregion
    }
}