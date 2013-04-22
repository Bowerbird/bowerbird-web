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
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Infrastructure;
using Bowerbird.Core.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Factory;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;

namespace Bowerbird.Web.Infrastructure
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
            Bind<ISystemStateManager>().To<SystemStateManager>().InSingletonScope();

            // Request scope
            //Bind<IDocumentSession>().ToProvider<NinjectRavenSessionProvider>().InRequestScope().OnActivation((x) => Debug.WriteLine("HTTP Request Document Session instantiated."));

            // Transient scope
            Bind<IConnectionManager>().ToMethod(x => GlobalHost.ConnectionManager);
            Bind<IMediaServiceFactory>().ToFactory();
            Bind<IMessageBus>().ToProvider<NinjectMessageBusProvider>();
            Bind<IDocumentSession>().ToProvider<NinjectRavenSessionProvider>();

            // Thread scope
            // HACK: Experimental loading of chat components into new async thread
            Bind<Bowerbird.Core.CommandHandlers.ICommandHandler<Bowerbird.Core.Commands.ChatCreateCommand>>().To<Bowerbird.Core.CommandHandlers.ChatCreateCommandHandler>().OnActivation((x) => Debug.WriteLine("ChatCreateCommandHandler instantiated.")).DefinesNamedScope("ASYNC");
            Bind<Bowerbird.Core.CommandHandlers.ICommandHandler<Bowerbird.Core.Commands.ChatUpdateCommand>>().To<Bowerbird.Core.CommandHandlers.ChatUpdateCommandHandler>().DefinesNamedScope("ASYNC");
            Bind<Bowerbird.Core.CommandHandlers.ICommandHandler<Bowerbird.Core.Commands.ChatDeleteCommand>>().To<Bowerbird.Core.CommandHandlers.ChatDeleteCommandHandler>().DefinesNamedScope("ASYNC");
            Bind<Bowerbird.Core.CommandHandlers.ICommandHandler<Bowerbird.Core.Commands.ChatMessageCreateCommand>>().To<Bowerbird.Core.CommandHandlers.ChatMessageCreateCommandHandler>().DefinesNamedScope("ASYNC");
            Bind<IDocumentSession>().ToProvider<NinjectRavenSessionProvider>().WhenAnyAnchestorNamed("ASYNC").InTransientScope().OnActivation((x) => Debug.WriteLine("Async Document Session instantiated."));

            //Bind<IEventHandlerFactory>().ToFactory().DefinesNamedScope("ASYNC");

            // HACK: Experimental loading of chat components into new async thread
            Bind(
                typeof(Bowerbird.Core.EventHandlers.IEventHandler<Bowerbird.Core.Events.DomainModelCreatedEvent<Bowerbird.Core.DomainModels.Chat>>),
                typeof(Bowerbird.Core.EventHandlers.IEventHandler<Bowerbird.Core.Events.DomainModelCreatedEvent<Bowerbird.Core.DomainModels.ChatMessage>>),
                typeof(Bowerbird.Core.EventHandlers.IEventHandler<Bowerbird.Core.Events.UserJoinedChatEvent>),
                typeof(Bowerbird.Core.EventHandlers.IEventHandler<Bowerbird.Core.Events.UserExitedChatEvent>))
                .To<Bowerbird.Core.EventHandlers.ChatUpdated>().OnActivation((x) => Debug.WriteLine("ChatUpdated instantiated.")).DefinesNamedScope("ASYNC");

            // Convention based mappings
            Kernel.Bind(x => 
                {
                    x
                    .FromAssemblyContaining(typeof(User), typeof(NinjectBindingModule))
                    .SelectAllClasses()
                    .Excluding<SystemStateManager>()
                    .Excluding<Bowerbird.Core.CommandHandlers.ChatCreateCommandHandler>() // HACK: Experimental loading of chat components into new async thread
                    .Excluding<Bowerbird.Core.CommandHandlers.ChatUpdateCommandHandler>()
                    .Excluding<Bowerbird.Core.CommandHandlers.ChatDeleteCommandHandler>()
                    .Excluding<Bowerbird.Core.CommandHandlers.ChatMessageCreateCommandHandler>()
                    .Excluding<Bowerbird.Core.EventHandlers.ChatUpdated>()
                    .Excluding<MessageBus>()
                    .BindAllInterfaces();

                    //x
                    //.FromAssemblyContaining<IEventHandlerFactory>()
                    //.Select(y => y == typeof(IEventHandlerFactory))
                    //.BindToFactory().Configure(z => z.DefinesNamedScope("ASYNC"));
                });
        }

        #endregion

    }
}
