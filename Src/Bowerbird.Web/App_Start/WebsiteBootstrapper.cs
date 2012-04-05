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

using System.Diagnostics;
using System.Linq;
using System.Threading;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Mvc;
using Raven.Client;
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using NinjectAdapter;
using System.Web.Mvc;
using System.Web.Routing;
using Bowerbird.Core.Config;
using Bowerbird.Core.CommandHandlers;

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

            AreaRegistration.RegisterAllAreas();

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            ServiceLocator.Current.GetInstance<SetupSystemDataCommandHandler>().Handle(new SetupSystemDataCommand());

            ServiceLocator.Current.GetInstance<ISystemStateManager>().EnableAllServices();
        }
    }
}