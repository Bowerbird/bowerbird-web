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
using Bowerbird.Core.DomainModels.MediaResources;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    public class ImageMediaResourceCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.InMemoryDocumentStore();            
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

        [Test]
        [Category(TestCategory.Persistance)]
        public void ImageMediaResourceCreateCommandHandler_Handle()
        {
            var user = FakeObjects.TestUserWithId();

            ImageMediaResource newValue = null;

            var command = new ImageMediaResourceCreateCommand()
            {
                UserId = user.Id,
                Description = FakeValues.Description,
                FileFormat = FakeValues.FileFormat,
                OriginalFileName = FakeValues.Filename,
                OriginalHeight = FakeValues.Number,
                OriginalWidth = FakeValues.Number,
                UploadedOn = FakeValues.CreatedDateTime
            };

            using (var session = _store.OpenSession())
            {
                session.Store(user);

                var commandHandler = new ImageMediaResourceCreateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Query<ImageMediaResource>().FirstOrDefault();
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Description, newValue.Description);
            Assert.AreEqual(command.FileFormat, newValue.FileFormat);
            Assert.AreEqual(command.OriginalFileName, newValue.OriginalFileName);
            Assert.AreEqual(command.OriginalHeight, newValue.Height);
            Assert.AreEqual(command.OriginalWidth, newValue.Width);
            Assert.AreEqual(command.UploadedOn, newValue.UploadedOn);
            //Assert.AreEqual(user.DenormalisedUserReference(), newValue.CreatedByUser);
        }

        #endregion 
    }
}