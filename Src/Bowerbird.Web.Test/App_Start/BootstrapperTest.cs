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

namespace Bowerbird.Web.Test.App_Start
{
    #region Namespaces

    using System;
    using System.Linq;
    using System.Reflection;
    
    using Ninject;
    using NinjectBootstrapper = Ninject.Web.Mvc.Bootstrapper;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NinjectAdapter;
    
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.Commands;
    using Bowerbird.Web.Config;

    #endregion

    [TestFixture]
    public class BootstrapperTest
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

        #region CommandHandlers

        #endregion

    }
}