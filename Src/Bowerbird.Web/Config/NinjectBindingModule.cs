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

using Ninject.Modules;
using Raven.Client;
using Bowerbird.Core.CommandHandlers;
using Ninject.Web.Common;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.BindingBuilder;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Extensions.Conventions.Syntax;
using Microsoft.Practices.ServiceLocation;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.DomainModels;
using SignalR;
using Bowerbird.Core.Config;
using System.Web;
using Bowerbird.Web.Hubs;
using SignalR.Hubs;
using System.Linq;

namespace Bowerbird.Web.Config
{
    public class NinjectBindingModule : NinjectModule
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
            Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();
            Bind<IPermissionChecker>().To<PermissionChecker>().InSingletonScope().OnActivation(x => ((PermissionChecker)x).Init());
            Bind<ISystemStateManager>().To<SystemStateManager>().InSingletonScope();

            // Request scope
            Bind<IDocumentSession>().ToProvider<NinjectRavenSessionProvider>().InRequestScope();

            // Transient scope
            Bind<IServiceLocator>().ToMethod(x => ServiceLocator.Current);
            Bind<IHubContext>().ToMethod(x => GlobalHost.ConnectionManager.GetHubContext<ActivityHub>()).When(x => 
                x.Target.GetCustomAttributes(typeof(HubContextAttribute), false).Cast<HubContextAttribute>().Any(y => y.HubType == typeof(ActivityHub))
            );

            // Convention based mappings
            Kernel.Bind(x => x
                .FromAssemblyContaining(typeof(User), typeof(NinjectBindingModule))
                .SelectAllClasses()
                .Excluding<PermissionChecker>()
                .Excluding<SystemStateManager>()
                .BindAllInterfaces());
        }

        #endregion

    }
}