/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Commands;
using Bowerbird.Core.Events;
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Bowerbird.Web.Config;
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;
using System.Web.Routing;
using Bowerbird.Core.Config;
using Bowerbird.Core.CommandHandlers;
using Nustache.Mvc;
using Raven.Client.MvcIntegration;
using Raven.Client;

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

            //var mustacheExtensionFormats = new[]
            //{
            //    "~/Views/{1}/{0}.mustache",
            //    "~/Views/Shared/{0}.mustache",
            //    "~/Views/{1}/{0}.html",
            //    "~/Views/Shared/{0}.html"
            //};

            ViewEngines.Engines.Add(new NustacheViewEngine()
            {
                //FileExtensions = mustacheExtensionFormats,
                //MasterLocationFormats = mustacheExtensionFormats,
                //PartialViewLocationFormats = mustacheExtensionFormats,
                //ViewLocationFormats = mustacheExtensionFormats
            });

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            ServiceLocator.Current.GetInstance<ISystemStateManager>().SetupSystem(true);

            RavenProfiler.InitializeFor(ServiceLocator.Current.GetInstance<IDocumentStore>());
        }
    }
}