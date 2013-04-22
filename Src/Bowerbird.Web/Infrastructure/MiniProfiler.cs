//using System;
//using System.Web;
//using System.Web.Mvc;
//using System.Linq;
//using Bowerbird.Core.Config;
//using Bowerbird.Core.DomainModels;
//using Ninject;
//using Raven.Client;
//using StackExchange.Profiling;
//using StackExchange.Profiling.MVCHelpers;
//using Microsoft.Web.Infrastructure;
//using Microsoft.Web.Infrastructure.DynamicModuleHelper;

//[assembly: WebActivator.PreApplicationStartMethod(typeof(Bowerbird.Web.Infrastructure.MiniProfilerPackage), "PreStart")]
//[assembly: WebActivator.PostApplicationStartMethod(typeof(Bowerbird.Web.Infrastructure.MiniProfilerPackage), "PostStart", Order = 3)]

//namespace Bowerbird.Web.Infrastructure
//{
//    public static class MiniProfilerPackage
//    {
//        public static void PreStart()
//        {

//            // Be sure to restart you ASP.NET Developement server, this code will not run until you do that. 

//            //TODO: See - _MINIPROFILER UPDATED Layout.cshtml
//            //      For profiling to display in the UI you will have to include the line @StackExchange.Profiling.MiniProfiler.RenderIncludes() 
//            //      in your master layout
            
//            //TODO: Non SQL Server based installs can use other formatters like: new StackExchange.Profiling.SqlFormatters.InlineFormatter()
//            //MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();

//            //TODO: To profile a standard DbConnection: 
//            // var profiled = new ProfiledDbConnection(cnn, MiniProfiler.Current);

//            //TODO: If you are profiling EF code first try: 
//            // MiniProfilerEF.Initialize();

//            //Make sure the MiniProfiler handles BeginRequest and EndRequest
//            // FR 10/02/13 - We don't need to register it here, because Ninject Inits all HTTP modules for us inside its uber NinjectHttpModule. This is because
//            // (like many parts of ASP.Net) the design pattern of IHTTPModule is not DI friendly
//            //DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));

//            //Setup profiler for Controllers via a Global ActionFilter
//            GlobalFilters.Filters.Add(new ProfilingActionFilter());

//            // You can use this to check if a request is allowed to view results
//            //MiniProfiler.Settings.Results_Authorize = (request) =>
//            //{
//                // you should implement this if you need to restrict visibility of profiling on a per request basis 
//            //    return !DisableProfilingResults; 
//            //};

//            // the list of all sessions in the store is restricted by default, you must return true to alllow it
//            //MiniProfiler.Settings.Results_List_Authorize = (request) =>
//            //{
//                // you may implement this if you need to restrict visibility of profiling lists on a per request basis 
//                //return true; // all requests are kosher
//            //};
//        }

//        public static void PostStart()
//        {
//            // Intercept ViewEngines to profile all partial views and regular views.
//            // If you prefer to insert your profiling blocks manually you can comment this out
//            var copy = ViewEngines.Engines.ToList();
//            ViewEngines.Engines.Clear();
//            foreach (var item in copy)
//            {
//                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
//            }
//        }
//    }

//    public class MiniProfilerStartupModule : IHttpModule
//    {
//        [Inject]
//        public IPermissionManager PermissionManager { get; set; }

//        public void Init(HttpApplication context)
//        {
//            context.BeginRequest += (sender, e) =>
//            {
//                var request = ((HttpApplication)sender).Request;
//                //TODO: By default only local requests are profiled, optionally you can set it up
//                //  so authenticated users are always profiled
//                if (request.IsLocal) { MiniProfiler.Start(); }
//            };


//            context.AuthenticateRequest += (sender, e) =>
//            {
//                if (((HttpApplication)sender).Context.User != null)
//                {
//                    var userId = ((HttpApplication) sender).Context.User.Identity.Name;
//                    if (!CurrentUserIsAllowedToSeeProfiler(userId))
//                    {
//                        StackExchange.Profiling.MiniProfiler.Stop(discardResults: true);
//                    }
//                }
//            };

//            context.EndRequest += (sender, e) =>
//            { 
//                MiniProfiler.Stop();
//            };
//        }

//        private bool CurrentUserIsAllowedToSeeProfiler(string userId)
//        {
//            return PermissionManager.HasRole(userId, "roles/" + RoleNames.GlobalAdministrator, Constants.AppRootId);
//        }

//        public void Dispose() { }
//    }
//}

