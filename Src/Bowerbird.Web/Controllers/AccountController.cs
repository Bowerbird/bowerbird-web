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
using System.Dynamic;
using System.IO;
using System.Linq;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Internationalisation;
using Bowerbird.Core.Queries;
using Bowerbird.Core.Services;
using Bowerbird.Core.ViewModelFactories;
using Bowerbird.Core.ViewModels;
using Bowerbird.Web.Infrastructure;
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

        private readonly IMessageBus _messageBus;
        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IAccountViewModelQuery _accountViewModelQuery;
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
            IAccountViewModelQuery accountViewModelQuery,
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
            Check.RequireNotNull(accountViewModelQuery, "accountViewModelQuery");
            Check.RequireNotNull(userViewModelQuery, "userViewModelQuery");
            Check.RequireNotNull(activityViewModelQuery, "activityViewModelQuery");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(permissionManager, "permissionManager");
            Check.RequireNotNull(dateTimeZoneService, "dateTimeZoneService");

            _messageBus = messageBus;
            _userContext = userContext;
            _documentSession = documentSession;
            _accountViewModelQuery = accountViewModelQuery;
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

            //ViewBag.AccountLogin = _accountViewModelQuery.MakeAccountLogin(returnUrl);
            //ViewBag.IsStaticLayout = true;

            //return View(Form.Login);

            dynamic viewModel = new ExpandoObject();
            viewModel.AccountLogin = _accountViewModelQuery.MakeAccountLogin(returnUrl);

            return RestfulResult(
                viewModel,
                "account",
                "login");
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

//            User user = null;

//            if (ModelState.IsValid &&
//                AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password, out user))
//            {
//                _messageBus.Send(
//                    new UserUpdateLastLoginCommand()
//                    {
//                        Email = accountLoginInput.Email
//                    });
                
//                _userContext.SignUserIn(user.Id, user.Email, accountLoginInput.RememberMe);

//#if !JS_COMBINE_MINIFY
//                DebugToClient("SERVER: Logged In Successfully as " + accountLoginInput.Email);
//#endif

//                if(Request.IsAjaxRequest())
//                {
//                    dynamic viewModel = new ExpandoObject();
//                    viewModel.User = _userViewFactory.Make(user, user);

//                    return RestfulResult(
//                        viewModel,
//                        string.Empty,
//                        string.Empty,
//                        null,
//                        null);
//                }

//                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
//            }

//            ModelState.AddModelError("", "");

//            ViewBag.AccountLogin = _accountViewModelQuery.MakeAccountLogin(accountLoginInput);
//            ViewBag.IsStaticLayout = true;

//            return View(Form.Login);
            User user = null;
            dynamic viewModel = null;

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

#if !JS_COMBINE_MINIFY
                    DebugToClient("SERVER: Logged In Successfully as " + accountLoginInput.Email);
