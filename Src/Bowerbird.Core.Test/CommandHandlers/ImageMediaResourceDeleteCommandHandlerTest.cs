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

    public class ImageMediaResourceDeleteCommandHandlerTest
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

        private ImageMediaResourceDeleteCommandHandler TestImageMediaResourceDeleteCommandHandler(IDocumentSession session)
        {
            return new ImageMediaResourceDeleteCommandHandler(
                new Repository<ImageMediaResource>(session)
                );
        }

        private ImageMediaResourceDeleteCommand TestImageMediaResourceDeleteCommand()
        {
            return new ImageMediaResourceDeleteCommand()
                       {
                           UserId = FakeValues.UserId.PrependWith("users/")
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceDeleteCommandHandler_Constructor_Passing_Null_SOMETHING_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ImageMediaResourceDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ImageMediaResourceDeleteCommandHandler_Handle_Passing_Null_ImageMediaResourceDeleteCommand_Throws_DesignByContractException()
        {
            var commandHandler = new ImageMediaResourceDeleteCommandHandler(new Mock<IRepository<ImageMediaResource>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Persistance)]
        public void ImageMediaResourceDeleteCommandHandler_Handle_Creates_Deletes_Updates_ImageMediaResourceDelete()
        {
            using (var session = _store.OpenSession())
            {
                var imageMediaResourceDeleteCommandHandler = TestImageMediaResourceDeleteCommandHandler(session);

                session.Store(new ImageMediaResource(
                    FakeObjects.TestUserWithId(),
                    FakeValues.CreatedDateTime,
                    FakeValues.Filename,
                    FakeValues.FileFormat,
                    FakeValues.Description,
                    FakeValues.Number,
                    FakeValues.Number));

                session.SaveChanges();

                var imageResource =
                    session.Query<ImageMediaResource>().Where(
                        x => x.CreatedByUser.Id == FakeValues.UserId.PrependWith("users/")).FirstOrDefault();

                Assert.IsNotNull(imageResource);

                imageMediaResourceDeleteCommandHandler.Handle(new ImageMediaResourceDeleteCommand()
                                                                  {
                                                                      Id = imageResource.Id, 
                                                                      UserId = imageResource.CreatedByUser.Id
                                                                  });

                session.SaveChanges();

                imageResource =
                    _store.OpenSession().Query<ImageMediaResource>().Where(
                        x => x.CreatedByUser.Id == FakeValues.UserId.PrependWith("users/")).FirstOrDefault();

                Assert.IsNull(imageResource);
            }
        }

        #endregion
    }
}