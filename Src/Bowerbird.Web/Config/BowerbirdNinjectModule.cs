using Ninject.Modules;
using Raven.Client;
using Bowerbird.Core.CommandHandlers;
using Ninject.Extensions.Conventions;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.DomainModels;

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

            // Request scope
            Bind<IDocumentSession>().ToProvider<RavenSessionProvider>().InRequestScope();

            // Transient scope
            Bind<IServiceLocator>().ToMethod(x => ServiceLocator.Current);

            Kernel.Scan(x =>
            {
                x.FromAssemblyContaining(typeof(User));
                x.FromCallingAssembly();

                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(ICommandHandler<>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IEventHandler<>)));
                x.BindingGenerators.Add(new DefaultBindingGenerator());
            });
        }

        #endregion

    }
}