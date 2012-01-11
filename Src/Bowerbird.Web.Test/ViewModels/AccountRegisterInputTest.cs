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
using DataAnnotationsExtensions;

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
    using Raven.Client;
    using Bowerbird.Web.Validators;

    #endregion

    [TestFixture] 
    public class AccountRegisterInputTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

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
        public void AccountRegisterInput_FirstName_Is_Required()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("FirstName");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(RequiredAttribute), false)
                .Cast<RequiredAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_LastName_Is_Required()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("LastName");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(RequiredAttribute), false)
                .Cast<RequiredAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Email_Is_Required()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("Email");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(RequiredAttribute), false)
                .Cast<RequiredAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Email_Must_Be_Valid_Format()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("Email");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(EmailAttribute), false)
                .Cast<EmailAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Email_Must_Be_Unique()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("Email");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(UniqueEmailAttribute), false)
                .Cast<UniqueEmailAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Password_Is_Required()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("Password");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(RequiredAttribute), false)
                .Cast<RequiredAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
        }

        [Test, Category(TestCategory.Unit)]
        public void AccountRegisterInput_Password_Must_Be_Greater_Than_Six_Characters()
        {
            var propertyInfo = typeof(AccountRegisterInput).GetProperty("Password");

            var attribute = propertyInfo
                .GetCustomAttributes(typeof(StringLengthAttribute), false)
                .Cast<StringLengthAttribute>()
                .FirstOrDefault();

            Assert.NotNull(attribute);
            Assert.AreEqual(6, attribute.MinimumLength);
        }

        #endregion					
    }
}