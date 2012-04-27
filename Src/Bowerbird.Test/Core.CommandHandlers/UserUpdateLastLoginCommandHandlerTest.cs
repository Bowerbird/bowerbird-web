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
using System.Threading;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class UserUpdateLastLoginCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;             
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Method tests

        [Test, Ignore]// TODO: Work out why extension method is not working for document session
        [Category(TestCategory.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle()
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

                // wait a second.
                Thread.Sleep(1000);

                var userUpdateLastLoginCommandHandler = new UserUpdateLastLoginCommandHandler(session);

                userUpdateLastLoginCommandHandler.Handle(new UserUpdateLastLoginCommand()
                       {
                           Email = user.Email
                       });

                session.SaveChanges();

                newValue = session.Load<User>(user.Id).LastLoggedIn;
            }

            Assert.AreNotEqual(originalValue, newValue);
            Assert.Greater(newValue, originalValue);
        }

        #endregion 
    }
}