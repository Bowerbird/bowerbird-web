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

[assembly: WebActivator.PreApplicationStartMethod(typeof(Bowerbird.Web.App_Start.Bootstrapper), "PreStart")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.App_Start.Bootstrapper), "PostStart")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Bowerbird.Web.App_Start.Bootstrapper), "Stop")]

namespace Bowerbird.Web.App_Start
{
    public static class Bootstrapper
    {
        private static readonly NinjectBootstrapper _ninjectBootstrapper = new NinjectBootstrapper();

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
        /// Sets up the application ready for use
        /// </summary>
        public static void PostStart()
        {
            EventProcessor.ServiceLocator = ServiceLocator.Current;

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());

            //ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NinjectValidatorFactory(ServiceLocator.Current))); 

            //DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            AreaRegistration.RegisterAllAreas();
             
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            SystemStateManager.EnableAllServices();

            SetupSystemData();
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
            kernel.Load(new BowerbirdNinjectModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));
            
            SignalR.Hosting.AspNet.AspNetHost.SetResolver(new SignalR.Ninject.NinjectDependencyResolver(kernel));

            //FluentValidationModelValidatorProvider.Configure(x => x.ValidatorFactory = new NinjectValidatorFactory(ServiceLocator.Current));
        }

        private static void SetupSystemData()
        {
            if (!SystemStateManager.SystemDataSetup)
            {
                var setupSystemDataCommand = new SetupSystemDataCommand();

                var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
                var documentSession = ServiceLocator.Current.GetInstance<IDocumentSession>();
                var documentStore = ServiceLocator.Current.GetInstance<IDocumentStore>();

                commandProcessor.Process(setupSystemDataCommand);

                documentSession.SaveChanges();

                // Wait for all stale indexes to complete.
                while (documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                {
                    Thread.Sleep(10);
                }
            }
        }

        private static ISystemStateManager SystemStateManager
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ISystemStateManager>();
            }
        }
    }
}