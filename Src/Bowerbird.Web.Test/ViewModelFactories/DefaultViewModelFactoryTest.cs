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

    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;

    #endregion

    [TestFixture] 
    public class DefaultViewModelFactoryTest
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

        [Test, Category(TestCategory.Unit)]
        public void DefaultViewModelFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new DefaultViewModelFactory(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)]
        public void DefaultViewModelFactory_Make_Returns_DefaultViewModel()
        {
            var defaultViewModel = new DefaultViewModelFactory(new Mock<IDocumentSession>().Object).Make();

            Assert.IsInstanceOf<DefaultViewModel>(defaultViewModel);
        }

        #endregion
				
    }
}