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
    #region Namespace

    using System.Linq;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Core.DomainModels.MediaResources;
    using Bowerbird.Core.Extensions;

    #endregion

    public class ImageMediaResourceCreateCommandHandlerTest
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
        public void ImageMediaResourceCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ImageMediaResourceCreateCommandHandler(
                        null,
                        new Mock<IRepository<ImageMediaResource>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceCreateCommandHandler_Constructor_Passing_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ImageMediaResourceCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceCreateCommandHandler_Handle_Passing_Null_ImageMediaResourceCreateCommand_Throws_DesignByContractException()
        {
            var commandHandler = new ImageMediaResourceCreateCommandHandler(
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<ImageMediaResource>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ImageMediaResourceCreateCommandHandler_Handle_Creates_ImageMediaResource()
        {
            ImageMediaResource result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<ImageMediaResource>(session);
                var proxyRepository = new ProxyRepository<ImageMediaResource>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                var imageMediaResourceCreateCommandHandler = new ImageMediaResourceCreateCommandHandler(
                    mockUserRepository.Object,
                    proxyRepository
                    );

                imageMediaResourceCreateCommandHandler.Handle(new ImageMediaResourceCreateCommand()
                {
                    Description = FakeValues.Description,
                    FileFormat = FakeValues.FileFormat,
                    OriginalFileName = FakeValues.Filename,
                    OriginalHeight = FakeValues.Number,
                    OriginalWidth = FakeValues.Number,
                    UploadedOn = FakeValues.CreatedDateTime,
                    UserId = FakeValues.UserId
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion 
    }
}