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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Controllers
{
    public class UserController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAvatarFactory _avatarFactory;

        #endregion

        #region Constructors

        public UserController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAvatarFactory avatarFactory
            )
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(avatarFactory, "avatarFactory");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _avatarFactory = avatarFactory;
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

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                _commandProcessor.Process(MakeUserUpdatePasswordCommand(accountChangePasswordInput));

                return RedirectToAction("index", "home");
            }

            return View("ChangePassword");
        }

        private UserUpdatePasswordCommand MakeUserUpdatePasswordCommand(AccountChangePasswordInput accountChangePasswordInput)
        {
            return new UserUpdatePasswordCommand()
                       {
                           UserId = _userContext.GetAuthenticatedUserId(),
                           Password = accountChangePasswordInput.Password
                       };
        }

        private UserUpdateCommand MakeUserUpdateCommand(UserUpdateInput userUpdateInput)
        {
            return new UserUpdateCommand()
                       {
                           FirstName = userUpdateInput.FirstName,
                           LastName = userUpdateInput.LastName,
                           Email = userUpdateInput.Email,
                           Description = userUpdateInput.Description,
                           AvatarId = userUpdateInput.AvatarId
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
                Description = user.Description,
                Avatar = _avatarFactory.GetAvatar(user)
            };
        }

        private UserUpdate MakeUserUpdate(UserUpdateInput userUpdateInput)
        {
            return new UserUpdate()
            {
                FirstName = userUpdateInput.FirstName,
                LastName = userUpdateInput.LastName,
                Email = userUpdateInput.Email,
                Description = userUpdateInput.Description,
                Avatar = _avatarFactory.GetAvatar(_documentSession.Load<User>(_userContext.GetAuthenticatedUserId()))
            };
        }

        #endregion

    }
}