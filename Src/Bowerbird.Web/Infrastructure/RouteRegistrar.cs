/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using Bowerbird.Web.Controllers;
using System.Collections.Generic;

namespace Bowerbird.Web.Infrastructure
{
    public class RouteRegistrar
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static void RegisterRoutesTo(RouteCollection routes)
        {
            routes.IgnoreRoute(
                "{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute(
                "{*favicon}",
                new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
                "home-privateindex",
                "",
                new {controller = "home", action = "privateindex"},
                new {authorised = new AuthenticatedConstraint()});

            routes.MapRoute(
                "home-publicindex",
                "",
                new {controller = "home", action = "publicindex"});

            routes.MapRoute(
                "account-notifications",
                "notifications",
                new { controller = "account", action = "notifications" },
                new { acceptType = new AcceptTypeContstraint("application/json", "text/javascript") });

            routes.MapRoute(
                "account-login",
                "account/login",
                new { controller = "account", action = "login" });

            routes.MapRoute(
                "account-register",
                "account/register",
                new { controller = "account", action = "register" });

            routes.MapRoute(
                "favourites-list",
                "favourites",
                new { controller = "home", action = "favourites" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "category-list",
                "categories",
                new { controller = "observations", action = "categorylist" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "species-list",
                "species",
                new { controller = "species", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "users-list",
                "users",
                new { controller = "users", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "sightings-list",
                "sightings",
                new { controller = "sightings", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "projects-list",
                "projects",
                new { controller = "projects", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "organisations-list",
                "organisations",
                new { controller = "organisations", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "home-sightings",
                "home/sightings",
                new { controller = "home", action = "sightings" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "home-posts",
                "home/posts",
                new { controller = "home", action = "posts" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "templates",
                "templates",
                new { controller = "templates", action = "index" });

            routes.MapRoute(
                "i18n",
                "i18n",
                new { controller = "i18n", action = "index" });

            routes.MapRoute(
                "project-join",
                "projects/{id}/members",
                new { controller = "projects", action = "updatemember" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "project-leave",
                "projects/{id}/members",
                new { controller = "projects", action = "deletemember" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });

            routes.MapRoute(
                "organisation-join",
                "organisations/{id}/members",
                new { controller = "organisations", action = "updatemember" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "organisation-leave",
                "organisations/{id}/members",
                new { controller = "organisations", action = "deletemember" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });

            routes.MapRoute(
                "sighting-note-create-form",
                "observations/{id}/notes/create",
                new { controller = "observations", action = "createnoteform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "sighting-note-update-form",
                "observations/{id}/notes/{sightingNoteId}/update",
                new { controller = "observations", action = "updatenoteform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "sighting-note-create",
                "observations/{id}/notes",
                new { controller = "observations", action = "createnote" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "sighting-note-update",
                "observations/{sightingId}/notes/{id}",
                new { controller = "observations", action = "updatenote" },
                new { httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "sighting-identification-create-form",
                "observations/{id}/identifications/create",
                new { controller = "observations", action = "createidentificationform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "sighting-identification-update-form",
                "observations/{id}/identifications/{identificationId}/update",
                new { controller = "observations", action = "updateidentificationform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "identification-create",
                "observations/{id}/identifications",
                new { controller = "observations", action = "createidentification" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "identification-update",
                "observations/{sightingId}/identifications/{id}",
                new { controller = "observations", action = "updateidentification" },
                new { httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "account-sighting-vote-update",
                "observations/{id}/vote",
                new { controller = "account", action = "updatevote", contributionType = "observations", subContributionType = "" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "account-sighting-note-vote-update",
                "observations/{id}/notes/{subid}/vote",
                new { controller = "account", action = "updatevote", contributionType = "observations", subContributionType = "notes" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "account-identification-vote-update",
                "observations/{id}/identifications/{subid}/vote",
                new { controller = "account", action = "updatevote", contributionType = "observations", subContributionType = "identifications" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "account-favourites-update",
                "favourites",
                new { controller = "account", action = "updatefavourite" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "account-follow-update",
                "follow",
                new { controller = "account", action = "updatefollowuser" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "projects-explore",
                "projects/explore",
                new { controller = "projects", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "organisations-explore",
                "organisations/explore",
                new { controller = "organisations", action = "list" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "group-post-create-form",
                "{groupType}/{groupId}/posts/create",
                new { controller = "posts", action = "createform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "group-post-index",
                "{groupType}/{groupId}/posts/{id}",
                new { controller = "posts", action = "index" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "project-post-update-form",
                "projects/{groupId}/posts/{id}/update",
                new { controller = "posts", action = "updateform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "organisation-post-update-form",
                "organisations/{groupId}/posts/{id}/update",
                new { controller = "posts", action = "updateform" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute(
                "group-post-create",
                "{groupType}/{groupId}/posts",
                new { controller = "posts", action = "create" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute(
                "project-post-update",
                "projects/{groupId}/posts/{id}",
                new { controller = "posts", action = "update" },
                new { httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "organisation-post-update",
                "organisations/{groupId}/posts/{id}",
                new { controller = "posts", action = "update" },
                new { httpMethod = new HttpMethodConstraint("PUT") });

            routes.MapRoute(
                "account-updatepassword",
                "account/updatepassword/{key}",
                new {controller = "account", action = "updatepassword", key = UrlParameter.Optional });

            // Load up restful controllers and create routes based on method name conventions
            RegisterRestfulControllerRouteConventions(routes);

            routes.MapRoute(
                "default",
                "{controller}/{action}",
                new {controller = "home", action = "publicindex"});
        }

        private static void RegisterRestfulControllerRouteConventions(RouteCollection routes)
        {
            var controllers = typeof(RouteRegistrar).Assembly.GetTypes().Where(x => x.BaseType == typeof(Bowerbird.Web.Controllers.ControllerBase));

            foreach (var controller in controllers)
            {
                CreateRestfulControllerRoutes(routes, controller.Name.ToLower().Replace("controller", string.Empty).Trim(), controller.GetMethods().Select(x => x.Name.ToLower()));
            }
        }

        private static void CreateRestfulControllerRoutes(RouteCollection routes, string controllerName, IEnumerable<string> controllerMethods)
        {
            //if (controllerMethods.Contains("list"))
            //{
            //    /* 
            //     * Eg: "/users" HTML/JSON GET
            //     * Used to get many users based one some filter criteria as HTML or JSON output
            //     */
            //    routes.MapRoute(
            //        controllerName + "-list",
            //        controllerName,
            //        new { controller = controllerName, action = "list" },
            //        new { httpMethod = new HttpMethodConstraint("GET") });
            //}

            if (controllerMethods.Contains("update"))
            {
                /* 
                 * Eg: "/users/2" HTML/JSON POST 
                 * Used to update a user based on an ID with HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-update",
                    controllerName + "/{id}",
                    new { controller = controllerName, action = "update" },
                    new { httpMethod = new HttpMethodConstraint("PUT"), id = @"\d+" });
            }

            if (controllerMethods.Contains("create"))
            {
                /* 
                 * Eg: "/users/2" HTML/JSON PUT 
                 * Used to create a user with HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-create",
                    controllerName,
                    new { controller = controllerName, action = "create" },
                    new { httpMethod = new HttpMethodConstraint("POST") });
            }

            if (controllerMethods.Contains("delete"))
            {
                /* 
                 * Eg: "/users/2" HTML/JSON DELETE 
                 * Used to delete a user based on an ID with HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-delete",
                    controllerName + "/{id}",
                    new { controller = controllerName, action = "delete" },
                    new { httpMethod = new HttpMethodConstraint("DELETE") });
            }

            if (controllerMethods.Contains("createform"))
            {
                /* 
                 * Eg: "/users/create" HTML/JSON GET
                 * Used to get create form data as HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-create-form",
                    controllerName + "/create",
                    new { controller = controllerName, action = "createform" },
                    new { httpMethod = new HttpMethodConstraint("GET") });
            }

            if (controllerMethods.Contains("updateform"))
            {
                /* 
                 * Eg: "/users/update" HTML/JSON GET
                 * Used to get update form data based on an ID as HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-update-form",
                    controllerName + "/{id}/update",
                    new { controller = controllerName, action = "updateform" },
                    new { httpMethod = new HttpMethodConstraint("GET") });
            }

            if (controllerMethods.Contains("deleteform"))
            {
                /* 
                 * Eg: "/users/delete" HTML/JSON GET
                 * Used to get delete form data based on an ID as HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-delete-form",
                    controllerName + "/{id}/delete",
                    new { controller = controllerName, action = "deleteform" },
                    new { httpMethod = new HttpMethodConstraint("GET") });
            }

            /* 
            * Eg: "/users/2/activity" HTML/JSON GET, POST
            * Used to get a page sub-section based on an ID as HTML or JSON output
            */
            routes.MapRoute(
                controllerName + "-section",
                controllerName + "/{id}/{action}",
                new {controller = controllerName},
                new { httpMethod = new HttpMethodConstraint("GET") });

            if (controllerMethods.Contains("index"))
            {
                /* 
                 * Eg: "/users/2" HTML/JSON GET
                 * Used to get one user based on an ID as HTML or JSON output
                 */
                routes.MapRoute(
                    controllerName + "-index",
                    controllerName + "/{id}",
                    new { controller = controllerName, action = "index" },
                    new { httpMethod = new HttpMethodConstraint("GET") });
            }
        }

        #endregion
    }
}