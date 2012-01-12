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
using Raven.Client;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.Commands;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    
    #endregion

    [TestFixture]
    public class ObservationCreateCommandHandlerTest
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
        public void ObservationCreateCommandHandler_Constructor_With_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>
                new ObservationCreateCommandHandler(null,
                                                    new Mock<IRepository<User>>().Object,
                                                    new Mock<IRepository<MediaResource>>().Object)
                ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Constructor_With_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>
                new ObservationCreateCommandHandler(new Mock<IRepository<Observation>>().Object,
                                                    null,
                                                    new Mock<IRepository<MediaResource>>().Object)
                ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Constructor_With_Null_MediaResourceRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() =>
                new ObservationCreateCommandHandler(new Mock<IRepository<Observation>>().Object,
                                                    new Mock<IRepository<User>>().Object,
                                                    null)
                ));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Handle_Passing_Null_ObservationCreateCommandHandle_Throws_DesignByContractException()
        {
            var handler = new ObservationCreateCommandHandler(new Mock<IRepository<Observation>>().Object,
                                                              new Mock<IRepository<User>>().Object,
                                                              new Mock<IRepository<MediaResource>>().Object);

            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => handler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCreateCommandHandler_Handle_Passing_ObservationCreateCommand_Saves_New_Observation()
        {
            Observation result = null;

            using (var session = _store.OpenSession())
            {
                var repository = new Repository<Observation>(session);
                var proxyRepository = new ProxyRepository<Observation>(repository);
                var mockUserRepository = new Mock<IRepository<User>>();

                proxyRepository.NotifyOnAdd(x => result = x);

                mockUserRepository
                    .Setup(x => x.Load(It.IsAny<string>()))
                    .Returns(FakeObjects.TestUserWithId);

                var observationCreateCommandHandler = new ObservationCreateCommandHandler(proxyRepository, mockUserRepository.Object, new Repository<MediaResource>(session));

                observationCreateCommandHandler.Handle(new ObservationCreateCommand()
                {
                    Address = FakeValues.Address,
                    IsIdentificationRequired = false,
                    Latitude = FakeValues.Latitude,
                    Longitude = FakeValues.Longitude,
                    ObservationCategory = FakeValues.Category,
                    ObservedOn = FakeValues.CreatedDateTime,
                    Title = FakeValues.Title,
                    UserId = FakeValues.UserId
                });

                session.SaveChanges();
            }

            Assert.IsNotNull(result);
        }

        #endregion
    }
}