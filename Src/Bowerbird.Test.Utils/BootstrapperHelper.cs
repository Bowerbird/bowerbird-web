using System;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Mvc;
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using NinjectAdapter;
using SignalR.Infrastructure;
using SignalR.Ninject;
using Bowerbird.Core;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bowerbird.Test.Utils
{
    public static class BootstrapperHelper
    {
        private static NinjectBootstrapper _ninjectBootstrapper;// = new NinjectBootstrapper();

        public static void Startup()
        {
            _ninjectBootstrapper = new NinjectBootstrapper();

            try
            {
                _ninjectBootstrapper.Initialize(CreateKernel);
            }
            catch { /* catches error on second test run - needs investigation */ }

            PostStart();
        }

        public static void Shutdown()
        {
            _ninjectBootstrapper.ShutDown();

            _ninjectBootstrapper.MakeNull<NinjectBootstrapper>();

            _ninjectBootstrapper = null;
        }

        /// <summary>
        /// Starts the application
        /// </summary>
        private static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));

            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));

            //_ninjectBootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Sets up the application ready for use
        /// </summary>
        private static void PostStart()
        {
            EventProcessor.ServiceLocator = ServiceLocator.Current;

            //XmlConfigurator.Configure();

            //ViewEngines.Engines.Clear();

            //ViewEngines.Engines.Add(new RazorViewEngine());

            //AreaRegistration.RegisterAllAreas();

            //RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        private static void Stop()
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
            kernel.Load(new BowerbirdNinjectModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            SignalR.Infrastructure.DependencyResolver.SetResolver(new SignalR.Ninject.NinjectDependencyResolver(kernel));
        }
    }
}