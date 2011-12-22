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

        private Mock<IServiceLocator> _mockServiceLocator;

        [SetUp] 
        public void TestInitialize()
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            BootstrapperHelper.Startup();
        }

        [TearDown] 
        public void TestCleanup()
        {
            BootstrapperHelper.Shutdown();            
        }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelRepository_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new CommandBuilder(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_Passing_Null_Input_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ViewModelRepository(ServiceLocator.Current).Load<HomeIndexInput, HomeIndex>(null)));
        }

        [Test, Category(TestCategories.Integration)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_With_ServiceLocator_Return_Null_Throws_Exception()
        {
            IViewModelFactory<HomeIndexInput, HomeIndex> factory = null;

            _mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndexInput, HomeIndex>>()).Returns(factory);

            Assert.IsTrue(BowerbirdThrows.Exception<Exception>(() => new ViewModelRepository(_mockServiceLocator.Object).Load<HomeIndexInput, HomeIndex>(new HomeIndexInput())));
        }
  
        [Test, Category(TestCategories.Integration)] 
        public void ViewModelRepository_Load_Having_ReturnViewModel_With_ServiceLocator_Return_Null_Throws_Exception()
        {
            IViewModelFactory<HomeIndex> factory = null;

            _mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndex>>()).Returns(factory);

            Assert.IsTrue(BowerbirdThrows.Exception<Exception>(() => new ViewModelRepository(_mockServiceLocator.Object).Load<HomeIndex>()));
        }

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_Passing_InputViewModel_Returns_ViewModel()
        {
            var repository = new ViewModelRepository(ServiceLocator.Current);

            Assert.IsInstanceOf<HomeIndex>(repository.Load<HomeIndexInput, HomeIndex>(new HomeIndexInput()));
        }

        [Test, Category(TestCategories.Integration)] 
        public void ViewModelRepository_Load_Having_InputViewModel_And_ReturnViewModel_Calls_ViewModelFactory_Make()
        {
            var factory = new Mock<IViewModelFactory<HomeIndex>>();
            factory.Setup(x => x.Make()).Returns(new HomeIndex());

            _mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndex>>()).Returns(factory.Object);

            var model = new ViewModelRepository(_mockServiceLocator.Object).Load<HomeIndex>();

            factory.Verify(x => x.Make(), Times.Once());
        }

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelRepository_Load_Having_ReturnViewModel_Returns_ViewModel()
        {
            var repository = new ViewModelRepository(ServiceLocator.Current);

            Assert.IsInstanceOf<HomeIndex>(repository.Load<HomeIndex>());
        }


        [Test, Category(TestCategories.Integration)]
        public void ViewModelRepository_Load_Having_ReturnViewModel_Calls_ViewModelFactory_Make()
        {
            var factory = new Mock<IViewModelFactory<HomeIndexInput,HomeIndex>>();
            factory.Setup(x => x.Make(It.IsAny<HomeIndexInput>())).Returns(new HomeIndex());

            _mockServiceLocator.Setup(x => x.GetInstance<IViewModelFactory<HomeIndexInput,HomeIndex>>()).Returns(factory.Object);

            var model = new ViewModelRepository(_mockServiceLocator.Object).Load<HomeIndexInput,HomeIndex>(new HomeIndexInput());

            factory.Verify(x => x.Make(It.IsAny<HomeIndexInput>()), Times.Once());
        }

        #endregion
    }
}