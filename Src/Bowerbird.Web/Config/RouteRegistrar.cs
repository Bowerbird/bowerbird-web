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

using System.Web.Mvc;
using System.Web.Routing;

namespace Bowerbird.Web.Config
{
    public class RouteRegistrar
    {
        public static void RegisterRoutesTo(RouteCollection routes)
        {
            routes.IgnoreRoute(
                "{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute(
                "{*favicon}", 
                new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "account-resetpassword",
                "account/resetpassword/{resetpasswordkey}",
                new {controller = "account", action = "resetpassword", resetpasswordkey = UrlParameter.Optional});

            routes.MapRoute(
                "observation-list", 
                "observation/{id}",
                new { controller = "observation", action = "list", id = UrlParameter.Optional },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "observation-update", 
                "observation/{id}",
                new { controller = "observation", action = "update" },
                new { httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "observation-delete", 
                "observation/{id}",
                new { controller = "observation", action = "delete" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });

            routes.MapRoute(
                "observation-create", 
                "observation/",
                new { controller = "observation", action = "create" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                new[] { "Bowerbird.Web.Controllers.Public" });
        }
    }
}
