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
    public class ProjectObservationCreateCommandHandlerTest
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
        public void ProjectObservationCreateCommandHandler_Creates_ProjectObservation()
        {
            var user = FakeObjects.TestUserWithId();
            var project = FakeObjects.TestProjectWithId();
            var observation = FakeObjects.TestObservationWithId();

            ProjectObservation newValue = null;

            var command = new ProjectObservationCreateCommand()
            {
                UserId = user.Id,
                CreatedDateTime = FakeValues.CreatedDateTime,
                ObservationId = observation.Id,
                ProjectId = project.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);
                session.Store(project);
                session.Store(observation);

                var commandHandler = new ProjectObservationCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<ProjectObservation>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.CreatedByUser);
            //Assert.AreEqual(project.DenormalisedNamedDomainModelReference(), newValue.Project);
            //Assert.AreEqual(observation.DenormalisedObservationReference(), newValue.Observation);
        }

        #endregion 
    }
}