/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Test.DomainModels
{
    [TestFixture]
    public class UserTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests
        
        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_Password_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName
                );

            var actual = user.ValidatePassword(FakeValues.Password);
            var expected = true;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_Email_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName
                );

            var expected = FakeValues.Email;
            var actual = user.Email;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_FirstName_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName
                );

            var expected = FakeValues.FirstName;
            var actual = user.FirstName;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_LastName_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName
                );

            var expected = FakeValues.LastName;
            var actual = user.LastName;

            Assert.AreEqual(actual, expected);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void User_ValidPassword_WithValidPassword()
        {
            Assert.IsTrue(FakeObjects.TestUser().ValidatePassword(FakeValues.Password));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_ValidPassword_WithInValidPassword()
        {
            Assert.IsFalse(FakeObjects.TestUser().ValidatePassword(FakeValues.InvalidPassword));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateEmail_WithValidEmail()
        {
            Assert.IsTrue(FakeObjects.TestUser()
                .UpdateEmail("new@email.com")
                .Email
                .Equals("new@email.com"));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateEmail_WithInValidEmail()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                FakeObjects.TestUser()
                .UpdateEmail(FakeValues.InvalidEmail)
                ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdatePassword()
        {
            // try new correct password
            Assert.IsTrue(
                FakeObjects.TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword("newpassword")
                );

            // try old password
            Assert.IsFalse(
                FakeObjects.TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword(FakeValues.Password)
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateLastLoggedIn()
        {
            var testUser = FakeObjects.TestUser();
            var initialLastLoggedIn = testUser.LastLoggedIn;

            Assert.IsTrue(
                testUser
                .WaitForASecond() //ensure not same clock-cycle
                .UpdateLastLoggedIn()
                .LastLoggedIn
                .IsMoreRecentThan(initialLastLoggedIn)
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateResetPasswordKey()
        {
            var testUser = FakeObjects.TestUser();

            var originalValue = testUser
                .ResetPasswordKey;

            var newValue = testUser
                .RequestPasswordReset()
                .ResetPasswordKey;

            Assert.AreNotEqual(originalValue, newValue);
            Assert.IsInstanceOf<string>(newValue);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_RequestPasswordReset_Then_UpdatePassword_Sets_ResetPasswordKey_To_Null()
        {
            // Request Password reset sets ResetPasswordKey. Updating password resets key back to null.
            var result = FakeObjects
                .TestUser()
                .RequestPasswordReset()
                .UpdatePassword("newpassword");

            Assert.IsNull(result.ResetPasswordKey);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_IncrementFlagsRaised()
        {
            Assert.AreEqual(
                FakeObjects.TestUser()
                .IncrementFlagsRaised()
                .FlagsRaised,
                FakeObjects.TestUser().FlagsRaised + 1
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_IncrementFlaggedItemsOwned()
        {
            Assert.AreEqual(
                FakeObjects.TestUser()
                .IncrementFlaggedItemsOwned()
                .FlaggedItemsOwned,
                FakeObjects.TestUser().FlaggedItemsOwned + 1
                );
        }

        #endregion
    }
}