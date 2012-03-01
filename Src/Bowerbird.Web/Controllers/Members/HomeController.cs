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
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Web.Config;
using Bowerbird.Web.Hubs;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers.Members
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public HomeController(
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

        #region Methods

        [HttpGet]
        public ActionResult Index(HomeIndexInput homeIndexInput)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(MakeHomeIndex(homeIndexInput));
            }
            //return View(MakeHomeIndex(homeIndexInput));

            return View(new HomeIndex(){UserProfile = new UserProfile(){Id = User.Identity.Name}});
        }

        [HttpGet]
        public ActionResult ActivityTest()
        {
            return View();
        }

        private HomeIndex MakeHomeIndex(HomeIndexInput indexInput)
        {
            var groupMemberships = _documentSession
                .Query<GroupMember>()
                .Include(x => x.User.Id)
                .Where(x => x.User.Id == indexInput.UserId)
                .ToList();

            var user = _documentSession.Load<User>(indexInput.UserId);

            var homeIndex = new HomeIndex()
            {
                ProjectMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("projects/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                TeamMenu = groupMemberships
                .Where(x => x.Group.Id.Contains("teams/"))
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.Group.Id,
                        Name = x.Group.Name
                    }
                )
                .ToList(),

                UserProfile =
                    new UserProfile()
                    {
                        Id = user.Id,
                        Name = string.Format("{0} {1}", user.FirstName, user.LastName)
                    },

                WatchlistMenu = _documentSession
                .Query<Watchlist>()
                .Where(x => x.User.Id == _userContext.GetAuthenticatedUserId())
                .Select(x =>
                    new MenuItem()
                    {
                        Id = x.QuerystringJson,
                        Name = x.Name
                    })
                    .ToList()
            };

            return homeIndex;
        }

        #endregion
    }
}