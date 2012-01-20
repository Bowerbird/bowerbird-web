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
using Raven.Client.Linq;

namespace Bowerbird.Test.CommandHandlers
{
    public class ProjectObservationDeleteCommandHandlerTest
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
        public void ProjectObservationDeleteCommandHandler_Deletes_ProjectObseravtion()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var observation = FakeObjects.TestObservationWithId();

            ProjectObservation deletedProjectObservation = null;

            var projectObservation = new ProjectObservation(
                user, 
                FakeValues.CreatedDateTime, 
                project, 
                observation);

            var command = new ProjectObservationDeleteCommand()
            {
                ProjectId = project.Id,
                UserId = user.Id,
                ObservationId = observation.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(observation);
                session.Store(projectObservation);

                var commandHandler = new ProjectObservationDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedProjectObservation = session
                    .Query<ProjectObservation>()
                    .Where(x => x.Project.Id == project.Id && x.Observation.Id == observation.Id)
                    .FirstOrDefault();
            }

            Assert.IsNull(deletedProjectObservation);
        }

        #endregion 
    }
}