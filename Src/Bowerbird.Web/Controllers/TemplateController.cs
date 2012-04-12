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
using System.Web.Mvc;


namespace Bowerbird.Web.Controllers
{
    /// <summary>
    /// http://stackoverflow.com/questions/9702130/sharing-mustache-nustache-templates-between-server-and-client-asp-net-mvc
    /// </summary>
    public class TemplateController : ControllerBase
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public PartialViewResult Get(string name)
        {
            return PartialView(name);
        }

        [HttpGet]
        public PartialViewResult Render(string name, Object model)
        {
            return PartialView(name, model);
        }

        #endregion
    }
}