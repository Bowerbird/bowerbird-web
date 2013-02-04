using System.Web;
using System.Web.Routing;

namespace Bowerbird.Web.Infrastructure
{
    public class AuthenticatedConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return httpContext.Request.IsAuthenticated;
        }
    }
}
