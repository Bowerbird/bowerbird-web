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
    using Bowerbird.Core.DomainModels.Comments;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ObservationCommentDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize()
        {
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        private ObservationCommentDeleteCommandHandler TestObservationCommentDeleteCommandHandler(IDocumentSession session)
        {
            return new ObservationCommentDeleteCommandHandler(
                new Repository<ObservationComment>(session)
                );
        }

        private ObservationCommentDeleteCommand TestObservationCommentDeleteCommand()
        {
            return new ObservationCommentDeleteCommand()
                       {

                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Constructor_Passing_Null_Something_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ObservationCommentDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Handle_Passing_Null_ObservationCommentDeleteCommand_Throws_DesignByContractException()
        {
            var commandHandler = new ObservationCommentDeleteCommandHandler(new Mock<IRepository<ObservationComment>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() => 
                    commandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void ObservationCommentDeleteCommandHandler_Handle_Deletes_ObservationComment()
        {

        }

        #endregion
    }
}