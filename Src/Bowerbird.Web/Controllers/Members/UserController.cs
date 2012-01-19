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
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;

namespace Bowerbird.Web.Controllers.Members
{
    public class UserController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserController(
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
        [Authorize]
        public ActionResult Update()
        {
            return View(MakeUserUpdate());
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult Update(UserUpdateInput userUpdateInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserUpdateCommand(userUpdateInput));

                return RedirectToAction("index", "home");
            }

            return View(MakeUserUpdate(userUpdateInput));
        }

        private UserUpdateCommand MakeUserUpdateCommand(UserUpdateInput userUpdateInput)
        {
            return new UserUpdateCommand()
                       {
                           FirstName = userUpdateInput.FirstName,
                           LastName = userUpdateInput.LastName,
                           Email = userUpdateInput.Email,
                           Description = userUpdateInput.Description
                       };
        }

        private UserUpdate MakeUserUpdate()
        {
            var user = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            return new UserUpdate()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Description = user.Description
            };
        }

        private UserUpdate MakeUserUpdate(UserUpdateInput userUpdateInput)
        {
            return new UserUpdate()
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
