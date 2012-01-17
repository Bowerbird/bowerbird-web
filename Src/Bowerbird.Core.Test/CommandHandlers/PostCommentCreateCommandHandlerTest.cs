/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Test.ProxyRepositories;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

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

    #endregion

    [TestFixture]
    public class PostCommentCreateCommandHandlerTest
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

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new PostCommentCreateCommandHandler(
                        null,
                        new Mock<IRepository<Post>>().Object,
                        new Mock<IRepository<PostComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Constructor_Passing_Null_PostRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new PostCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<PostComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Constructor_Passing_Null_PostCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new PostCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Post>>().Object,
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Handle_Passing_Null_PostCommentCreate_Throws_DesignByContractException()
        {
            var commandHandler = new PostCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Post>>().Object,
                        new Mock<IRepository<PostComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ObservationCommentCreateCommandHandler_Handle_Creates_ObservationComment()
        {
            PostComment result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<PostComment>(session);
                var proxyRepository = new ProxyRepository<PostComment>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();
                var mockPostRepository = new Mock<IRepository<Post>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                //mockPostRepository
                //    .Setup(x => x.Load(It.IsAny<string>()))
                //    .Returns(FakeObjects.TestPost);

                var postCommentCreateCommandHandler = new PostCommentCreateCommandHandler(
                    mockUserRepository.Object,
                    mockPostRepository.Object,
                    proxyRepository
                    );

                postCommentCreateCommandHandler.Handle(new PostCommentCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    PostId = FakeValues.KeyString,
                    Message = FakeValues.Message,
                    PostedOn = FakeValues.CreatedDateTime
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}