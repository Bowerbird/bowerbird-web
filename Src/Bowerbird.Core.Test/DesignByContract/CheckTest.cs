using Bowerbird.Core.DesignByContract;
using NUnit.Framework;

namespace Bowerbird.Core.Test.DesignByContract
{
    // TODO: Incomplete Test
    [TestFixture]
    public class CheckTest
    {

        #region Constructor tests


        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test]
        public void Check_RequireNotNullOrWhiteSpace_Passing_TestString_Does_Not_Throw_DesignByContractException()
        {
            Assert.IsFalse(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace("teststring", "teststring")
                ));
        }

        [Test]
        public void Check_RequireNotNullOrWhiteSpace_Passing_EmptyString_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace(string.Empty, "teststring")
                ));
        }

        [Test]
        public void Check_RequireNotNullOrWhiteSpace_Passing_NullString_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace(null, "teststring")
                ));
        }

        [Test]
        public void Check_RequireNotNullOrWhiteSpace_Passing_WhitespaceString_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace("  ", "teststring")
                ));
        }

        [Test]
        public void Check_RequireValidEmail_Passing_A_ValidEmail_Does_Not_Throw_DesignByContractException()
        {
            Assert.IsFalse(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace(FakeValues.Email, "email")
                ));
        }

        [Test]
        public void Check_RequireValidEmail_Passing_An_InValid_Email_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace(FakeValues.InvalidEmail, "email")
                ));
        }

        [Test]
        public void Check_RequireValidEmail_Passing_An_Empty_String_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(() =>
                    Check.RequireNotNullOrWhitespace(string.Empty, "email")
                ));
        }

        #endregion

        #region Helpers


        #endregion

    }
}