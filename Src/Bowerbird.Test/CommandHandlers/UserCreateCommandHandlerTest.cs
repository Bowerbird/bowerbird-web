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
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class UserCreateCommandHandlerTest
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

        #region Method tests

        [Test]
        [Category(TestCategory.Integration)]
        public void UserCreateCommandHandler_Creates_User()
        {
            User newUser = null;
            using (var session = _store.OpenSession())
            {
                var commandHandler = new UserCreateCommandHandler(session);

                commandHandler.Handle(new UserCreateCommand()
                {
                    Email = FakeValues.Email,
                    Description = FakeValues.Description,
                    FirstName = FakeValues.FirstName,
                    LastName = FakeValues.LastName,
                    Password = FakeValues.Password,
                    Roles = FakeObjects.TestRoles().Select(x => x.Name)
                });

                session.SaveChanges();

                newUser = session.Query<User>().FirstOrDefault();
            }

            Assert.IsNotNull(newUser);
            Assert.AreEqual(FakeValues.FirstName, newUser.FirstName);
            Assert.AreEqual(FakeValues.LastName, newUser.LastName);
            Assert.AreEqual(FakeValues.Email, newUser.Email);
            //Assert.AreEqual(FakeValues.Description, newUser.Description);
            Assert.IsTrue(newUser.ValidatePassword(FakeValues.Password));
        }

        #endregion 
    }
}