/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Caching;
using System.Collections.Generic;
using Bowerbird.Core.Internationalisation;

namespace Bowerbird.Web.Controllers
{
    /// <summary>
    /// http://stackoverflow.com/questions/9702130/sharing-mustache-nustache-templates-between-server-and-client-asp-net-mvc
    /// </summary>
    public class I18nController : ControllerBase
    {
        #region Fields

        #endregion

        #region Constructors

        public I18nController()
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            // Default language is en-AU (only language currently configured)
            var i18n = I18n.ResourceManager.GetResourceSet(new CultureInfo("en-AU"), true, true);
           
            // Return a JSONP result that contains the internationalisation data. It will be intitialised and made available by require.js
            return Content(string.Format(@"
                define(function () {{
                    return {0};
                }});
                ", Newtonsoft.Json.JsonConvert.SerializeObject(i18n)), "text/javascript; charset=UTF-8");
        }

        #endregion
    }
}