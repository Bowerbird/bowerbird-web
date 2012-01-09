/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Commands
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserCreateCommandTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        public UserCreateCommand TestUserCreateCommand()
        {
            return new UserCreateCommand()
                       {
                           Description = FakeValues.Description,
                           Email = FakeValues.Email,
                           FirstName = FakeValues.FirstName,
                           LastName = FakeValues.LastName,
                           Password = FakeValues.Password,
                           Roles = new List<string>(){"Member"}
                       };
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_Description_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUserCreateCommand().Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_Email_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUserCreateCommand().Email);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_FirstName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUserCreateCommand().FirstName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_LastName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUserCreateCommand().LastName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_Password_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUserCreateCommand().Password);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_Roles_Is_TypeOf_List_String()
        {
            Assert.IsInstanceOf<List<string>>(TestUserCreateCommand().Roles);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommand_ValidationResults_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new UserCreateCommand().ValidationResults()));
        }

        #endregion 
    }
}