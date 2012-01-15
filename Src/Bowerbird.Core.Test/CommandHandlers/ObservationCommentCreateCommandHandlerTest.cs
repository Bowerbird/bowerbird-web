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

    using System.Linq;

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
    public class ObservationCommentCreateCommandHandlerTest
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

        private ObservationCommentCreateCommandHandler TestObservationCommentCreateCommandHandler(IDocumentSession session)
        {
            return new ObservationCommentCreateCommandHandler(
                new Repository<User>(session),
                new Repository<Observation>(session),
                new Repository<ObservationComment>(session)
                );
        }

        private ObservationCommentCreateCommand TestObservationCommentCreateCommand()
        {
            return new ObservationCommentCreateCommand()
                       {
                           Comment = FakeValues.Comment,
                           CommentedOn = FakeValues.CreatedDateTime,
                           ObservationId = FakeValues.KeyString.PrependWith("observationcomments/"),
                           UserId = FakeValues.UserId.PrependWith("users/")
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new ObservationCommentCreateCommandHandler(
                        null, 
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Observation>>().Object, 
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Handle_Passing_Null_ObservationCommentCreate_Throws_DesignByContractException()
        {
            var commandHandler = new ObservationCommentCreateCommandHandler(
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<Observation>>().Object,
                new Mock<IRepository<ObservationComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentCreateCommandHandler_Handle_Creates_ObservationComment()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(FakeObjects.TestUserWithId());
                session.Store(FakeObjects.TestObservationWithId());
                
                session.SaveChanges();
                
                var observationCommentCreateCommandHandler = TestObservationCommentCreateCommandHandler(session);
                var command = TestObservationCommentCreateCommand();
                observationCommentCreateCommandHandler.Handle(command);
                session.SaveChanges();

                var observationComment =
                     _store.OpenSession().Query<ObservationComment>().Where(
                        x => x.Observation.Id == FakeValues.KeyString.PrependWith("observationcomments/")).FirstOrDefault();

                //session.Store(new ObservationComment(
                //    FakeObjects.TestUserWithId(),
                //    FakeObjects.TestObservationWithId(),
                //    FakeValues.CreatedDateTime,
                //    FakeValues.Comment
                //    ));

                Assert.AreEqual(observationComment.CommentedOn, command.CommentedOn);
                Assert.AreEqual(observationComment.Message, command.Comment);
                Assert.AreEqual(observationComment.Observation.Id, command.ObservationId);
            }
        }

        #endregion
    }
}