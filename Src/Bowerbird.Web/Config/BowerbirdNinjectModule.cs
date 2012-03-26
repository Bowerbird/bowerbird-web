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

using Bowerbird.Web.Notifications;
using Ninject.Modules;
using Raven.Client;
using Bowerbird.Core.CommandHandlers;
using Ninject.Extensions.Conventions;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.DomainModels;
using SignalR;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.Config
{
    public class BowerbirdNinjectModule : NinjectModule
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override void Load()
        {
            // Singleton scope
            Bind<IDocumentStore>().ToProvider<RavenDocumentStoreProvider>().InSingletonScope();
            Bind<IPermissionChecker>().To<PermissionChecker>().InSingletonScope().OnActivation(x => ((PermissionChecker)x).Init());
            Bind<SystemStateProvider>().To<SystemStateProvider>().InSingletonScope();

            // Request scope
            Bind<IDocumentSession>().ToProvider<RavenSessionProvider>().InRequestScope();

            // Transient scope
            Bind<IServiceLocator>().ToMethod(x => ServiceLocator.Current);
            Bind<IJsonSerializer>().To<SignalRJsonConvertAdapter>();
            Bind<ISystemState>().ToProvider<SystemStateProvider>();

            Kernel.Scan(x =>
            {
                x.FromAssemblyContaining(typeof(User));
                x.FromCallingAssembly();

                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(ICommandHandler<>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(ICommandHandler<,>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IEventHandler<>)));
                x.BindingGenerators.Add(new DefaultBindingGenerator());

                x.Excluding<PermissionChecker>();
                x.Excluding<SystemState>();
                x.Excluding<ISystemState>();
            });
        }

        #endregion

    }
}