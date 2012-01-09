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

using System.ComponentModel.DataAnnotations;

namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class AccountRegisterInputTest
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

        #endregion

        #region Property tests

        [Test, Category(TestCategory.Unit)] 
        public void AccountRegisterInput_FirstName_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountRegisterInput() { FirstName = FakeValues.FirstName }.FirstName);
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountRegisterInput_LastName_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountRegisterInput() { LastName = FakeValues.LastName }.LastName);
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountRegisterInput_Email_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountRegisterInput() { Email = FakeValues.Email }.Email);
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountRegisterInput_Password_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountRegisterInput() { Password = FakeValues.Password }.Password);
        }

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_FirstName_Is_Invalid_When_Empty()
        {
            var accountRegisterInput = new AccountRegisterInput()
                                           {
                                               FirstName = string.Empty,
                                               LastName = FakeValues.LastName,
                                               Email = FakeValues.Email,
                                               Password = FakeValues.Password
                                           };

            var validationResults = ValidationHelper.ValidateModel(accountRegisterInput);

            Assert.IsTrue(validationResults.Count == 1);
            Assert.IsTrue(validationResults[0].MemberNames.First() == "FirstName");
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_LastName_Is_Invalid_When_Empty()
        {
            var accountRegisterInput = new AccountRegisterInput()
            {
                FirstName = FakeValues.FirstName,
                LastName = string.Empty,
                Email = FakeValues.Email,
                Password = FakeValues.Password
            };

            var validationResults = ValidationHelper.ValidateModel(accountRegisterInput);

            Assert.IsTrue(validationResults.Count == 1);
            Assert.IsTrue(validationResults[0].MemberNames.First() == "LastName");
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Email_Is_Invalid_When_Empty()
        {
            var accountRegisterInput = new AccountRegisterInput()
            {
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName,
                Email = string.Empty,
                Password = FakeValues.Password
            };

            var validationResults = ValidationHelper.ValidateModel(accountRegisterInput);

            Assert.IsTrue(validationResults.Count == 1);
            Assert.IsTrue(validationResults[0].MemberNames.First() == "Email");
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Email_Is_Invalid_When_WrongEmailFormat()
        {
            var accountRegisterInput = new AccountRegisterInput()
            {
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName,
                Email = FakeValues.InvalidEmail,
                Password = FakeValues.Password
            };

            var validationResults = ValidationHelper.ValidateModel(accountRegisterInput);

            Assert.IsTrue(validationResults.Count == 1);
            Assert.IsTrue(validationResults[0].MemberNames.First() == "Email");
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Password_Is_Invalid_When_Empty()
        {
            var accountRegisterInput = new AccountRegisterInput()
            {
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName,
                Email = FakeValues.Email,
                Password = string.Empty
            };

            var validationResults = ValidationHelper.ValidateModel(accountRegisterInput);

            Assert.IsTrue(validationResults.Count == 1);
            Assert.IsTrue(validationResults[0].MemberNames.First() == "Password");
        }

        #endregion					
    }
}