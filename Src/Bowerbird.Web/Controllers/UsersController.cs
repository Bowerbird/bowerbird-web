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
using Bowerbird.Web.Builders;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    [Restful]
    public class UsersController : ControllerBase
    {
        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IUserViewModelBuilder _viewModelBuilder;
        private readonly IStreamItemsViewModelBuilder _streamItemsViewModelBuilder;

        #endregion

        #region Constructors

        public UsersController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IUserViewModelBuilder userViewModelBuilder,
            IStreamItemsViewModelBuilder streamItemsViewModelBuilder
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(streamItemsViewModelBuilder, "streamItemsViewModelBuilder");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _viewModelBuilder = userViewModelBuilder;
            _streamItemsViewModelBuilder = streamItemsViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Stream(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.Stream);
        }

        [HttpGet]
        public ActionResult Observations(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.Observations);
        }

        [HttpGet]
        public ActionResult Posts(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.Posts);
        }

        [HttpGet]
        public ActionResult Following(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.Following);
        }

        [HttpGet]
        public ActionResult Followers(PagingInput pagingInput)
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.Followers);
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Model = new
            {
                User = _viewModelBuilder.BuildItem(pagingInput.Id),
                StreamItems = _streamItemsViewModelBuilder.BuildHomeStreamItems(pagingInput)
            };

            return View(Form.About);
        }

        [HttpGet]
        public ActionResult Explore(PagingInput pagingInput)
        {
            ViewBag.UserList = _viewModelBuilder.BuildList(pagingInput);

            return View(Form.List);
        }

        [HttpGet]
        public ActionResult GetOne(IdInput idInput)
        {
            return Json(_viewModelBuilder.BuildItem(idInput));
        }

        [HttpGet]
        public ActionResult GetMany(PagingInput pagingInput)
        {
            return Json(_viewModelBuilder.BuildList(pagingInput));
        }

        /// <summary>
        /// Placeholder Method: Keeping Restful Convention
        /// </summary>
        [HttpGet]
        public ActionResult CreateForm()
        {
            return RedirectToAction("Register", "Account");
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateForm(IdInput idInput)
        {
            ViewBag.User = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Update);
        }

        /// <summary>
        /// Placeholder Method: Keeping Restful Convention
        /// </summary>
        [HttpGet]
        [Authorize]
        public ActionResult DeleteForm(IdInput idInput)
        {
            ViewBag.User = _viewModelBuilder.BuildItem(idInput);

            return View(Form.Delete);
        }

        /// <summary>
        /// Placeholder Method: Keeping Restful Convention
        /// </summary>
        [HttpPost]
        [Authorize]
        public ActionResult Create()
        {
            return HttpNotFound();
        }

        [HttpPut]
        [Authorize]
        [Transaction]
        public ActionResult Update(UserUpdateInput userUpdateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(
                    new UserUpdateCommand()
                    {
                        FirstName = userUpdateInput.FirstName,
                        LastName = userUpdateInput.LastName,
                        Email = userUpdateInput.Email,
                        Description = userUpdateInput.Description,
                        AvatarId = userUpdateInput.AvatarId
                    });

                return RedirectToAction("index", "home");
            }

            ViewBag.User = new
            {
                userUpdateInput.AvatarId,
                userUpdateInput.Description,
                userUpdateInput.Email,
                userUpdateInput.FirstName,
                userUpdateInput.LastName
            };

            return View(Form.Update);
        }

        /// <summary>
        /// Placeholder Method: Keeping Restful Convention
        /// </summary>
        [HttpDelete]
        [Authorize]
        public ActionResult Delete()
        {
            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View(Form.ChangePassword);
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Password = accountChangePasswordInput.Password
                    });

                return RedirectToAction("index", "home");
            }

            return View(Form.ChangePassword);
        }

        #endregion
    }
}