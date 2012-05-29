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
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using NinjectAdapter;
using System.Web.Mvc;
using System.Web.Routing;
using Bowerbird.Core.Config;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Bowerbird.Web.App_Start.NinjectBootstrapper), "PreStart")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Bowerbird.Web.App_Start.NinjectBootstrapper), "Stop")]

namespace Bowerbird.Web.App_Start
{
    public static class NinjectBootstrapper
    {
        private static readonly Bootstrapper _ninjectBootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));

            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));

            _ninjectBootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            _ninjectBootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            RegisterServices(kernel);

            return kernel;
        }

        /// <summary>
        /// Load modules and services
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new NinjectBindingModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            //SignalR.Hosting.AspNet.AspNetHost.SetResolver(new SignalR.Ninject.NinjectDependencyResolver(kernel));

            EventProcessor.ServiceLocator = ServiceLocator.Current;
        }
    }
}