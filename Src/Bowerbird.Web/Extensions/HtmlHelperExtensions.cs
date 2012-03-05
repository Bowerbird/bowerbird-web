/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Configuration;
using System.Web.Mvc;
using Bowerbird.Core.Extensions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
				
namespace Bowerbird.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Returns encoded JSON string
        /// </summary>
        public static IHtmlString Json(this HtmlHelper htmlHelper, object model)
        {
            return new MvcHtmlString(Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

    }
}