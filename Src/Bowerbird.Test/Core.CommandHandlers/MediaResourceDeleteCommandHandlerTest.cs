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
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Raven.Client;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Test.Core.CommandHandlers
{
    public class MediaResourceDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.StartRaven();
        }

        [TearDown]
        public void TestCleanup()
        {
            DocumentStoreHelper.KillRaven();
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void MediaResourceDeleteCommandHandler_Handle()
        {
            var mediaResource = FakeObjects.TestMediaResourceWithId();
            var user = FakeObjects.TestUserWithId();

            MediaResource deleted = null;

            var command = new MediaResourceDeleteCommand()
            {
                Id = mediaResource.Id,
                UserId = user.Id
            };

            using (var session = _store.OpenSession())
            {
                session.Store(mediaResource);
                session.Store(user);

                var commandHandler = new MediaResourceDeleteCommandHandler(session);

                commandHandler.Handle(command);

                session.SaveChanges();

                deleted = session.Load<MediaResource>(mediaResource.Id);
            }

            Assert.IsNull(deleted);
        }

        #endregion
    }
}