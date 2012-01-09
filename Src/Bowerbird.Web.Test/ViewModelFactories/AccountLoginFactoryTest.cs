using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Web.Test.ViewModelFactories
{
    [TestFixture]
    public class AccountLoginFactoryTest
    {
            
        #region Infrastructure

        private Mock<IUserContext> _mockUserContext;
        private Mock<IDocumentSession> _mockDocumentSession;

        [SetUp]
        public void TestInitialize()
        {
            _mockUserContext = new Mock<IUserContext>();
            _mockDocumentSession = new Mock<IDocumentSession>();
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
        public void AccountLoginFactory_Constructor_Passing_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountLoginFactory(null, new Mock<IDocumentSession>().Object)));
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountLoginFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountLoginFactory(_mockUserContext.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)]
        public void AccountLoginFactory_Make_Passing_Null_AccountLoginInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountLoginFactory(_mockUserContext.Object, _mockDocumentSession.Object).Make(null)));
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountLoginFactory_Make_Passing_AccountLoginInput_Returns_AccountLogin()
        {
            var accountLoginInput = new AccountLoginInput()
                                        {
                                            Email = FakeValues.Email,
                                            RememberMe = FakeValues.IsTrue,
                                            ReturnUrl = FakeValues.Website
                                        };

            var accountLogin = new AccountLoginFactory(_mockUserContext.Object, _mockDocumentSession.Object).Make(accountLoginInput);

            Assert.AreEqual(accountLoginInput.Email, accountLogin.Email);
            Assert.AreEqual(accountLoginInput.RememberMe, accountLogin.RememberMe);
            Assert.AreEqual(accountLoginInput.ReturnUrl, accountLogin.ReturnUrl);
        }

        #endregion					
				
    }
}