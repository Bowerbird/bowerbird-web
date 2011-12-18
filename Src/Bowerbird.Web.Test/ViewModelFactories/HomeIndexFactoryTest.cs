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
				
namespace Bowerbird.Web.Test.ViewModelFactories
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;

    #endregion

    [TestFixture] public class HomeIndexFactoryTest
    {

        #region Test Infrastructure

        private IDocumentStore _store;
        private Mock<IPagedListFactory> _mockPagedListFactory;
        private HomeIndexFactory _homeIndexFactory;

        [SetUp] public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
            _mockPagedListFactory = new Mock<IPagedListFactory>();
        }

        [TearDown] public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test] public void HomeIndexFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                        BowerbirdThrows.Exception<DesignByContractException>(
                            () => new HomeIndexFactory(
                                null,
                                new Mock<IPagedListFactory>().Object
                            )));
        }

        [Test] public void HomeIndexFactory_Constructor_Passing_Null_PagedListFactory_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                        BowerbirdThrows.Exception<DesignByContractException>(
                            () => new HomeIndexFactory(
                                new Mock<IDocumentSession>().Object,
                                null
                            )));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test,Ignore] public void HomeIndexFactory_Make_Passing_HomeIndexInput_Returns_HomeIndex()
        {
            var homeIndexInput = new
            {
                Page = FakeValues.Page,
                PageSize = FakeValues.PageSize,
                Username = FakeValues.UserName
            };

            //_mockPagedListFactory.Setup(x => x.Make<StreamItem>(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<StreamItem>>, It.IsAny<IDictionary<int, string>>)).Returns(new HomeIndex());

            using (var session = _store.OpenSession())
            {
                _homeIndexFactory = new HomeIndexFactory(session, _mockPagedListFactory.Object);
            }
        }

        #endregion					
				
    }
}