using Bowerbird.Core.DesignByContract;
using Bowerbird.Test.Utils;
using Bowerbird.Web.ViewModelFactories;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Public;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Web.Test.ViewModelFactories
{
    [TestFixture]
    public class AccountRegisterFactoryTest
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

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountRegisterFactory(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterFactory_Make_Passing_Null_AccountRegisterInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new AccountRegisterFactory(_mockDocumentSession.Object).Make(null)));
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterFactory_Make_Passing_AccountRegisterInput_Returns_AccountRegister()
        {
            var accountRegisterInput = new AccountRegisterInput()
                                        {
                                           FirstName = FakeValues.FirstName,
                                           LastName = FakeValues.LastName,
                                           Email = FakeValues.Email,
                                           Password = FakeValues.Password
                                        };

            var accountRegister = new AccountRegisterFactory(_mockDocumentSession.Object).Make(accountRegisterInput);

            Assert.AreEqual(accountRegisterInput.FirstName, accountRegister.FirstName);
            Assert.AreEqual(accountRegisterInput.LastName, accountRegister.LastName);
            Assert.AreEqual(accountRegisterInput.Email, accountRegister.Email);
        }

        #endregion					
				
    }
}