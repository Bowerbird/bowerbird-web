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
using Bowerbird.Core.Config;
using Bowerbird.Core.Queries;
using Bowerbird.Core.ViewModels;
using Ninject;

namespace Bowerbird.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        [Inject]
        public IUserContext UserContext { get; set; }

        [Inject]
        public IUserViewModelQuery UserViewModelQuery { get; set; }

        [Inject]
        public IConfigSettings ConfigSettings { get; set; }

        #endregion

        #region Methods

        protected ActionResult JsonSuccess()
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Success = true;

            return RestfulResult(viewModel, null, null);
        }

        protected ActionResult JsonFailed()
        {
            dynamic viewModel = new ExpandoObject();

            viewModel.Success = false;

            return RestfulResult(viewModel, null, null);
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

        protected ActionResult RestfulResult(dynamic viewModel, string prerenderedViewName, string htmlViewName)
        {
            ActionResult actionResult = null;

            ViewBag.Model = new ExpandoObject();
            ViewBag.Model = viewModel; // Wrap the model in a "Model" property to make it work on both client & server Mustache templates

            // Stupid IE aggressively caches all requests, even *AJAX* requests. So, we have to bust out of the caching 
            // for IE using this rudimentary browser sniffing. It does the job. 'Nuff said.
            if (!string.IsNullOrWhiteSpace(Request.UserAgent) && Request.UserAgent.ToLower().Contains("msie"))
            {
                Response.Expires = -1;
                Response.CacheControl = "no-cache";
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Model.Errors = (from field in ModelState
                                    where field.Value.Errors.Any()
                                    select new
                                    {
                                        Field = field.Key,
                                        Messages = from error in field.Value.Errors
                                                   select error.ErrorMessage
                                    }).ToList();
            }

            if (Request.IsAjaxRequest())
            {
                actionResult = new JsonNetResult(new { ViewBag.Model });
            }
            else
            {
                // Add the prerendered view name that will be used by the client side JS to render bootstrapped data
                ViewBag.Model.PrerenderedView = prerenderedViewName ?? string.Empty;

                //var userContext = ServiceLocator.Current.GetInstance<IUserContext>();

                ViewBag.Model.Ver = ConfigSettings.GetEnvironmentStaticContentIncrement();// System.Configuration.ConfigurationManager.AppSettings["StaticContentIncrement"];

                if (UserContext.IsUserAuthenticated())
                {
                    //var userViewModelQuery = ServiceLocator.Current.GetInstance<IUserViewModelQuery>();
                    ViewBag.Model.AuthenticatedUser = UserViewModelQuery.BuildAuthenticatedUser(UserContext.GetAuthenticatedUserId());
                }

                ViewBag.BootstrappedJson = Raven.Imports.Newtonsoft.Json.JsonConvert.SerializeObject(ViewBag);

#if JS_COMBINE_MINIFY
                ViewBag.JavascriptSource = "main-min.js";
#elif JS_COMBINE_VERBOSE
                ViewBag.JavascriptSource = "main-combined.js";
#else
                ViewBag.JavascriptSource = "main.js";
#endif

                #region Profiling

                //ViewBag.RavenProfiler = Raven.Client.MvcIntegration.RavenProfiler.CurrentRequestSessions().ToString();
                
                if (StackExchange.Profiling.MiniProfiler.Current != null && UserContext.IsUserAuthenticated() && UserContext.HasRole("roles/" + RoleNames.GlobalAdministrator, Constants.AppRootId))
                {
                    ViewBag.MiniProfiler = StackExchange.Profiling.MiniProfiler.RenderIncludes().ToHtmlString();
                }

                #endregion Profiling

                actionResult = View(htmlViewName, "_Layout");
            }

            return actionResult;
        }

        #endregion
    }
}