using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bowerbird.Core.Config;
using Bowerbird.Core.Services;
using Ninject.Modules;
using Raven.Client;
using Bowerbird.Core;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Repositories;
using Ninject.Extensions.Conventions;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Tasks;
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.CommandFactories;

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

            // Request scope
            Bind<IDocumentSession>().ToProvider<RavenSessionProvider>().InRequestScope();

            // Transient scope
            Bind<IServiceLocator>().ToMethod(x => ServiceLocator.Current);
            Bind(typeof(IRepository<>)).To(typeof(Repository<>));

            Kernel.Scan(x =>
            {
                x.FromAssemblyContaining(typeof(User));
                x.FromCallingAssembly();

                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(ICommandHandler<>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IViewModelFactory<,>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IViewModelFactory<>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IEventHandler<>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(ICommandFactory<,>)));
                x.BindingGenerators.Add(new GenericBindingGenerator(typeof(IRepository<>)));
                x.BindingGenerators.Add(new DefaultBindingGenerator());
            });
        }

        #endregion

    }
}