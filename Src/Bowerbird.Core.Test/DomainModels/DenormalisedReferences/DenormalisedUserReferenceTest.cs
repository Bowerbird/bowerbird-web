/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels.DenormalisedReferences
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class DenormalisedUserReferenceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static DenormalisedUserReference TestDenormalise(User user)
        {
            DenormalisedUserReference denormalisedUserReference = user;

            return denormalisedUserReference;
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests
        
        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedUserReference_Implicit_Operator_Passing_User_Returns_DenormalisedUserReference_With_Populated_Properties()
        {
            var normalisedUser = FakeObjects.TestUser();

            DenormalisedUserReference denormalisedUser = TestDenormalise(normalisedUser);

            Assert.AreEqual(normalisedUser.Id, denormalisedUser.Id);
            Assert.AreEqual(normalisedUser.FirstName, denormalisedUser.FirstName);
            Assert.AreEqual(normalisedUser.LastName, denormalisedUser.LastName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void DenormalisedUserReference_Implicit_Operator_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => TestDenormalise(null)));
        }

        #endregion 
    }
}