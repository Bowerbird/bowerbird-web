/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Indexes;
using Bowerbird.Web.ViewModels.Members;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;

namespace Bowerbird.Test.Indexes
{
    [TestFixture]
    public class ProjectMemberIndexTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            IndexCreation.CreateIndexes(typeof(ProjectMember_WithProjectIdAndUserId).Assembly, _documentStore);
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
        public void ProjectMemberIndex_Saves_And_Retrieves_ProjectMembers_By_UserId()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var projectMember = FakeObjects.TestProjectMemberWithId(); 

            List<MenuItem> indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(projectMember);
                session.SaveChanges();
            }

            Thread.Sleep(1000); // give time for raven to index the user

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<ProjectMember>("ProjectMember/WithProjectIdAndUserId")
                    .WhereContains("UserId", user.Id)
                    .WaitForNonStaleResults()
                    .Select(x => new MenuItem()
                    {
                        Id = x.Project.Id,
                        Name = x.Project.Name
                    })
                    .ToList();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(1, indexResult.Count);
            Assert.AreEqual(projectMember.Project.Name, indexResult[0].Name);
            Assert.AreEqual(projectMember.Project.Id, indexResult[0].Id);
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ProjectMemberIndex_Saves_And_Retrieves_ProjectMembers_By_ProjectId()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var projectMember = FakeObjects.TestProjectMemberWithId(); 

            List<UserProfile> indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(projectMember);
                session.SaveChanges();
            }

            Thread.Sleep(1000); // give time for raven to index the user

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<ProjectMember>("ProjectMember/WithProjectIdAndUserId")
                    .WhereContains("ProjectId", projectMember.Project.Id)
                    .WaitForNonStaleResults()
                    .Select(x => new UserProfile()
                    {
                        Id = x.User.Id,
                        Name = x.User.FirstName.AppendWith(" ").AppendWith(x.User.LastName)
                    })
                    .ToList();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(1, indexResult.Count);
            Assert.AreEqual(projectMember.User.Id, indexResult[0].Id);
            Assert.AreEqual(projectMember.User.FirstName.AppendWith(" ").AppendWith(projectMember.User.LastName), indexResult[0].Name);
        }

        #endregion
    }
}