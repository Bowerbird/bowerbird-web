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

        protected HttpUnauthorizedResult HttpUnauthorized()
        {
            return new HttpUnauthorizedResult();
        }

        /// <summary>
        /// Returns a mustachioed view using the default layout
        /// </summary>
        /// <param name="modelName">name of the model to be referenced inside the mustache template</param>
        /// <param name="model">The model to be bound</param>
        /// <returns>A mustache view</returns>
        protected ViewResult TemplateView(string modelName, object model)
        {
            return TemplateView(modelName, model, null, "_Layout");
        }

        /// <summary>
        /// Returns a mustachioed view using the default layout
        /// </summary>
        /// <param name="modelName">name of the model to be referenced inside the mustache template</param>
        /// <param name="model">The model to be bound</param>
        /// <param name="viewName">Name of the mustache template</param>
        /// <returns>A mustache view</returns>
        protected ViewResult TemplateView(string modelName, object model, string viewName)
        {
            return TemplateView(modelName, model, viewName, "_Layout");
        }

        /// <summary>
        /// Returns a mustachioed view using the default layout
        /// </summary>
        /// <param name="modelName">name of the model to be referenced inside the mustache template</param>
        /// <param name="model">The model to be bound</param>
        /// <param name="viewName">Name of the mustache template</param>
        /// <param name="layout">The master layout to use</param>
        /// <returns>A mustache view</returns>
        protected ViewResult TemplateView(string modelName, object model, string viewName, string layout)
        {
            ViewData[modelName] = model;

            var userContext = ServiceLocator.Current.GetInstance<IUserContext>();
            if (userContext.IsUserAuthenticated())
            {
                ViewData["AuthenticatedUser"] = ServiceLocator.Current.GetInstance<IDocumentSession>().Load<User>(userContext.GetAuthenticatedUserId());
            }

            return View(viewName, layout);
        }

        #endregion      
      
    }
}
