/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Queries;
using Bowerbird.Core.Services;
using Bowerbird.Core.ViewModelFactories;
using Bowerbird.Core.ViewModels;
using Bowerbird.Web.Infrastructure;
using NLog;
using Raven.Client;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Config;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Raven.Client.Linq;

namespace Bowerbird.Web.Controllers
{
    public class AccountController : ControllerBase
    {
        #region Members

        private Logger _logger = LogManager.GetLogger("AccountController");
        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IUserViewModelQuery _userViewModelQuery;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IActivityViewModelQuery _activityViewModelQuery;
        private readonly IPermissionManager _permissionManager;
        private readonly IDateTimeZoneService _dateTimeZoneService;

        #endregion

        #region Constructors

        public AccountController(
            IMessageBus messageBus,
            IUserContext userContext,
            IDocumentSession documentSession,
            IUserViewModelQuery userViewModelQuery,
            IActivityViewModelQuery activityViewModelQuery,
            IUserViewFactory userViewFactory,
            IPermissionManager permissionManager,
            IDateTimeZoneService dateTimeZoneService
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(activityViewModelQuery, "activityViewModelQuery");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(dateTimeZoneService, "dateTimeZoneService");

            _messageBus = messageBus;
            _userContext = userContext;
            _documentSession = documentSession;
            _userViewModelQuery = userViewModelQuery;
            _activityViewModelQuery = activityViewModelQuery;
            _userViewFactory = userViewFactory;
            _permissionManager = permissionManager;
            _dateTimeZoneService = dateTimeZoneService;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("privateindex", "home");
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.AccountLogin = new
                {
                    Email = _userContext.HasEmailCookieValue() ? _userContext.GetEmailCookieValue() : string.Empty,
                    ReturnUrl = returnUrl
                };
                
            return RestfulResult(
                viewModel,
                "account",
                "login");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            User user = null;
            dynamic viewModel = new ExpandoObject();

            if (ModelState.IsValid)
            {
                if (AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password, out user))
                {
                    _messageBus.Send(
                        new UserUpdateLastLoginCommand()
                            {
                                Email = accountLoginInput.Email
                            });

                    _userContext.SignUserIn(user.Id, user.Email, accountLoginInput.RememberMe);

                    if (Request.IsAjaxRequest())
                    {
                        viewModel.User = _userViewFactory.Make(user, user);

                        return RestfulResult(
                            viewModel,
                            "account",
                            "login");
                    }
                    else
                    {
                        return RedirectToAction("loggingin", new {returnUrl = accountLoginInput.ReturnUrl});
                    }
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;

                    ModelState.AddModelError("CredentialsInvalid", I18n.CredentialsInvalid);
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            viewModel.AccountLogin = accountLoginInput;

            return RestfulResult(
                viewModel,
                "account",
                "login");
        }

        [HttpGet]
        public ActionResult LoggingIn(string returnUrl)
        {
            // HACK: Wait here for a bit in case this is the first time the user is logging in after having
            // registered. THe waiting time is in case the indexes haven't updated to contain the new user. I 
            // realise this is a penalty on all users logging in, but its better than getting an error on 
            // sign up.
            Thread.Sleep(2500);

            if (!_userContext.HasEmailCookieValue())
            {
                // User attempted to login without cookies enabled
                return RedirectToAction("login");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _userContext.SignUserOut();

            // Even though we have signed out via FormsAuthentication, the session still contains the User.Identity until the 
            // HTTP response is fully written. In order for the User.Identity to be properly cleared, we have to do a full HTTP redirect 
            // rather than simply showing of the View in this action.
            return RedirectToAction("publicindex", "home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (_userContext.IsUserAuthenticated())
            {
                return RedirectToAction("privateindex", "home");
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.AccountRegister = new AccountRegisterInput();

            return RestfulResult(
                viewModel,
                "account",
                "register");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            dynamic viewModel = new ExpandoObject();

            if (ModelState.IsValid)
            {
                var user = _messageBus.Send<UserCreateCommand, User>(
                    new UserCreateCommand()
                        {
                            Name = accountRegisterInput.Name,
                            Email = accountRegisterInput.Email,
                            Password = accountRegisterInput.Password,
                            Timezone = Constants.DefaultTimezone,
                            Roles = new[] {"roles/globalmember"}
                        });

                // HACK: We wait here as long as possible to prevent errors due to indexes not yet having been re-indexed. Note that 
                // this still might not work all the time. 
                Thread.Sleep(5000);

                _userContext.SignUserIn(user.Id, accountRegisterInput.Email.ToLower(), true);

                // App login
                if (Request.IsAjaxRequest())
                {
                    viewModel.AccountRegister = accountRegisterInput;
                    viewModel.User = _userViewFactory.Make(user, null); // Required by API, might be able to remove later

                    return RestfulResult(
                        viewModel,
                        "account",
                        "register");
                }
                
                return RedirectToAction("loggingin");
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            viewModel.AccountRegister = accountRegisterInput;

            return RestfulResult(
                viewModel,
                "account",
                "register");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Profile()
        {
            if (Request.IsAjaxRequest())
            {
                dynamic viewModel = new ExpandoObject();
                viewModel.AuthenticatedUser = _userViewModelQuery.BuildAuthenticatedUser(_userContext.GetAuthenticatedUserId());

                return RestfulResult(
                    viewModel,
                    "account",
                    "profile");
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Notifications(ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelQuery.BuildNotificationActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                    }
                });
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Activity(ActivitiesQueryInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelQuery.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                    }
                });
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Update()
        {
            var userId = _userContext.GetAuthenticatedUserId();

            dynamic viewModel = new ExpandoObject();

            var user = _documentSession.Load<User>(userId);

            viewModel.User = _userViewModelQuery.BuildUpdateUser(userId);
            viewModel.TimezoneSelectList = GetTimeZones(null, user.Timezone);
            viewModel.LicenceSelectList = new[]
                {
                    new 
                    {
                        Text = "All Rights Reserved",
                        Value = " ",
                        Selected = user.DefaultLicence.Trim() == string.Empty
                    },
                    new 
                    {
                        Text = "Attribution",
                        Value = "BY",
                        Selected = user.DefaultLicence == "BY"
                    },
                    new 
                    {
                        Text = "Attribution-Share Alike",
                        Value = "BY-SA",
                        Selected = user.DefaultLicence == "BY-SA"
                    },
                    new 
                    {
                        Text = "Attribution-No Derivative Works",
                        Value = "BY-ND",
                        Selected = user.DefaultLicence == "BY-ND"
                    },
                    new 
                    {
                        Text = "Attribution-Noncommercial",
                        Value = "BY-NC",
                        Selected = user.DefaultLicence == "BY-NC"
                    },
                    new 
                    {
                        Text = "Attribution-Noncommercial-Share Alike",
                        Value = "BY-NC-SA",
                        Selected = user.DefaultLicence == "BY-NC-SA"
                    },
                    new 
                    {
                        Text = "Attribution-Noncommercial-No Derivatives",
                        Value = "BY-NC-ND",
                        Selected = user.DefaultLicence == "BY-NC-ND"
                    }
                };

            return RestfulResult(
                viewModel,
                "account",
                "update");
        }

        [HttpPut]
        [Authorize]
        public ActionResult Update(AccountUpdateInput updateInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserUpdateCommand()
                    {
                        Id = _userContext.GetAuthenticatedUserId(),
                        Name = updateInput.Name,
                        Email = updateInput.Email,
                        Description = updateInput.Description,
                        AvatarId = updateInput.AvatarId,
                        BackgroundId = updateInput.BackgroundId,
                        Timezone = updateInput.Timezone,
                        DefaultLicence = updateInput.DefaultLicence
                    });
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.User = updateInput;

            return RestfulResult(
                viewModel,
                "account",
                "update");
        }

        [HttpGet]
        public ActionResult UpdatePassword(AccountUpdatePasswordKeyInput accountUpdatePasswordKeyInput)
        {
            _logger.Debug("Getting /account/updatepassword. Key: {0}.", accountUpdatePasswordKeyInput.Key);

            // This action is used for both logged in users changing their passwords, as well as
            // non-authenticated users wanting to reset their passwords, having obtained a key (which 
            // is emailed to them)
            dynamic viewModel = new ExpandoObject();

            if (_userContext.IsUserAuthenticated())
            {
                _logger.Debug("Getting /account/updatepassword. User is authenticated, generating new key.");

                // User is authenticated, generate a key so that they can save their password
                var user = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());
                user.RequestPasswordUpdate(false);
                _documentSession.Store(user);
                _documentSession.SaveChanges();

                accountUpdatePasswordKeyInput.Key = user.ResetPasswordKey;
                ModelState.Clear();
            }
            else
            {
                // User is not logged in, so validate the passed Password Key byu checking if it exists in the DB. If not, user can't continue.

                _logger.Debug("Getting /account/updatepassword. Key: {0}. User is unauthenticated, checking passed key.", accountUpdatePasswordKeyInput.Key);

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int) System.Net.HttpStatusCode.BadRequest;
                }
            }

            viewModel.AccountUpdatePassword = accountUpdatePasswordKeyInput;

            return RestfulResult(
                viewModel,
                "account",
                "updatepassword");
        }

