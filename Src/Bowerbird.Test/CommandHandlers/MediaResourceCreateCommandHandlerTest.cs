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
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Services;
using Moq;

namespace Bowerbird.Test.CommandHandlers
{
    public class MediaResourceCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
        private Mock<IMediaFilePathService> _mockMediaFilePathService;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.InMemoryDocumentStore();
            _mockMediaFilePathService = new Mock<IMediaFilePathService>();
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

        #region Property tests

        #endregion

        #region Method tests

        // TODO: Requires functionality built in hander
        [Test, Ignore]
        [Category(TestCategory.Persistance)]
        public void MediaResourceCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();

            MediaResource newValue = null;

            var command = new MediaResourceCreateCommand()
            {
                UserId = user.Id,
                UploadedOn = FakeValues.CreatedDateTime,
                MimeType = FakeValues.FileFormat,
                OriginalFileName = FakeValues.Filename,
                Stream = null,
                Usage = FakeValues.Usage
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);

                var commandHandler = new MediaResourceCreateCommandHandler(session, _mockMediaFilePathService.Object);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<MediaResource>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.UploadedOn, newValue.UploadedOn);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.CreatedByUser);
        }

        #endregion 
    }
}