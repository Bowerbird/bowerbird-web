/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.Core.Indexes
{
    [TestFixture]
    public class GroupIndexTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.StartRaven();

            using(var documentSession = _documentStore.OpenSession())
            {

            }
        }

        [TearDown]
        public void TestCleanup()
        {

        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        //[Test]
        //[Category(TestCategory.Persistance)]
        //public void ProjectMemberIndex_Saves_And_Retrieves_ProjectMembers_By_UserId()
        //{
        //    var user = new User(FakeValues.Password, FakeValues.Email, FakeValues.FirstName, FakeValues.LastName);
        //    var project = new Project(user, FakeValues.Name, FakeValues.Description, FakeValues.Website, null, FakeValues.CreatedDateTime);
        //    var projectMember = new Member(user, user, project, new List<Role>());

        //    List<ProjectView> indexResult;

        //    using (var session = _documentStore.OpenSession())
        //    {
        //        session.Store(user);
        //        session.Store(project);
        //        session.Store(projectMember);
        //        session.SaveChanges();
        //    }

        //    Thread.Sleep(1000); // give time for raven to index the user

        //    using (var session = _documentStore.OpenSession())
        //    {
        //        indexResult = session
        //            .Query<All_Groups.Result, All_Groups>()
        //            .AsProjection<All_Groups.Result>()
        //            .Customize(x => x.WaitForNonStaleResults());
        //    }

        //    Assert.IsNotNull(indexResult);
        //    Assert.AreEqual(1, indexResult.Count);
        //    Assert.AreEqual(projectMember.Project.Name, indexResult[0].Name);
        //    Assert.AreEqual(projectMember.Project.Id, indexResult[0].Id);
        //}

        #endregion
    }
}