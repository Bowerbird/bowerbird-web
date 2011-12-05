using System;
using System.Web.Mvc;
using System.Web.Routing;
using CommonServiceLocator.NinjectAdapter;
//using FluentValidation.Attributes;
//using Ninject.Web.Mvc.FluentValidation;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using Ninject.Web.Mvc;
//using FluentValidation.Mvc;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Bowerbird.Core;

namespace Bowerbird.Web.Config
{
    public class BowerbirdHttpApplication : NinjectHttpApplication
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        protected override void OnApplicationStarted()
        {
            XmlConfigurator.Configure();

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());

            //FluentValidationModelValidatorProvider.Configure(x => x.ValidatorFactory = new NinjectValidatorFactory(Kernel));

            //ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            //ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new NinjectValidatorFactory(Kernel))); 

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            AreaRegistration.RegisterAllAreas();

            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            EventProcessor.ServiceLocator = ServiceLocator.Current;

            //IndexCreation.CreateIndexes(typeof(ImageTags_GroupByTagName).Assembly, documentStore);
        }

        protected override IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel(new BowerbirdNinjectModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));

            return kernel;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
//            // Log the exception.            
//            ILog log = LogManager.GetLogger(typeof(BowerbirdHttpApplication));
//            Exception exception = Server.GetLastError();
//            log.Error("An error occurred", exception);

//#if !DEBUG
//            RouteRegistrar.RouteError(this, exception);
//#endif
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
        }

        #endregion

    }
}
