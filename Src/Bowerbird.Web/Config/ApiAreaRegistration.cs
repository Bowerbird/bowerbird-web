using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Bowerbird.Web.Config
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ApiDefault",
                "api/{controller}/{action}/{id}",
                new { action = "index", id = UrlParameter.Optional },
                new[] { "Bowerbird.Web.Controllers.Api" }
            );
        }
    }
}
