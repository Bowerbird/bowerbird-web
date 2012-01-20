/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels.Posts;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    [TestFixture]
    public class ProjectPostDeleteCommandHandlerTest
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
        [Category(TestCategory.Persistance)]
        public void ProjectPostDeleteCommandHandler_Deletes_ProjectPost()
        {
            var projectPost = FakeObjects.TestProjectPostWithId();
            var user = FakeObjects.TestUserWithId();

            ProjectPost deletedProjectPost = null;

            var command = new ProjectPostDeleteCommand()
            {
                Id = projectPost.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(projectPost);
                session.Store(user);

                var commandHandler = new ProjectPostDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedProjectPost = session.Load<ProjectPost>(projectPost.Id);
            }

            Assert.IsNull(deletedProjectPost);
        }

        #endregion 
    }
}