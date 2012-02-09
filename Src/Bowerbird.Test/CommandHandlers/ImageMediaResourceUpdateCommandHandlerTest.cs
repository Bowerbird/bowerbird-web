/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels.MediaResources;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    public class ImageMediaResourceUpdateCommandHandlerTest
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

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void ImageMediaResourceUpdateCommandHandler_Handle()
        {
            var originalValue = FakeObjects.TestImageMediaResourceWithId();
            var user = FakeObjects.TestUserWithId();

            ImageMediaResource newValue;

            var command = new ImageMediaResourceUpdateCommand()
            {
                Id = originalValue.Id,
                UserId = user.Id,
                Description = FakeValues.Description.PrependWith("new")
            };

            using (var session = _store.OpenSession())
            {
                session.Store(originalValue);

                var commandHandler = new ImageMediaResourceUpdateCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                newValue = session.Load<ImageMediaResource>(originalValue.Id);
            }

            Assert.IsNotNull(newValue);
            Assert.AreEqual(command.Description, newValue.Description);
        }

        #endregion
    }
}