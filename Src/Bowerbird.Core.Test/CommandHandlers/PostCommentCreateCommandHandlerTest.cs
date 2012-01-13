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

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class PostCommentCreateCommandHandlerTest
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

        private PostCommentCreateCommand TestPostCommentCreateCommand()
        {
            return new PostCommentCreateCommand()
                       {

                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Constructor_Passing_Null_Something_Throws_DesignByContractException()
        {
           // Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new PostCommentCreateCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void PostCommentCreateCommandHandler_Handle_Passing_Null_PostCommentCreate_Throws_DesignByContractException()
        {
            //Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _commandHandler.Handle(null)));
        }

        #endregion
    }
}