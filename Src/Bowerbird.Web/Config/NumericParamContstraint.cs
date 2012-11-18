//using System.Web;
//using System.Linq;
//using System.Web.Routing;
//using System.Collections;
//using System.Collections.Generic;

//namespace Bowerbird.Web.Config
//{
//    public class NumericParamContstraint : IRouteConstraint
//    {

//        #region Members

//        #endregion

//        #region Constructors

//        public NumericParamContstraint()
//        {
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
//        {
//            var y = false;
//            return values.Any(x => System.Int32.TryParse(x, out y));
//        }

//        #endregion      

//    }
//}
