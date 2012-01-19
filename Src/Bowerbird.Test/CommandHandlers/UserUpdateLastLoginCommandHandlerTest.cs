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
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class UserUpdateLastLoginCommandHandlerTest
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

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_UserUpdateLastLoginCommand_Updates_User_LastLoggedIn_Property()
        {
            DateTime originalValue;
            DateTime newValue;

            using (var session = _store.OpenSession())
            {
                var user = FakeObjects.TestUserWithId();
                user.UpdateLastLoggedIn();
                originalValue = user.LastLoggedIn;

                session.Store(user);

                session.SaveChanges();

                var userUpdateLastLoginCommandHandler = new UserUpdateLastLoginCommandHandler(session);

                userUpdateLastLoginCommandHandler.Handle(new UserUpdateLastLoginCommand()
                       {
                           Email = FakeValues.Email
                       });

                session.SaveChanges();

                newValue = session.Load<User>("users/" + FakeValues.UserId).LastLoggedIn;
            }

            Assert.AreNotEqual(originalValue, newValue);
            Assert.Greater(newValue, originalValue);
        }

        #endregion 
    }
}