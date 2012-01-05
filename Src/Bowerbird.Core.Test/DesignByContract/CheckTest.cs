/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DesignByContract
{
    #region Namespaces

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class CheckTest
    {
        #region Test Infrastructure

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireNotNullOrWhiteSpace_Passing_TestString_Does_Not_Throw_DesignByContractException()
        {
            Assert.IsFalse(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireNotNullOrWhitespace("teststring", "teststring")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireNotNullOrWhiteSpace_Passing_EmptyString_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireNotNullOrWhitespace(string.Empty, "teststring")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireNotNullOrWhiteSpace_Passing_NullString_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireNotNullOrWhitespace(null, "teststring")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireNotNullOrWhiteSpace_Passing_WhitespaceString_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireNotNullOrWhitespace("  ", "teststring")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireValidEmail_Passing_A_ValidEmail_Does_Not_Throw_DesignByContractException()
        {
            Assert.IsFalse(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireValidEmail(FakeValues.Email, "email")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireValidEmail_Passing_An_InValid_Email_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireValidEmail(FakeValues.InvalidEmail, "email")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireValidEmail_Passing_An_Empty_String_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>Check.RequireValidEmail(string.Empty, "email")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireGreaterThanZero_Passing_LessThan_Zero_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => Check.RequireGreaterThanZero(-1, "number")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireGreaterThanZero_Passing_Zero_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => Check.RequireGreaterThanZero(0, "number")));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void Check_RequireGreaterThanZero_Passing_One_DoesNotThrow_DesignByContractException()
        {
            Assert.IsFalse(BowerbirdThrows.Exception<DesignByContractException>(() => Check.RequireGreaterThanZero(1, "number")));
        }

        #endregion
    }
}