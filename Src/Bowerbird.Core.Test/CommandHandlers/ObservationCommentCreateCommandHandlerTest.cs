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

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels.Comments;

    #endregion

    [TestFixture]
    public class ObservationCommentCreateCommandHandlerTest
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

        private ObservationCommentCreateCommandHandler TestObservationCommentCreateCommandHandler(IDocumentSession session)
        {
            return new ObservationCommentCreateCommandHandler(
                new Repository<User>(session),
                new Repository<Observation>(session),
                new Repository<ObservationComment>(session)
                );
        }

        private ObservationCommentCreateCommand TestObservationCommentCreateCommand()
        {
            return new ObservationCommentCreateCommand()
                       {

                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    new ObservationCommentCreateCommandHandler(
                        null, 
                        new Mock<IRepository<Observation>>().Object,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        null,
                        new Mock<IRepository<ObservationComment>>().Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Constructor_Passing_Null_ObservationCommentRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    new ObservationCommentCreateCommandHandler(
                        new Mock<IRepository<User>>().Object,
                        new Mock<IRepository<Observation>>().Object, 
                        null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentCreateCommandHandler_Handle_Passing_Null_ObservationCommentCreate_Throws_DesignByContractException()
        {
            var commandHandler = new ObservationCommentCreateCommandHandler(
                new Mock<IRepository<User>>().Object,
                new Mock<IRepository<Observation>>().Object,
                new Mock<IRepository<ObservationComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        #endregion
    }
}