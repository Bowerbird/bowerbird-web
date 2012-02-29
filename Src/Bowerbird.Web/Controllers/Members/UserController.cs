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
using Bowerbird.Core.Services;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;
using System;

namespace Bowerbird.Web.Controllers.Members
{
    public class UserController : ControllerBase
    {

        #region Members

        private readonly ICommandProcessor _commandProcessor;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IMediaFilePathService _mediaFilePathService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructors

        public UserController(
            ICommandProcessor commandProcessor,
            IUserContext userContext,
            IDocumentSession documentSession,
            IMediaFilePathService mediaFilePathService,
            IConfigService configService)
        {
            Check.RequireNotNull(commandProcessor, "commandProcessor");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(mediaFilePathService, "mediaFilePathService");
            Check.RequireNotNull(configService, "configService");

            _commandProcessor = commandProcessor;
            _userContext = userContext;
            _documentSession = documentSession;
            _mediaFilePathService = mediaFilePathService;
            _configService = configService;
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
                Avatar = GetAvatar(user)
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
                Avatar = GetAvatar(_documentSession.Load<User>(_userContext.GetAuthenticatedUserId()))
            };
        }

        private Avatar GetAvatar(User user)
        {
            return new Avatar()
            {
                AltTag = string.Format("{0} {1}", user.FirstName,user.LastName),
                UrlToImage = user.Avatar != null ?
                    _mediaFilePathService.MakeMediaFileUri(user.Avatar.Id, "image", "avatar", user.Avatar.Metadata["metatype"]) :
                    _configService.GetDefaultAvatar("user")
            };
        }

        #endregion

    }
}
