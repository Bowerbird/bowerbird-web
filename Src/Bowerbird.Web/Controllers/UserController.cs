using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bowerbird.Core;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.Config;

namespace Bowerbird.Web.Controllers
{
    public class UserController : Controller
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IViewModelRepository _viewModelRepository;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public UserController(
            ICommandProcessor commandProcessor,
            IViewModelRepository viewModelRepository,
            IUserContext userContext)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(viewModelRepository, "viewModelRepository");
            Check.RequireNotNull(userContext, "userContext");

            _commandProcessor = commandProcessor;
            _viewModelRepository = viewModelRepository;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        [Authorize]
        public ActionResult Update()
        {
            //return View(_viewModelRepository.Load<IdInput, UserUpdate>(new IdInput(){ Id =  _userContext.GetAuthenticatedUserId() }));
            return View(_viewModelRepository.Load<IdInput, UserUpdate>(new IdInput(){ Id = _userContext.GetAuthenticatedUserId() }));
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult Update(UserUpdateInput userUpdateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserUpdate(userUpdateInput));

                return RedirectToAction("index", "home");
            }

            return View(_viewModelRepository.Load<UserUpdateInput, UserUpdate>(userUpdateInput));
        }

        private UserUpdateCommand MakeUserUpdate(UserUpdateInput userUpdateInput)
        {
            return new UserUpdateCommand()
                       {
                           FirstName = userUpdateInput.FirstName,
                           LastName = userUpdateInput.LastName,
                           Email = userUpdateInput.Email,
                           Description = userUpdateInput.Description
                       };
        }

        #endregion

    }
}