        [HttpPost]
        public ActionResult UpdatePassword(AccountUpdatePasswordInput accountUpdatePasswordInput)
        {
            _logger.Debug("Posting /account/updatepassword. Key: {0}, NewPassword: {1}", accountUpdatePasswordInput.Key, accountUpdatePasswordInput.NewPassword);

            // This action is used for both logged in users changing their passwords, as well as
            // non-authenticated users wanting to reset their passwords, having obtained a key (which 
            // is emailed to them)
            dynamic viewModel = new ExpandoObject();

            if (ModelState.IsValid)
            {
                _logger.Debug("Posting /account/updatepassword. Key is valid");

                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                        {
                            ResetPasswordKey = accountUpdatePasswordInput.Key,
                            Password = accountUpdatePasswordInput.NewPassword
                        });

                if (Request.IsAjaxRequest())
                {
                    viewModel.AccountUpdatePassword = new {};

                    return RestfulResult(
                        viewModel,
                        "account",
                        "updatepassword");
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                _logger.Debug("Posting /account/updatepassword. Key is invalid");

                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            viewModel.AccountUpdatePassword = accountUpdatePasswordInput;

            return RestfulResult(
                viewModel,
                "account",
                "updatepassword");
        }

        [HttpGet]
        public ActionResult RequestPasswordUpdate()
        {
            dynamic viewModel = new ExpandoObject();
            viewModel.RequestPasswordReset = new { };

            return RestfulResult(
                viewModel,
                "account",
                "requestpasswordupdate");
        }

        [HttpPost]
        public ActionResult RequestPasswordUpdate(AccountRequestPasswordUpdateInput accountRequestPasswordUpdateInput)
        {
            dynamic viewModel = new ExpandoObject();

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserRequestPasswordResetCommand()
                        {
                            Email = accountRequestPasswordUpdateInput.Email
                        });

                if (Request.IsAjaxRequest())
                {
                    viewModel.AccountRequestPasswordUpdate = accountRequestPasswordUpdateInput;

                    return RestfulResult(
                        viewModel,
                        "account",
                        "requestpasswordupdate");
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            }

            viewModel.AccountRequestPasswordUpdate = accountRequestPasswordUpdateInput;

            return RestfulResult(
                viewModel,
                "account",
                "requestpasswordupdate");
        }

