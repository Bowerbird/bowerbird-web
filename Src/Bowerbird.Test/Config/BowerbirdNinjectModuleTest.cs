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

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Core.Events;
using Bowerbird.Web.EventHandlers;

namespace Bowerbird.Test.Config
{
    [TestFixture] 
    public class BowerbirdNinjectModuleTest
    {
        #region Test Infrastructure

        [SetUp] 
        public void TestInitialize()
        {
            BootstrapperHelper.Startup();
        }

        [TearDown] 
        public void TestCleanup()
        {
            BootstrapperHelper.Shutdown();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #region Event Handlers

        //[Test, Category(TestCategory.Unit), Ignore] 
        //public void BowerbirdNinjectModule_Binds_NotifyActiviyObservationCreatedEventHandler_To_DomainModelCreatedEvent_Having_Observation()
        //{
        //    Assert.IsInstanceOf<NotifyActivityObservationCreatedEventHandler>(ServiceLocator.Current.GetInstance<IEventHandler<DomainModelCreatedEvent<Observation>>>());
        //}

        //[Test, Category(TestCategory.Unit), Ignore]
        //public void BowerbirdNinjectModule_Binds_NotifyActivityUserLoggedInEventHandler_To_UserLoggedInEvent()
        //{
        //    Assert.IsInstanceOf<NotifyActivityUserLoggedInEventHandler>(ServiceLocator.Current.GetInstance<IEventHandler<UserLoggedInEvent>>());
        //}

        #endregion

        #region Dynamic Bindings

        [Test]
        [Category(TestCategory.Unit), Ignore] 
        public void BowerbirdNinjectModule_Binds_CommandHandler_Interface_To_CommandHandler_Instance()
        {
            Assert.IsInstanceOf<ICommandHandler<UserCreateCommand>>(ServiceLocator.Current.GetInstance<ICommandHandler<UserCreateCommand>>());
        }

        #endregion

        #endregion

    }
}