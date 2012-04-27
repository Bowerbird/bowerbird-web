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
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Indexes;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;

namespace Bowerbird.Test.Indexes
{
    [TestFixture]
    public class UserIndexTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            IndexCreation.CreateIndexes(typeof(User_WithUserIdAndEmail).Assembly, _documentStore);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void UserIndex_Saves_And_Retrieves_User_By_Id()
        {
            var user = FakeObjects.TestUserWithId();

            UserProfile indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<User>("User/WithUserIdAndEmail")
                    .WhereContains("UserId", user.Id)
                    .WaitForNonStaleResults()
                    .Select(x => new UserProfile()
                        {
                            Id = x.Id,
                            LastLoggedIn = x.LastLoggedIn,
                            Name = x.FirstName + " " + x.LastName
                        })
                    .FirstOrDefault();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(user.FirstName.AppendWith(" ").AppendWith(user.LastName), indexResult.Name);
            Assert.AreEqual(user.LastLoggedIn, indexResult.LastLoggedIn);
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void UserIndex_Saves_And_Retrieves_User_By_Email()
        {
            var user = FakeObjects.TestUserWithId();

            UserProfile indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<User>("User/WithUserIdAndEmail")
                    .WhereContains("Email", user.Email)
                    .WaitForNonStaleResults()
                    .Select(x => new UserProfile()
                    {
                        Id = x.Id,
                        LastLoggedIn = x.LastLoggedIn,
                        Name = x.FirstName + " " + x.LastName
                    })
                    .FirstOrDefault();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(user.FirstName.AppendWith(" ").AppendWith(user.LastName), indexResult.Name);
            Assert.AreEqual(user.LastLoggedIn, indexResult.LastLoggedIn);
        }

        #endregion 
    }
}