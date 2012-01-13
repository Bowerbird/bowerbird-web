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
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;

    using Core.Commands;
    using Core.CommandHandlers;
    using Core.DesignByContract;
    using Core.DomainModels;
    using Core.Repositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Test.ProxyRepositories;

    #endregion

    [TestFixture]
    public class MediaResourceCreateCommandHandlerTest
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

        private MediaResourceCreateCommandHandler TestMediaResourceCreateCommandHandler(IDocumentSession session)
        {
            return new MediaResourceCreateCommandHandler(
                new Repository<User>(session),
                new Repository<MediaResource>(session)
                );
        }

        //private MediaResourceCreateCommand TestMediaResourceCreateCommand()
        //{
        //    return NotImplementedException();
        //}

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResourceCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
           Assert.IsTrue(
               BowerbirdThrows.Exception<DesignByContractException>(() => 
                   new MediaResourceCreateCommandHandler(
                       null,
                       new Mock<IRepository<MediaResource>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResourceCreateCommandHandler_Constructor_Passing_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new MediaResourceCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResourceCreateCommandHandler_Handle_Passing_Null_MediaResourceCreateCommand_Throws_DesignByContractException()
        {
            var mediaResourceCreateCommandHandler = new MediaResourceCreateCommandHandler(
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<MediaResource>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    mediaResourceCreateCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResourceCreateCommandHandler_Handle_Creates_MediaResource()
        {
            MediaResource read = null;

            using (var session =_store.OpenSession())
            {
                var mediaResourceCreateCommandHandler = TestMediaResourceCreateCommandHandler(session);

                


            }
        }

        #endregion
    }
}