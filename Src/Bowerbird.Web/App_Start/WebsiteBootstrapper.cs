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
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;
using System.Web.Routing;
using Bowerbird.Core.Config;
using Bowerbird.Core.CommandHandlers;
using Nustache.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.App_Start.WebsiteBootstrapper), "PostStart")]

namespace Bowerbird.Web.App_Start
{
    public static class WebsiteBootstrapper
    {
        /// <summary>
        /// Sets up the application ready for use
        /// </summary>
        public static void PostStart()
        {
            EventProcessor.ServiceLocator = ServiceLocator.Current;

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());
            //ViewEngines.Engines.Add(new NustacheViewEngine() { RootContext = NustacheViewEngineRootContext.Model });
            ViewEngines.Engines.Add(new NustacheViewEngine());

            //AreaRegistration.RegisterAllAreas();

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            ServiceLocator.Current.GetInstance<SetupSystemDataCommandHandler>().Handle(new SetupSystemDataCommand());

            ServiceLocator.Current.GetInstance<ISystemStateManager>().EnableAllServices();
        }
    }
}