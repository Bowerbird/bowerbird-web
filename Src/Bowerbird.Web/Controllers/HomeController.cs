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
using System;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;

        #endregion

        #region Constructors

        public HomeController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IActivityViewModelBuilder activityViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _activityViewModelBuilder = activityViewModelBuilder;
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
        [Authorize]
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

            ViewBag.PrerenderedView = "home";

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Activity(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = _activityViewModelBuilder.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                });
            }

            return HttpNotFound();
        }

        #endregion
    }
}