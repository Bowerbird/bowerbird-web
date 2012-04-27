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
using System.Collections.Generic;
using Bowerbird.Core.DomainModels;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Indexes;
using Bowerbird.Web.ViewModels.Shared;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;

namespace Bowerbird.Test.Indexes
{
    [TestFixture]
    public class StreamItemIndexTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.TestDocumentStore();

            IndexCreation.CreateIndexes(typeof(StreamItem_WithParentIdAndUserIdAndCreatedDateTimeAndType).Assembly, _documentStore);
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
        public void StreamItemIndex_Saves_And_Retrieves_StreamItems_By_UserId()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var projectPost = FakeObjects.TestProjectPostWithId();
            var projectPostStreamItem = FakeObjects.TestStreamItem(projectPost, project.Id);
            var observation = FakeObjects.TestObservationWithId();
            var observationStreamItem = FakeObjects.TestStreamItem(observation, project.Id);
            var observationNote = FakeObjects.TestObservationNoteWithId();
            var observationNoteStreamItem = FakeObjects.TestStreamItem(observationNote, project.Id);
            
            List<StreamItemViewModel> indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(observation);
                session.Store(observationStreamItem);
                session.Store(observationNote);
                session.Store(observationNoteStreamItem);
                session.Store(projectPost);
                session.Store(projectPostStreamItem);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<StreamItem>("StreamItem/WithParentIdAndUserIdAndCreatedDateTimeAndType")
                    .WhereContains("UserId", user.Id)
                    .WaitForNonStaleResults()
                    .Select(x => new StreamItemViewModel()
                    {
                        Item = x.Item,
                        ItemId = x.ItemId,
                        ParentId = x.ParentId,
                        SubmittedOn = x.CreatedDateTime,
                        Type = x.Type,
                        UserId = x.User.Id
                    })
                    .ToList();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(3, indexResult.Count);
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void StreamItemIndex_Saves_And_Retrieves_StreamItem_By_ParentId()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var projectPost = FakeObjects.TestProjectPostWithId();
            var projectPostStreamItem = FakeObjects.TestStreamItem(projectPost, project.Id);
            var observation = FakeObjects.TestObservationWithId();
            var observationStreamItem = FakeObjects.TestStreamItem(observation, project.Id);
            var observationNote = FakeObjects.TestObservationNoteWithId();
            var observationNoteStreamItem = FakeObjects.TestStreamItem(observationNote, project.Id);

            List<StreamItemViewModel> indexResult;

            using (var session = _documentStore.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(observation);
                session.Store(observationStreamItem);
                session.Store(observationNote);
                session.Store(observationNoteStreamItem);
                session.Store(projectPost);
                session.Store(projectPostStreamItem);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                indexResult = session
                    .Advanced
                    .LuceneQuery<StreamItem>("StreamItem/WithParentIdAndUserIdAndCreatedDateTimeAndType")
                    .WhereContains("ParentId", project.Id)
                    .WaitForNonStaleResults()
                    .Select(x => new StreamItemViewModel()
                    {
                        Item = x.Item,
                        ItemId = x.ItemId,
                        ParentId = x.ParentId,
                        SubmittedOn = x.CreatedDateTime,
                        Type = x.Type,
                        UserId = x.User.Id
                    })
                    .ToList();
            }

            Assert.IsNotNull(indexResult);
            Assert.AreEqual(3, indexResult.Count);
        }

        #endregion
    }
}