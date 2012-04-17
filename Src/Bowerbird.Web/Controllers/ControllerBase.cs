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
using Bowerbird.Core.Config;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;
using Bowerbird.Core.DomainModels;

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
            }

            var userContext = ServiceLocator.Current.GetInstance<IUserContext>();
            if (userContext.IsUserAuthenticated())
            {
                var user = ServiceLocator.Current.GetInstance<IDocumentSession>().Load<User>(userContext.GetAuthenticatedUserId());
                ViewBag.UserContext = new
                                          {
                                              User = user,
                                              UserJson = Newtonsoft.Json.JsonConvert.SerializeObject(user)
                                          };
            }

            base.OnActionExecuted(filterContext);
        }

        protected HttpUnauthorizedResult HttpUnauthorized()
        {
            return new HttpUnauthorizedResult();
        }

        #endregion      
      
    }
}
