using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
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

        private Mock<IDocumentSession> _mockDocumentSession;

        [SetUp]
        public void TestInitialize()
        {
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

        [Test, Category(TestCategories.Unit)]
        public void AccountLoginFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountLoginFactory(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategories.Unit)]
        public void AccountLoginFactory_Make_Passing_Null_AccountLoginInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>new AccountLoginFactory(_mockDocumentSession.Object).Make(null)));
        }

        [Test, Category(TestCategories.Unit)]
        public void AccountLoginFactory_Make_Passing_AccountLoginInput_Returns_AccountLogin()
        {
            var accountLoginInput = new AccountLoginInput()
                                        {
                                            Username = FakeValues.UserName,
                                            RememberMe = FakeValues.IsTrue,
                                            ReturnUrl = FakeValues.Website
                                        };

            var accountLogin = new AccountLoginFactory(_mockDocumentSession.Object).Make(accountLoginInput);

            Assert.AreEqual(accountLoginInput.Username, accountLogin.Username);
            Assert.AreEqual(accountLoginInput.RememberMe, accountLogin.RememberMe);
            Assert.AreEqual(accountLoginInput.ReturnUrl, accountLogin.ReturnUrl);
        }

        #endregion					
				
    }
}