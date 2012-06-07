/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using Bowerbird.Core.Events;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using System.Web.Routing;
using SignalR;
using NinjectDependencyResolver = Bowerbird.Web.Config.NinjectDependencyResolver;
using System.Web;
using NinjectAdapter;

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
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
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
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

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

            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(20);
            //GlobalHost.Configuration.HeartBeatInterval = TimeSpan.FromSeconds(10);

            GlobalHost.DependencyResolver = new NinjectDependencyResolver(kernel);

            RouteTable.Routes.MapHubs();

            EventProcessor.ServiceLocator = ServiceLocator.Current;
        }
    }
}