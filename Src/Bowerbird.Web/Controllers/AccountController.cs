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
using Bowerbird.Core.Repositories;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Properties;
using Bowerbird.Web.ViewModels;
using NodaTime;
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
        private readonly IAccountViewModelBuilder _accountViewModelBuilder;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IActivityViewModelBuilder _activityViewModelBuilder;
        private readonly ISightingViewModelBuilder _sightingViewModelBuilder;
        private readonly IPostViewModelBuilder _postViewModelBuilder;

        #endregion

        #region Constructors

        public AccountController(
            IMessageBus messageBus,
            IUserContext userContext,
            IDocumentSession documentSession,
            IAccountViewModelBuilder accountViewModelBuilder,
            IUserViewModelBuilder userViewModelBuilder,
            IActivityViewModelBuilder activityViewModelBuilder,
            IUserViewFactory userViewFactory,
            ISightingViewModelBuilder sightingViewModelBuilder,
            IPostViewModelBuilder postViewModelBuilder
            )
        {
            Check.RequireNotNull(messageBus, "messageBus");
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(accountViewModelBuilder, "accountViewModelBuilder");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(activityViewModelBuilder, "activityViewModelBuilder");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(sightingViewModelBuilder, "sightingViewModelBuilder");
            Check.RequireNotNull(postViewModelBuilder, "postViewModelBuilder");

            _messageBus = messageBus;
            _userContext = userContext;
            _documentSession = documentSession;
            _accountViewModelBuilder = accountViewModelBuilder;
            _userViewModelBuilder = userViewModelBuilder;
            _activityViewModelBuilder = activityViewModelBuilder;
            _userViewFactory = userViewFactory;
            _sightingViewModelBuilder = sightingViewModelBuilder;
            _postViewModelBuilder = postViewModelBuilder;
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

            //ViewBag.AccountLogin = _accountViewModelBuilder.MakeAccountLogin(returnUrl);
            //ViewBag.IsStaticLayout = true;

            //return View(Form.Login);

            dynamic viewModel = new ExpandoObject();
            viewModel.AccountLogin = _accountViewModelBuilder.MakeAccountLogin(returnUrl);

            return RestfulResult(
                viewModel,
                "account",
                "login",
                new Action<dynamic>(x =>
                {
                    //x.Model.ShowWelcome = user.User.CallsToAction.Contains("welcome");
                    //x.Model.ShowActivities = true;
                }));
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Login(AccountLoginInput accountLoginInput)
        {
            Check.RequireNotNull(accountLoginInput, "accountLoginInput");

            User user = null;

            if (ModelState.IsValid &&
                AreCredentialsValid(accountLoginInput.Email, accountLoginInput.Password, out user))
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

                if(Request.IsAjaxRequest())
                {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.User = _userViewFactory.Make(user);

                    return RestfulResult(
                        viewModel,
                        string.Empty,
                        string.Empty,
                        null,
                        null);
                }

                return RedirectToAction("loggingin", new { returnUrl = accountLoginInput.ReturnUrl });
            }

            ModelState.AddModelError("", "");

            ViewBag.AccountLogin = _accountViewModelBuilder.MakeAccountLogin(accountLoginInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.Login);
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

            ViewBag.IsStaticLayout = true;

            return View(Form.Register);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Transaction]
        public ActionResult Register(AccountRegisterInput accountRegisterInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserCreateCommand()
                    {
                        Name = accountRegisterInput.Name,
                        Email = accountRegisterInput.Email,
                        Password = accountRegisterInput.Password,
                        DefaultLicence = Constants.DefaultLicence,
                        Timezone = Constants.DefaultTimezone,
                        Roles = new[] { "roles/globalmember" }
                    });

                // HACK: Persist user before _userContext.SignUserIn(..)
                _documentSession.SaveChanges();

                // HACK: Wait a couple of seconds to ensure all indexes are up to date
                //var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                //stopwatch.Start();
                //while (stopwatch.ElapsedMilliseconds < 3000) {}

                var user = _documentSession
                    .Query<All_Users.Result, All_Users>()
                    //.Customize(x => x..WaitForNonStaleResultsAsOfLastWrite())
                    .AsProjection<All_Users.Result>()
                    .Where(x => x.Email == accountRegisterInput.Email)
                    .First()
                    .User;

                _userContext.SignUserIn(user.Id, accountRegisterInput.Email.ToLower(), accountRegisterInput.RememberMe);

                // App login
                if (Request.IsAjaxRequest())
                {
                    dynamic viewModel = new ExpandoObject();
                    viewModel.User = _userViewFactory.Make(user);

                    return RestfulResult(
                        viewModel,
                        string.Empty,
                        string.Empty,
                        null,
                        null);
                }

                return RedirectToAction("loggingin");
            }

            ViewBag.AccountRegister = _accountViewModelBuilder.MakeAccountRegister(accountRegisterInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.Register);
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

            ViewBag.RequestPasswordReset = _accountViewModelBuilder.MakeAccountRequestPasswordReset(accountRequestPasswordResetInput);
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
            ViewBag.RequestPasswordResetSuccess = _accountViewModelBuilder.MakeAccountResetPassword(accountResetPasswordInput);
            ViewBag.IsStaticLayout = true;

            return View(Form.ResetPassword);
        }

        [HttpPost]
        [Transaction]
        public ActionResult ResetPassword(AccountResetPasswordInput accountResetPasswordInput, AccountChangePasswordInput accountChangePasswordInput)
        {
            if (ModelState.IsValid)
            {
                var user = _documentSession.LoadUserByResetPasswordKey(accountResetPasswordInput.ResetPasswordKey);

                _messageBus.Send(
                    new UserUpdatePasswordCommand()
                    {
                        UserId = user.Id,
                        Password = accountChangePasswordInput.Password
                    });

                _userContext.SignUserIn(user.Id, user.Email, false);

                return RedirectToAction("privateindex", "home");
            }

            ViewBag.ResetPassword = _accountViewModelBuilder.MakeAccountResetPassword(accountResetPasswordInput);
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
                viewModel.AuthenticatedUser =
                    _userViewModelBuilder.BuildAuthenticatedUser(_userContext.GetAuthenticatedUserId());

                return RestfulResult(
                    viewModel,
                    string.Empty,
                    string.Empty
                    );
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Notifications(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelBuilder.BuildNotificationActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
                    }
                });
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Activity(ActivityInput activityInput, PagingInput pagingInput)
        {
            if (Request.IsAjaxRequest())
            {
                return new JsonNetResult(new
                {
                    Model = new
                    {
                        Activities = _activityViewModelBuilder.BuildHomeActivityList(_userContext.GetAuthenticatedUserId(), activityInput, pagingInput)
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

            if (!_userContext.HasUserPermission(userId))
            {
                return HttpUnauthorized();
            }

            dynamic viewModel = new ExpandoObject();

            var user = _documentSession.Load<User>(_userContext.GetAuthenticatedUserId());

            viewModel.User = _userViewModelBuilder.BuildUpdateUser(userId);
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
                "update",
                new Action<dynamic>(x => x.Model.Update = true));
        }

        [HttpPut]
        [Authorize]
        [Transaction]
        public ActionResult Update(UserUpdateInput userUpdateInput)
        {
            if (ModelState.IsValid)
            {
                _messageBus.Send(
                    new UserUpdateCommand()
                    {
                        Id = _userContext.GetAuthenticatedUserId(),
                        Name = userUpdateInput.Name,
                        Email = userUpdateInput.Email,
                        Description = userUpdateInput.Description,
                        AvatarId = userUpdateInput.AvatarId,
                        Timezone = userUpdateInput.Timezone,
                        DefaultLicence = userUpdateInput.DefaultLicence
                    });

                if (Request.IsAjaxRequest())
                {
                    return JsonSuccess();
                }

                return RedirectToAction("index", "home");
            }

            if (Request.IsAjaxRequest())
            {
                return JsonFailed();
            }
            
            ViewBag.Model.User = new
            {
                userUpdateInput.AvatarId,
                userUpdateInput.Description,
                userUpdateInput.Email,
                userUpdateInput.Name
            };

            return View(Form.Update);
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

        /// <summary>
        /// Returns a list of valid timezones as a dictionary, where the key is the timezone id, and the value can be used for display.
        /// </summary>
        /// <param name="countryCode">The two-letter country code to get timezones for.  Returns all timezones if null or empty.</param>
        public object GetTimeZones(string countryCode, string existingTimezone)
        {
            var timeZones = string.IsNullOrEmpty(countryCode)
                                ? Zones.SelectMany(x => x)
                                : Zones[countryCode.ToUpper()];

            var now = Instant.FromDateTimeUtc(DateTime.UtcNow);
            var tzdb = DateTimeZoneProviders.Tzdb;

            var list = from id in timeZones
                       let tz = tzdb[id]
                       let offset = tz.GetZoneInterval(now).StandardOffset
                       orderby offset, id
                       select new
                       {
                           Value = id,
                           Text = string.Format("{0} ({1})", id, offset.ToString("+HH:mm", null)),
                           Selected = id == existingTimezone
                       };

            return list;
        }


        private static volatile ILookup<string, string> _zones;
        private static readonly object SyncRoot = new object();

        private static ILookup<string, string> Zones
        {
            get
            {
                if (_zones != null)
                    return _zones;

                lock (SyncRoot)
                {
                    if (_zones == null)
                        _zones = ReadAndParseTimeZones().ToLookup(x => x.Key, x => x.Value);
                }

                return _zones;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> ReadAndParseTimeZones()
        {
            // Simple parser for the zones data, which is embedded as a string resource.
            // The data is sourced from the zone.tab file from the offical tz database.
            // TODO: When NodaTime embeds this file, switch to their copy so we don't have to maintain it.
            using (var reader = new StringReader(Resources.Zones))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // ignore comments and blank lines
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                        continue;

                    var data = line.Split('\t');
                    var key = data[0];
                    var value = data[2];
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

        #endregion
    }
}