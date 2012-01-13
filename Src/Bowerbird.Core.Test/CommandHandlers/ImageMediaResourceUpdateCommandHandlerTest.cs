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

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Core.DomainModels.MediaResources;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    public class ImageMediaResourceUpdateCommandHandlerTest
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

        private ImageMediaResourceUpdateCommandHandler TestImageMediaResourceUpdateCommandHandler(IDocumentSession session)
        {
            return new ImageMediaResourceUpdateCommandHandler(
                new Repository<User>(session),
                new Repository<ImageMediaResource>(session)
                );
        }

        private ImageMediaResourceUpdateCommand TestImageMediaResourceUpdateCommand(string id)
        {
            return new ImageMediaResourceUpdateCommand()
                       {
                           Id = id,
                           UserId = FakeValues.UserId.PrependWith("users/"),
                           Description = FakeValues.Description.PrependWith("new")
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceUpdateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ImageMediaResourceUpdateCommandHandler(null, new Mock<Repository<ImageMediaResource>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceUpdateCommandHandler_Constructor_Passing_Null_ImageMediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ImageMediaResourceUpdateCommandHandler(new Mock<Repository<User>>().Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceUpdateCommandHandler_Handle_Passing_Null_ImageMediaResourceUpdateCommand_Throws_DesignByContractException()
        {
            var commandHandler = new ImageMediaResourceUpdateCommandHandler(
                new Mock<IRepository<User>>().Object, 
                new Mock<IRepository<ImageMediaResource>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ImageMediaResourceUpdateCommandHandler_Handle_Updates_ImageMediaResourceUpdate()
        {
            using (var session = _store.OpenSession())
            {
                var imageMediaResourceUpdateCommandHandler = TestImageMediaResourceUpdateCommandHandler(session);

                session.Store(new ImageMediaResource(
                    FakeObjects.TestUserWithId(),
                    FakeValues.CreatedDateTime,
                    FakeValues.Filename,
                    FakeValues.FileFormat,
                    FakeValues.Description,
                    FakeValues.Number,
                    FakeValues.Number));

                session.SaveChanges();

                var imageResourceId =
                    session
                    .Query<ImageMediaResource>()
                    .Where(x => x.CreatedByUser.Id == FakeValues.UserId.PrependWith("users/"))
                    .FirstOrDefault()
                    .Id;

                var imageMediaResourceUpdateCommand = TestImageMediaResourceUpdateCommand(imageResourceId);

                imageMediaResourceUpdateCommandHandler.Handle(imageMediaResourceUpdateCommand);

                session.SaveChanges();

                var imageMediaResource =
                    session
                        .Query<ImageMediaResource>()
                        .Where(x => x.Id == imageResourceId)
                        .FirstOrDefault();

                Assert.AreEqual(imageMediaResourceUpdateCommand.Description, imageMediaResource.Description);
            }
        }

        #endregion
    }
}