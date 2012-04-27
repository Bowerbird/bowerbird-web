/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.CommandHandlers
{
    [TestFixture]
    public class UserCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            // start raven without seeding with data
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

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Integration)]
        public void UserCreateCommandHandler_Handle()
        {
            var permissions = FakeObjects.TestPermissions();
            var roles = FakeObjects.TestRoles();

            User newUser = null;
            using (var session = _store.OpenSession())
            {
                foreach (var permission in permissions) session.Store(permission);
                foreach (var role in roles) session.Store(role);

                session.SaveChanges();

                var commandHandler = new UserCreateCommandHandler(session);

                commandHandler.Handle(new UserCreateCommand()
                {
                    Email = FakeValues.Email.PrependWith("new"),
                    Description = FakeValues.Description,
                    FirstName = FakeValues.FirstName,
                    LastName = FakeValues.LastName,
                    Password = FakeValues.Password,
                    Roles = FakeObjects.TestRoles().Select(x => x.Id)
                });

                session.SaveChanges();

                newUser = session.Query<User>()
                    .Where(x => x.Email == FakeValues.Email.PrependWith("new"))
                    .FirstOrDefault();
            }

            Assert.IsNotNull(newUser);
            Assert.AreEqual(FakeValues.FirstName, newUser.FirstName);
            Assert.AreEqual(FakeValues.LastName, newUser.LastName);
            Assert.AreEqual(FakeValues.Email.PrependWith("new"), newUser.Email);
            Assert.IsTrue(newUser.ValidatePassword(FakeValues.Password));
        }

        #endregion 
    }
}