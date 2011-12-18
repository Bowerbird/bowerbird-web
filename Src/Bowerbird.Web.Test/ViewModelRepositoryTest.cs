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
				
namespace Bowerbird.Web.Test
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;

    using Bowerbird.Web;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;

    #endregion

    [TestFixture] public class ViewModelRepositoryTest
    {

        #region Infrastructure

        private Mock<IServiceLocator> _mockServiceLocator;
        private Mock<HomeIndex> _mockHomeIndex;
        private Mock<HomeIndexInput> _mockHomeIndexInput;

        [SetUp] public void TestInitialize()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();
            _mockHomeIndex = new Mock<HomeIndex>();
            _mockHomeIndexInput = new Mock<HomeIndexInput>();
        }

        [TearDown] public void TestCleanup() { }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        [Test] public void ViewModelRepository_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                        BowerbirdThrows.Exception<DesignByContractException>(
                            () => new CommandBuilder(null)));
        }

        #endregion

        #region Property tests

        //[Test]
        //public void ViewModelRepository_

        #endregion

        #region Method tests



        #endregion					
				
    }
}