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
using System.Web.Mvc;
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Core.DomainModels;
using Newtonsoft.Json;
using Bowerbird.Core.Config;
using Bowerbird.Web.Builders;

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

                    var authenticatedUser = userViewModelBuilder.BuildAuthenticatedUser();

                    ViewBag.AuthenticatedUser = authenticatedUser;
                    ViewBag.BootstrappedJson = JsonConvert.SerializeObject(new
                        {
                            AuthenticatedUser = authenticatedUser,
                            OnlineUsers = userViewModelBuilder.BuildOnlineUsers(),
                            Model = ViewBag.Model,
                            PrerenderedView = ViewBag.PrerenderedView
                        });
                }
                else
                {
                    ViewBag.BootstrappedJson = JsonConvert.SerializeObject(new
                        {
                            Model = ViewBag.Model,
                            PrerenderedView = ViewBag.PrerenderedView
                        });
                }

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
            return new JsonNetResult("success");
        }

        protected ActionResult JsonFailed()
        {
            return new JsonNetResult("failure");
        }
        #endregion      
    }
}