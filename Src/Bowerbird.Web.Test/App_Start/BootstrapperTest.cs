using System;
using System.Linq;
using System.Reflection;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Web.Config;
using Ninject;
using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using CommonServiceLocator.NinjectAdapter;

namespace Bowerbird.Web.Test.App_Start
{
    [TestFixture]
    public class BootstrapperTest
    {

        #region Test Infrastructure

        private static readonly NinjectBootstrapper _ninjectBootstrapper = new NinjectBootstrapper();

        private static IKernel _kernel;

        [SetUp]
        public void TestInitialize()
        {
            _ninjectBootstrapper.Initialize(CreateKernel);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static IKernel CreateKernel()
        {
            _kernel = new StandardKernel();

            RegisterServices(_kernel);

            return _kernel;
        }

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new BowerbirdNinjectModule());

            ServiceLocator.SetLocatorProvider(() => new NinjectServiceLocator(kernel));
        }

        #endregion

        #region CommandHandlers

        [Test]
        public void Bootstrapper_NinjectModule_Binds_ObservationCreateCommand_To_ObservationCreateCommandHandler()
        {
            Assert.IsTrue(
                _kernel
                    .GetBindings(typeof (ICommandHandler<ObservationCreateCommand>))
                    .FirstOrDefault()
                    .Service
                    .GetInterfaces()
                    .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<ObservationCreateCommand>)));
        }

        #endregion  

    }
}
