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
using Bowerbird.Web.Factories;
using Bowerbird.Web.ViewModels;
using Microsoft.Practices.ServiceLocation;

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
                
                ViewBag.UserContext = GetClientUserContext();
                ViewBag.PrerenderedView = filterContext.HttpContext.Request.RawUrl.ToLower().Substring(1);

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

        protected ClientUserContext GetClientUserContext()
        {
            return ServiceLocator.Current.GetInstance<IClientUserContextFactory>().ClientUserContext();
        }

        #endregion      
    }
}