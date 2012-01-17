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
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.Comments;
    using Bowerbird.Core.Extensions;

    #endregion

    [TestFixture]
    public class ObservationCommentUpdateCommandHandlerTest
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

        private ObservationCommentUpdateCommandHandler TestObservationCommentUpdateCommandHandler(IDocumentSession session)
        {
            return new ObservationCommentUpdateCommandHandler(
                new Repository<User>(session),
                new Repository<ObservationComment>(session)
                );
        }

        private ObservationCommentUpdateCommand TestObservationCommentUpdateCommand(string id)
        {
            return new ObservationCommentUpdateCommand()
            {
                Comment = FakeValues.Comment.PrependWith("new"),
                UpdatedOn = FakeValues.ModifiedDateTime,
                Id = id,
                UserId = FakeValues.UserId.PrependWith("users/")
            };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentUpdateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new ObservationCommentUpdateCommandHandler(null, new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentUpdateCommandHandler_Constructor_Passing_Null_ObservationCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentUpdateCommandHandler(new Mock<IRepository<User>>().Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentUpdateCommandHandler_Handle_Passing_Null_ObservationCommentUpdate_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentUpdateCommandHandler(new Mock<IRepository<User>>().Object, new Mock<IRepository<ObservationComment>>().Object)
                        .Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentUpdateCommandHandler_Handle_Updates_ObservationComment()
        {
            var testUser = FakeObjects.TestUserWithId();
            var testObservation = FakeObjects.TestObservationWithId();

            using (var session = _store.OpenSession())
            {
                session.Store(testUser);
                session.Store(testObservation);
                session.Store(new ObservationComment(
                    testUser,
                    testObservation,
                    FakeValues.CreatedDateTime,
                    FakeValues.Comment
                    ));

                session.SaveChanges();

                var observationCommentId =
                    session
                    .Query<ObservationComment>()
                    .Where(x => x.User.Id == FakeValues.UserId.PrependWith("users/"))
                    .FirstOrDefault()
                    .Id;

                var observationCommentUpdateCommandHandler = TestObservationCommentUpdateCommandHandler(session);
                var command = TestObservationCommentUpdateCommand(observationCommentId);
                observationCommentUpdateCommandHandler.Handle(command);

                session.SaveChanges();

                var observationComment =
                     session
                     .Query<ObservationComment>()
                     .Where(x => x.Id == observationCommentId)
                     .FirstOrDefault();

                Assert.AreEqual(observationComment.EditedOn, command.UpdatedOn);
                Assert.AreEqual(observationComment.Message, command.Comment);
            }
        }

        #endregion
    }
}