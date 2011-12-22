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

namespace Bowerbird.Core.Test.Repositories
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;
    using Raven.Client;
    using Raven.Client.Linq;

    using Bowerbird.Core.Entities;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class UserRepositoryTest
    {

        #region Test Infrastructure

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

        #region Test Helpers

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

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategories.Persistance)] 
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

    }
}