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
