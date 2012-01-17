/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System;
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels.Comments;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationCommentDeleteCommandHandlerTest
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

        private ObservationCommentDeleteCommandHandler TestObservationCommentDeleteCommandHandler(IDocumentSession session)
        {
            return new ObservationCommentDeleteCommandHandler(
                new Repository<ObservationComment>(session)
                );
        }

        private ObservationCommentDeleteCommand TestObservationCommentDeleteCommand(ObservationComment observationComment)
        {
            return new ObservationCommentDeleteCommand()
                       {
                          Id = observationComment.Id,
                          UserId = observationComment.User.Id
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Constructor_Passing_Null_ObservationCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCommentDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Handle_Passing_Null_ObservationCommentDeleteCommand_Throws_DesignByContractException()
        {
            var commandHandler = new ObservationCommentDeleteCommandHandler(new Mock<IRepository<ObservationComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Handle_Deletes_ObservationComment()
        {
            var testUser = FakeObjects.TestUserWithId();
            var testObservation = FakeObjects.TestObservationWithId();
            var testObservationComment = new ObservationComment(
                testUser,
                testObservation,
                FakeValues.CreatedDateTime,
                FakeValues.Comment
                );

            using (var session = _store.OpenSession())
            {
                session.Store(testUser);
                session.Store(testObservation);
                session.Store(testObservationComment);
                
                session.SaveChanges();

                var observationComment = _store.OpenSession()
                    .Query<ObservationComment>()
                    .Where(x => x.User.Id == testUser.Id)
                    .FirstOrDefault();

                Assert.IsNotNull(observationComment);
                Assert.AreEqual(testObservationComment.Message, observationComment.Message);
                Assert.AreEqual(testObservationComment.Observation.Id, observationComment.Observation.Id);
                Assert.AreEqual(testObservationComment.Observation.Title, observationComment.Observation.Title);
                Assert.AreEqual(testObservationComment.User.Id, observationComment.User.Id);
                Assert.AreEqual(testObservationComment.User.FirstName, observationComment.User.FirstName);
                Assert.AreEqual(testObservationComment.User.LastName, observationComment.User.LastName);
                Assert.AreEqual(testObservationComment.User.Email, observationComment.User.Email);
                Assert.AreEqual(testObservationComment.CommentedOn, observationComment.CommentedOn);

                var observationCommentDeleteCommandHandler = TestObservationCommentDeleteCommandHandler(session);

                observationCommentDeleteCommandHandler.Handle(TestObservationCommentDeleteCommand(observationComment));

                session.SaveChanges();

                observationComment = _store.OpenSession()
                    .Query<ObservationComment>()
                    .Where(x => x.User.Id == testUser.Id)
                    .FirstOrDefault();

                Assert.IsNull(observationComment);
            }
        }

        #endregion
    }
}