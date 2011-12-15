using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Entities;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Linq;

namespace Bowerbird.Core.Test.Repositories
{
    [TestFixture]
    public class UserRepositoryTest
    {

        #region Infrastructure

        private IDocumentStore _store;
        
        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanUp()
        {
            _store = null;
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        public void UserRepository_Can_Save_User_Record()
        {
            User userWrite, userRead;

            using (var session = _store.OpenSession())
            {
                userWrite = new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles()
                    );

                session.Store(userWrite);
                session.SaveChanges();
            }

            using (var session = _store.OpenSession())
            {
                userRead = session
                    .Query<User>()
                    .Where(x => x.Id == userWrite.Id)
                    .FirstOrDefault();
            }

            // TODO: Implement equality overloading to pass this assertion.. 
            Assert.AreEqual(userWrite, userRead);
        }

        #endregion

        #region Helpers

        private static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        private static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };

        }

        #endregion
				
    }
}