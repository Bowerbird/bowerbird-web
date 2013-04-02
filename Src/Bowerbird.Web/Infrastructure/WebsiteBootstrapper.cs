/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
using System.Web.Routing;
using Nustache.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.Infrastructure.WebsiteBootstrapper), "PostStart", Order = 2)]

namespace Bowerbird.Web.Infrastructure
{
    public static class WebsiteBootstrapper
    {
        public static void PostStart()
        {
            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new NustacheViewEngine());

            RouteTable.Routes.MapHubs(new HubConfiguration {EnableJavaScriptProxies = false});

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
        }
    }
}