using System.Web;
using System.Linq;
using System.Web.Routing;
using System.Collections;
using System.Collections.Generic;

namespace Bowerbird.Web.Config
{
    public class AcceptTypeContstraint : IRouteConstraint
    {

        #region Members

        #endregion

        #region Constructors

        public AcceptTypeContstraint(params string[] acceptTypes)
        {
            AcceptTypes = acceptTypes.Select(x => x.ToLower().Trim());
        }

        #endregion

        #region Properties

        public IEnumerable<string> AcceptTypes { get; set; }

        #endregion

        #region Methods

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return httpContext.Request.AcceptTypes.Select(x => x.ToLower()).Intersect(AcceptTypes).Count() > 0;
        }

        #endregion      

    }
}
