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

    using Microsoft.Practices.ServiceLocation;
    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;

    #endregion

    [TestFixture] 
    public class ViewModelRepositoryTest
    {
        #region Infrastructure

        [SetUp] 
        public void TestInitialize()
        {
        }

        [TearDown] 
        public void TestCleanup()
        {
        }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_Passing_Null_Input_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ViewModelRepository(new Mock<IServiceLocator>().Object).Load<HomeIndexInput, HomeIndex>(null)));
        }

        [Test]
        [Category(TestCategory.Integration)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_With_ServiceLocator_Return_Null_Throws_Exception()
        {
            IViewModelFactory<HomeIndexInput, HomeIndex> factory = null;

            var mockServiceLocator = new Mock<IServiceLocator>();

            mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndexInput, HomeIndex>>()).Returns(factory);

            Assert.IsTrue(BowerbirdThrows.Exception<Exception>(() => new ViewModelRepository(mockServiceLocator.Object).Load<HomeIndexInput, HomeIndex>(new HomeIndexInput())));
        }

        [Test]
        [Category(TestCategory.Integration)] 
        public void ViewModelRepository_Load_Having_ReturnViewModel_With_ServiceLocator_Return_Null_Throws_Exception()
        {
            IViewModelFactory<HomeIndex> factory = null;

            var mockServiceLocator = new Mock<IServiceLocator>();

            mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndex>>()).Returns(factory);

            Assert.IsTrue(BowerbirdThrows.Exception<Exception>(() => new ViewModelRepository(mockServiceLocator.Object).Load<HomeIndex>()));
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_Passing_InputViewModel_Returns_ViewModel()
        {
            var mockServiceLocator = new Mock<IServiceLocator>();

            var mockViewModelFactory = new Mock<IViewModelFactory<HomeIndexInput, HomeIndex>>();

            mockServiceLocator
                .Setup(x => x.GetInstance<IViewModelFactory<HomeIndexInput, HomeIndex>>())
                .Returns(mockViewModelFactory.Object);

            mockViewModelFactory
                .Setup(x => x.Make(It.IsAny<HomeIndexInput>()))
                .Returns(new HomeIndex());

            var repository = new ViewModelRepository(mockServiceLocator.Object);

            var result = repository.Load<HomeIndexInput, HomeIndex>(new HomeIndexInput());

            Assert.IsInstanceOf<HomeIndex>(result);
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void ViewModelRepository_Load_Having_ReturnViewModel_Returns_ViewModel()
        {
            var factory = new Mock<IViewModelFactory<HomeIndex>>();
            factory.Setup(x => x.Make()).Returns(new HomeIndex());

            var mockServiceLocator = new Mock<IServiceLocator>();

            mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndex>>()).Returns(factory.Object);

            var model = new ViewModelRepository(mockServiceLocator.Object).Load<HomeIndex>();

            Assert.IsInstanceOf<HomeIndex>(model);
        }

        #endregion
    }
}