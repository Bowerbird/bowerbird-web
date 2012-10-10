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
using Bowerbird.Core.Infrastructure;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.Builders;
using System;
using System.Dynamic;

namespace Bowerbird.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        #region Members

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;

        #endregion

        #region Constructors

        public HomeController(
            IMessageBus messageBus,
            IUserContext userContext,
            IActivityViewModelBuilder activityViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _activityViewModelBuilder = activityViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
        }

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult PublicIndex()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.HomeHeader = true;
            
            return RestfulResult(
                viewModel,
                "home",
                "publicindex",
                null);
        }

        [HttpGet]
        [Authorize]
        public ActionResult PrivateIndex(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (!_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("PublicIndex");
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.User = _userViewModelBuilder.BuildUser(_userContext.GetAuthenticatedUserId());

            return RestfulResult(
                viewModel,
                "home",
                "privateindex",
                null,
                new Action<dynamic>(x => {
                    x.Model.Activities = _activityViewModelBuilder.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput);
                }));
        }

        #endregion
    }
}