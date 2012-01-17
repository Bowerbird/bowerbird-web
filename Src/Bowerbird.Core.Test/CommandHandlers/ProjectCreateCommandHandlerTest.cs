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
    #region Namespace

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Test.ProxyRepositories;

    #endregion

    public class ProjectCreateCommandHandlerTest
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
        public void ProjectCreateCommandHandler_Constructor_Passing_Null_ProjectRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectCreateCommandHandler(null, new Mock<IRepository<User>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectCreateCommandHandler(new Mock<IRepository<Project>>().Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle_Passing_Null_ProjectCreateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ProjectCreateCommandHandler(
                        new Mock<IRepository<Project>>().Object, 
                        new Mock<IRepository<User>>().Object)
                            .Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectCreateCommandHandler_Handle_Creates_Project()
        {
            Project result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<Project>(session);
                var proxyRepository = new ProxyRepository<Project>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                
                var observationCommentCreateCommandHandler = new ProjectCreateCommandHandler(
                    proxyRepository,
                    mockUserRepository.Object
                    );

                observationCommentCreateCommandHandler.Handle(new ProjectCreateCommand()
                {
                    UserId = FakeValues.UserId,
                    Description = FakeValues.Description,
                    Name = FakeValues.Name
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}