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
using Bowerbird.Core.DomainModels.MediaResources;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;

namespace Bowerbird.Test.CommandHandlers
{
    public class ImageMediaResourceDeleteCommandHandlerTest
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
        public void ImageMediaResourceDeleteCommandHandler_Handle()
        {
            var imageMediaResource = FakeObjects.TestImageMediaResourceWithId();
            var user = FakeObjects.TestUserWithId();

            ImageMediaResource deletedTeam = null;

            var command = new ImageMediaResourceDeleteCommand()
            {
                Id = imageMediaResource.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(imageMediaResource);
                session.Store(user);

                var commandHandler = new ImageMediaResourceDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deletedTeam = session.Load<ImageMediaResource>(imageMediaResource.Id);
            }

            Assert.IsNull(deletedTeam);
        }

        #endregion
    }
}