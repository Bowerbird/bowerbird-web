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
using System.Web.Routing;

namespace Bowerbird.Web.Config
{
    public class MembersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "members";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "members-home-index",
                "",
                new { controller = "home", action = "index" },
                new { authorised = new AuthenticatedConstraint() },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "observation-list",
                "members/observation/{id}",
                new { controller = "observation", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "observation-update",
                "members/observation/{id}",
                new { controller = "observation", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "observation-delete",
                "members/observation/{id}",
                new { controller = "observation", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "observation-create",
                "members/observation/",
                new { controller = "observation", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "project-list",
                "members/project/{id}",
                new { controller = "project", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "project-update",
                "members/project/{id}",
                new { controller = "project", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "project-delete",
                "members/project/{id}",
                new { controller = "project", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "project-create",
                "members/project/",
                new { controller = "project", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "team-list",
                "members/team/{id}",
                new { controller = "team", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "team-update",
                "members/team/{id}",
                new { controller = "team", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "team-delete",
                "members/team/{id}",
                new { controller = "team", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "team-create",
                "members/team/",
                new { controller = "team", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "organisation-list",
                "members/organisation/{id}",
                new { controller = "organisation", action = "list", id = UrlParameter.Optional },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("GET") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "organisation-update",
                "members/organisation/{id}",
                new { controller = "organisation", action = "update" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("PUT") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "organisation-delete",
                "members/organisation/{id}",
                new { controller = "organisation", action = "delete" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("DELETE") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "organisation-create",
                "members/organisation/",
                new { controller = "organisation", action = "create" },
                new { authorised = new AuthenticatedConstraint(), httpMethod = new HttpMethodConstraint("POST") },
                new[] { "Bowerbird.Web.Controllers.Members" });

            context.MapRoute(
                "MembersDefault",
                "members/{controller}/{action}/{id}",
                new { action = "index", id = UrlParameter.Optional },
                new[] { "Bowerbird.Web.Controllers.Members" }
            );
        }
    }
}
