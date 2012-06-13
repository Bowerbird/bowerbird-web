/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.Events;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;
using System.Web.Routing;
using Bowerbird.Core.Config;
using Bowerbird.Core.CommandHandlers;
using Nustache.Mvc;
using Raven.Client.MvcIntegration;
using Raven.Client;
using SignalR;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.App_Start.WebsiteBootstrapper), "PostStart")]

namespace Bowerbird.Web.App_Start
{
    public static class WebsiteBootstrapper
    {
        public static void PostStart()
        {
            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new NustacheViewEngine());

            RouteTable.Routes.MapHubs();

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            ServiceLocator.Current.GetInstance<ISystemStateManager>().SetupSystem(true);

            RavenProfiler.InitializeFor(ServiceLocator.Current.GetInstance<IDocumentStore>());
        }
    }
}