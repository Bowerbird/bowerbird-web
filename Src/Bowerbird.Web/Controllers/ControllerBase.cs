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
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Bowerbird.Core.Config;
using Bowerbird.Web.Builders;
using Bowerbird.Web.Services;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.DomainModels;
using System;
using Bowerbird.Core.Services;

namespace Bowerbird.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                ((ViewResult)filterContext.Result).MasterName = "_Layout";

                var userContext = ServiceLocator.Current.GetInstance<IUserContext>();

                ViewBag.Ver = System.Configuration.ConfigurationManager.AppSettings["StaticContentIncrement"]; ;

                if (userContext.IsUserAuthenticated())
                {
                    var userViewModelBuilder = ServiceLocator.Current.GetInstance<IUserViewModelBuilder>();

                    var authenticatedUser = userViewModelBuilder.BuildAuthenticatedUser(userContext.GetAuthenticatedUserId());

                    ViewBag.AuthenticatedUser = authenticatedUser;
                    ViewBag.BootstrappedJson = JsonConvert.SerializeObject(new
                        {
                            AuthenticatedUser = authenticatedUser,
                            ViewBag.Model,
                            ViewBag.PrerenderedView
                        });
                }
                else
                {
                    ViewBag.BootstrappedJson = JsonConvert.SerializeObject(new
                    {
                        ViewBag.Model,
                        ViewBag.PrerenderedView
                    });
                }

#if JS_COMBINE_MINIFY
                    ViewBag.JavascriptSource = "main-min.js";
#elif JS_COMBINE_VERBOSE
                    ViewBag.JavascriptSource = "main-combined.js";
#else
                ViewBag.JavascriptSource = "main.js";
#endif

#if DEBUG
                ViewBag.RavenProfiler = Raven.Client.MvcIntegration.RavenProfiler.CurrentRequestSessions().ToString();
#endif
            }

            base.OnActionExecuted(filterContext);
        }

        protected HttpUnauthorizedResult HttpUnauthorized()
        {
            return new HttpUnauthorizedResult();
        }

        protected ActionResult JsonSuccess()
        {
            return new JsonNetResult(new { Success = true });
        }

        protected ActionResult JsonFailed()
        {
            return new JsonNetResult(new { Success = false });
        }

        protected void DebugToClient(dynamic output)
        {
            var debugger = ServiceLocator.Current.GetInstance<IBackChannelService>();

            debugger.DebugToClient(output);
        }

        protected string VerbosifyId<T>(string id)
        {
            if (id.Contains("/"))
            {
                return id;
            }

            string name = typeof(T).Name.ToLower();

            if (name.EndsWith("s"))
            {
                name = name += "es";
            }
            else if (name.EndsWith("y"))
            {
                name = name.Substring(0, name.Length - 1) + "ies";
            }
            else
            {
                name += "s";
            }

            return string.Format("{0}/{1}", name, id);
        }

        protected ActionResult RestfulResult(dynamic viewModel, string prerenderedViewName, string htmlViewName, Action<dynamic> htmlViewTask = null, Action<dynamic> jsonViewTask = null)
        {
            ActionResult actionResult = null;

            var newViewModel = new { Model = viewModel }; // Wrap the model in a "Model" property to make it work on both client & server Mustache templates

            if (Request.IsAjaxRequest())
            {
                if (jsonViewTask != null)
                {
                    jsonViewTask(newViewModel);
                }
                actionResult = new JsonNetResult(newViewModel);
            }
            else
            {
                // Add the prerendered view name that will be used by the client side JS to render bootstrapped data
                if (!string.IsNullOrWhiteSpace(prerenderedViewName))
                {
                    ViewBag.PrerenderedView = prerenderedViewName;
                }

                // Perform any additional html view tasks on the model
                if (htmlViewTask != null)
                {
                    htmlViewTask(newViewModel);
                }

                ViewBag.Model = viewModel;

                actionResult = View(htmlViewName);
            }

            return actionResult;
        }

        #endregion
    }
}