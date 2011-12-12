using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Extensions;
using NUnit.Framework;
using Raven.Client.Linq;
using Raven.Client.Document;
using Raven.Storage.Esent;
using Raven.Storage.Managed;

namespace Bowerbird.Core.Test.Repositories
{
    [TestFixture]
    public class UserRepositoryTest : RepositoryBaseTest
    {

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        public void UserRepository_Can_Save_User_Record()
        {
            using (var session = _store.OpenSession())
            {
                var userWrite = new User(
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

                var userRead = session.Query<User>().Where(x => x.Id == userWrite.Id).FirstOrDefault();

                Assert.AreEqual(userWrite, userRead);
            }
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