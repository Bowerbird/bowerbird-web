using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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
                "MembersDefault",
                "members/{controller}/{action}/{id}",
                new { action = "index", id = UrlParameter.Optional },
                new[] { "Bowerbird.Web.Controllers.Members" }
            );
        }
    }
}