#endif

                    if (Request.IsAjaxRequest())
                    {
                        viewModel = new ExpandoObject();
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

            viewModel = new ExpandoObject();
            viewModel.AccountLogin = _accountViewModelQuery.MakeAccountLogin(accountLoginInput);

            return RestfulResult(
                viewModel,
                "account",
                "login");
        }

        [HttpGet]
        public ActionResult LoggingIn(string returnUrl)
        {
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
            return RedirectToAction("logoutsuccess");
        }

        [HttpGet]
        public ActionResult LogoutSuccess()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.LogoutSuccess);
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
        [Transaction]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            //if (ModelState.IsValid)
            //{
            //    _messageBus.Send(
            //        new UserCreateCommand()
            //        {
            //            Name = accountRegisterInput.Name,
            //            Email = accountRegisterInput.Email,
            //            Password = accountRegisterInput.Password,
            //            Timezone = Constants.DefaultTimezone,
            //            Roles = new[] { "roles/globalmember" }
            //        });

            //    var user = _documentSession
            //        .Query<All_Users.Result, All_Users>()
            //        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())  // Wait for user to be persisted
            //        .AsProjection<All_Users.Result>()
            //        .Where(x => x.Email == accountRegisterInput.Email)
            //        .First()
            //        .User;

            //    _userContext.SignUserIn(user.Id, accountRegisterInput.Email.ToLower(), accountRegisterInput.RememberMe);

            //    // App login
            //    if (Request.IsAjaxRequest())
            //    {
            //        dynamic viewModel = new ExpandoObject();
            //        viewModel.User = _userViewFactory.Make(user, null);

            //        return RestfulResult(
            //            viewModel,
            //            string.Empty,
            //            string.Empty,
            //            null,
            //            null);
            //    }

            //    return RedirectToAction("loggingin");
            //}

            //ViewBag.AccountRegister = _accountViewModelQuery.MakeAccountRegister(accountRegisterInput);
            //ViewBag.IsStaticLayout = true;

            //return View(Form.Register);

            dynamic viewModel = new ExpandoObject();

            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserCreateCommand()
                        {
                            Name = accountRegisterInput.Name,
                            Email = accountRegisterInput.Email,
                            Password = accountRegisterInput.Password,
                            Timezone = Constants.DefaultTimezone,
                            Roles = new[] {"roles/globalmember"}
                        });

                var user = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()) // Wait for user to be persisted
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.Email == accountRegisterInput.Email)
                    .First()
                    .User;

                _userContext.SignUserIn(user.Id, accountRegisterInput.Email.ToLower(), true);

                // App login
                if (Request.IsAjaxRequest())
                {
                    viewModel = new ExpandoObject();
                    viewModel.AccountRegister = _accountViewModelQuery.MakeAccountRegister(accountRegisterInput);
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

            viewModel = new ExpandoObject();
            viewModel.AccountRegister = _accountViewModelQuery.MakeAccountRegister(accountRegisterInput);

            return RestfulResult(
                viewModel,
                "account",
                "register");
        }

        [HttpGet]
        public ActionResult RequestPasswordReset()
        {
            ViewBag.IsStaticLayout = true;

            return View(Form.RequestPasswordReset);
        }

        [HttpPost]
        [Transaction]
        public ActionResult RequestPasswordReset(AccountRequestPasswordResetInput accountRequestPasswordResetInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserRequestPasswordResetCommand()
                    {
                        Email = accountRequestPasswordResetInput.Email
                    });

                return RedirectToAction("requestpasswordresetsuccess", "account");
            }

            ViewBag.RequestPasswordReset = _accountViewModelQuery.MakeAccountRequestPasswordReset(accountRequestPasswordResetInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.RequestPasswordReset);
        }

        [HttpGet]
        public ActionResult RequestPasswordResetSuccess()
        {
            return View(Form.RequestPasswordResetSuccess);
        }

        [HttpGet]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput)
        {
            ViewBag.RequestPasswordResetSuccess = _accountViewModelQuery.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
        }

        [HttpPost]
        [Transaction]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput, AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                var user = _documentSession
                    .Query<User>()
                    .Where(x => x.ResetPasswordKey == accountResetPasswordInput.ResetPasswordKey)
                    .FirstOrDefault();

                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = user.Id,
                        Password = accountChangePasswordInput.Password
                    });

                _userContext.SignUserIn(user.Id, user.Email, false);

                return RedirectToAction("privateindex", "home");
            }

            ViewBag.ResetPassword = _accountViewModelQuery.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
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
                    string.Empty
                    );
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
        [Transaction]
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
        [Authorize]
        public ActionResult ChangePassword()
        {
            var userId = _userContext.GetAuthenticatedUserId();

            if (!_userContext.HasUserPermission(userId))
            {
                return HttpUnauthorized();
            }

            var viewModel = new {};

            return RestfulResult(
                viewModel,
                "account",
                "changepassword");
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult ChangePassword(AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Password = accountChangePasswordInput.Password
                    });

                if(Request.IsAjaxRequest())
                {
                    return JsonSuccess();
                }

                return RedirectToAction("index", "home");
            }

            if (Request.IsAjaxRequest())
            {
                return JsonFailed();
            }

            return View(Form.ChangePassword);
        }

        [HttpPost]
        [Authorize]
        [Transaction]
        public ActionResult CloseCallToAction(string name)
        {
            if (Request.IsAjaxRequest())
            {
                _messageBus.Send(
                    new UserUpdateCallToActionCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        Name = name
                    });

                return JsonSuccess();
            }

            return HttpNotFound();
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
        [Transaction]
        public ActionResult UpdateVote(string id, string subId, string contributionType, string subContributionType, int score)
        {
            if (score > 1)
                score = 1;

            if (score < -1)
                score = -1;

            if (Request.IsAjaxRequest())
            {
                _messageBus.Send(
                    new VoteUpdateCommand()
                    {
                        UserId = _userContext.GetAuthenticatedUserId(),
                        ContributionId = contributionType + "/" + id,
                        Score = score,
                        SubContributionId = !string.IsNullOrWhiteSpace(subContributionType) ? subContributionType + "/" + subId : null
                    });

                return JsonSuccess();
            }

            return HttpNotFound();
        }

        [Transaction]
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

            return JsonSuccess();
        }

        [Transaction]
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

            return JsonSuccess();
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