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
using System.IO;
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

        /// <summary>
        /// http://stackoverflow.com/questions/9702130/sharing-mustache-nustache-templates-between-server-and-client-asp-net-mvc
        /// </summary>
        public static IHtmlString RenderControllerTemplate(this HtmlHelper helper, string controller, string view)
        {
            string filePath = HttpContext.Current.Server.MapPath(string.Format("~Views/{0}/{1}.mustache", controller, view));

            //load from file
            StreamReader streamReader = File.OpenText(filePath);
            string markup = streamReader.ReadToEnd();
            streamReader.Close();

            return new HtmlString(markup);

        }

        public static IHtmlString RenderSharedTemplate(this HtmlHelper helper, string view)
        {
            string filePath = HttpContext.Current.Server.MapPath(string.Format("Views/Shared/{0}.mustache", view));

            //load from file
            StreamReader streamReader = File.OpenText(filePath);
            string markup = streamReader.ReadToEnd();
            streamReader.Close();

            markup = string.Format(@"<script id=""{0}"" type=""text/html"">{1}</script>", view, markup);

            return helper.Raw(markup);

        }

    }
}