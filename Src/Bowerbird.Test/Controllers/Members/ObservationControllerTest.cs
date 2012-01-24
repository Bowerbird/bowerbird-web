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

using Bowerbird.Core.Commands;
using Bowerbird.Web.Controllers.Members;
using NUnit.Framework;
using Moq;

namespace Bowerbird.Test.Controllers.Members
{
    #region Namespaces

    #endregion

    [TestFixture]
    public class ObservationControllerTest
    {
        #region Test Infrastructure

        private Mock<ICommandProcessor> _mockCommandProcessor;
        private ObservationController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _mockCommandProcessor = new Mock<ICommandProcessor>();
            _controller = new ObservationController(
                _mockCommandProcessor.Object,
                null,
                null
                );
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        //[Test, Category(TestCategory.Unit)]
        //public void ObservationController_HttpGet_Index_Having_Valid_ObservationId_Returns_ObservationViewModel()
        //{
        //    var accountLogin = new AccountLogin() { Email = string.Empty };

        //    _mockUserContext.Setup(x => x.IsUserAuthenticated()).Returns(FakeValues.IsFalse);
        //    _mockUserContext.Setup(x => x.HasEmailCookieValue()).Returns(FakeValues.IsFalse);

        //    _controller.Login();

        //    var viewModel = _controller.ViewData.Model;

        //    Assert.IsInstanceOf<AccountLogin>(viewModel);
        //    Assert.IsTrue(((AccountLogin)viewModel).Email.Equals(string.Empty));
        //}

        #endregion
    }
}