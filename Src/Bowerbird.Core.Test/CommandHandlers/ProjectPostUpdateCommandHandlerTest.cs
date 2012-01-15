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

    using Core.Commands;
    using Core.CommandHandlers;
    using Core.DesignByContract;
    using Core.DomainModels;
    using Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class ProjectPostUpdateCommandHandlerTest
    {
        #region Test Infrastructure

        //private Mock<IDefaultRepository<User>> _mockUserRepository;
        //private Mock<User> _mockUser;
        //private ProjectPostUpdateCommandHandler _commandHandler;

        [SetUp]
        public void TestInitialize()
        {
            //_mockUserRepository = new Mock<IDefaultRepository<User>>();
            //_mockUser = new Mock<User>();
            //_commandHandler = new ProjectPostUpdateCommandHandler();
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        private ProjectPostUpdateCommand TestProjectPostUpdateCommand()
        {
            return new ProjectPostUpdateCommand()
                       {
                           Id = FakeValues.KeyString,
                           MediaResources = new List<string>(),
                           Message = FakeValues.Message,
                           Subject = FakeValues.Subject,
                           Timestamp = FakeValues.CreatedDateTime,
                           UserId = FakeValues.UserId
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommandHandler_Constructor_Passing_Null_Something_Throws_DesignByContractException()
        {
            //Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProjectPostUpdateCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectPostUpdateCommandHandler_Handle_Passing_Null_ProjectPostUpdateCommand_Throws_DesignByContractException()
        {
            //Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => _commandHandler.Handle(null)));
        }

        #endregion
    }
}