/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Dynamic;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.Config;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
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

                ViewBag.Ver = System.Configuration.ConfigurationManager.AppSettings["StaticContentIncrement"];

                if (userContext.IsUserAuthenticated())
                {
                    var userViewModelQuery = ServiceLocator.Current.GetInstance<IUserViewModelQuery>();
                    ViewBag.Model.AuthenticatedUser = userViewModelQuery.BuildAuthenticatedUser(userContext.GetAuthenticatedUserId());
                }

                ViewBag.BootstrappedJson = Raven.Imports.Newtonsoft.Json.JsonConvert.SerializeObject(ViewBag);

#if JS_COMBINE_MINIFY
                ViewBag.JavascriptSource = "main-min.js";
#elif JS_COMBINE_VERBOSE
                ViewBag.JavascriptSource = "main-combined.js";
#else
                ViewBag.JavascriptSource = "main.js";
#endif

#if DEBUG
                //ViewBag.RavenProfiler = Raven.Client.MvcIntegration.RavenProfiler.CurrentRequestSessions().ToString();
#endif
            }

            base.OnActionExecuted(filterContext);
        }

        protected HttpUnauthorizedResult HttpUnauthorized()
        {
            return new HttpUnauthorizedResult();
        }

        protected ActionResult JsonSuccess(string action = "")
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Success = true;

            if (!string.IsNullOrEmpty(action))
            {
                viewModel.Action = action;
            }

            return RestfulResult(
                viewModel,
                null,
                null);
        }

        protected ActionResult JsonFailed(string action = "")
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Success = false;

            if (!string.IsNullOrEmpty(action))
            {
                viewModel.Action = action;
            }

            return RestfulResult(
                viewModel,
                null,
                null);
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

            dynamic newViewModel = new ExpandoObject();
            newViewModel.Model = new ExpandoObject();
            newViewModel.Model = viewModel; // Wrap the model in a "Model" property to make it work on both client & server Mustache templates

            // Stupid IE aggressively caches all requests, even *AJAX* requests. So, we have to bust out of the caching 
            // for IE using this rudimentary browser sniffing. It does the job. 'Nuff said.
            if (Request.UserAgent.ToLower().Contains("msie"))
            {
                Response.Expires = -1;
                Response.CacheControl = "no-cache";
            }

            if (!ModelState.IsValid)
            {
                newViewModel.Model.Errors = (from field in ModelState
                                    where field.Value.Errors.Any()
                                    select new
                                    {
                                        Field = field.Key,
                                        Messages = from error in field.Value.Errors
                                                   select error.ErrorMessage
                                    }).ToList();
            }

            // Hamish added 23/11/12 the ajax response for empty view names for app debugging.
            if (Request.IsAjaxRequest() || ((string.IsNullOrEmpty(prerenderedViewName)) && (string.IsNullOrEmpty(htmlViewName))))
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
                    newViewModel.Model.PrerenderedView = prerenderedViewName;
                }

                // Perform any additional html view tasks on the model
                if (htmlViewTask != null)
                {
                    htmlViewTask(newViewModel);
                }

                ViewBag.Model = newViewModel.Model;

                actionResult = View(htmlViewName);
            }

            return actionResult;
        }

        #endregion
    }
}