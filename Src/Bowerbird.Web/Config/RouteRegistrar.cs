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
                "home-index-private",
                "",
                new { controller = "home", action = "privateindex" },
                new { authorised = new AuthenticatedConstraint() });

            routes.MapRoute(
                "home-index-public",
                "",
                new { controller = "home", action = "publicindex" });






            routes.MapRoute(
                "observations-get-many",
                "observations",
                new { controller = "observations", action = "getmany" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "observations-get-one",
                "observations/{id}",
                new { controller = "observations", action = "getone" },
                new { httpMethod = new HttpMethodConstraint("GET"), id = @"^((?!create|update|delete).*)$" });

            routes.MapRoute(
                "observations-create-form",
                "observations/create",
                new { controller = "observations", action = "createform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "observations-update-form",
                "observations/update/{id}",
                new { controller = "observations", action = "updateform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "observations-delete-form",
                "observations/delete/{id}",
                new { controller = "observations", action = "deleteform" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "observations-create",
                "observations/",
                new { controller = "observations", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "observations-update",
                "observations/{id}", 
                new { controller = "observations", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "observations-delete",
                "observations/{id}",
                new { controller = "observations", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") });







            routes.MapRoute(
                "project-list",
                "project/{id}",
                new { controller = "project", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "project-update",
                "project/{id}",
                new { controller = "project", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "project-delete",
                "project/{id}",
                new { controller = "project", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "project-create",
                "project/",
                new { controller = "project", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "team-list",
                "team/{id}",
                new { controller = "team", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "team-update",
                "team/{id}",
                new { controller = "team", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "team-delete",
                "team/{id}",
                new { controller = "team", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "team-create",
                "team/",
                new { controller = "team", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "organisation-list",
                "organisation/{id}",
                new { controller = "organisation", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "organisation-update",
                "organisation/{id}",
                new { controller = "organisation", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            routes.MapRoute(
                "organisation-delete",
                "organisation/{id}",
                new { controller = "organisation", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers" });

            routes.MapRoute(
                "organisation-create",
                "organisation/",
                new { controller = "organisation", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers" });

            //routes.MapRoute(
            //    "MembersDefault",
            //    "{controller}/{action}/{id}",
            //    new {action = "index", id = UrlParameter.Optional},
            //    new[] {"Bowerbird.Web.Controllers"});

            routes.MapRoute("Templates", "templates/{name}",
                new { controller = "Template", action = "Get" });


            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                new[] { "Bowerbird.Web.Controllers" });

        }
    }
}