        [HttpPost]
        [Authorize]
        public ActionResult CloseCallToAction(string name)
        {
            _messageBus.Send(
                new UserUpdateCallToActionCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    Name = name
                });

            dynamic viewModel = new ExpandoObject();

            return RestfulResult(
                viewModel,
                "account",
                "closecalltoaction");
        }

        private bool AreCredentialsValid(string email, string password, out User user)
        {
            user = null;

            var result = _documentSession
                .Query<All_Users.Result, All_Users>()
                .AsProjection<All_Users.Result>()
                .Where(x => x.Email == email)
                .ToList()
                .FirstOrDefault();

            if(result != null)
            {
                user = result.User;
            }

            return user != null && user.ValidatePassword(password);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateVote(string id, string subId, string contributionType, string subContributionType, int score)
        {
            if (score > 1)
                score = 1;

            if (score < -1)
                score = -1;

            _messageBus.Send(
                new VoteUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    ContributionId = contributionType + "/" + id,
                    Score = score,
                    SubContributionId = !string.IsNullOrWhiteSpace(subContributionType) ? subContributionType + "/" + subId : null
                });

            if (Request.IsAjaxRequest())
            {
                dynamic viewModel = new ExpandoObject();

                return RestfulResult(
                    viewModel,
                    "account",
                    "updatevote");
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateFavourite(string id)
        {
            string observationId = VerbosifyId<Observation>(id);

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new FavouriteUpdateCommand()
                {
                    UserId = _userContext.GetAuthenticatedUserId(),
                    SightingId = observationId
                });

            if (Request.IsAjaxRequest())
            {
                dynamic viewModel = new ExpandoObject();

                return RestfulResult(
                    viewModel,
                    "account",
                    "updatefavourite");
            }
            else
            {
                return Redirect("/");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateFollowUser(string id)
        {
            string userId = VerbosifyId<User>(id);

            if (!ModelState.IsValid)
            {
                return JsonFailed();
            }

            _messageBus.Send(
                new UserFollowUpdateCommand()
                {
                    FollowerUserId = _userContext.GetAuthenticatedUserId(),
                    FolloweeUserId = userId
                });

            if (Request.IsAjaxRequest())
            {
                dynamic viewModel = new ExpandoObject();

                return RestfulResult(
                    viewModel,
                    "account",
                    "updatefollowuser");
            }
            else
            {
                return Redirect("/");
            }
        }

        private object GetTimeZones(string countryCode, string existingTimezone)
        {
            return _dateTimeZoneService.GetTimeZones(countryCode).Select(x =>
                                                                  new
                                                                      {
                                                                          Value = x.Key,
                                                                          Text = x.Value,
                                                                          Selected = x.Key == existingTimezone
                                                                      });

        }

        #endregion
    }
}