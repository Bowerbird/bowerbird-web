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
    using System.Linq;
    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.Commands;
    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserUpdateLastLoginCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<User> _mockUser;
        private UserUpdateLastLoginCommandHandler _userUpdateLastLoginCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockUserRepository = new Mock<IRepository<User>>();  
            _mockUser = new Mock<User>();
            _userUpdateLastLoginCommandHandler = new UserUpdateLastLoginCommandHandler(_mockUserRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private UserUpdateLastLoginCommand TestUserUpdateLastLoginCommand()
        {
            return new UserUpdateLastLoginCommand()
                       {
                           UserId = FakeValues.KeyString
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategories.Unit)]
        public void UserUpdateLastLoginCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserUpdateLastLoginCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategories.Unit)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_Null_UserUpdateLastLoginCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserUpdateLastLoginCommandHandler(_mockUserRepository.Object).Handle(null)));
        }

        [Test]
        [Category(TestCategories.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_UserUpdateLastLoginCommand_Calls_UserRepository_Load()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);

            _userUpdateLastLoginCommandHandler.Handle(TestUserUpdateLastLoginCommand());

            _mockUserRepository.Verify(x => x.Load(It.IsAny<string>()), Times.Once());
        }

        [Test]
        [Category(TestCategories.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_UserUpdateLastLoginCommand_Calls_UserRepository_Add()
        {
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);
            _mockUserRepository.Setup(x => x.Add(It.IsAny<User>())).Verifiable();

            _userUpdateLastLoginCommandHandler.Handle(TestUserUpdateLastLoginCommand());

            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Once());
        }

        [Test]
        [Category(TestCategories.Integration)]
        public void UserUpdateLastLoginCommandHandler_Handle_Passing_UserUpdateLastLoginCommand_Calls_User_UpdateLastLoggedIn()
        {
            _mockUser.Setup(x => x.UpdateLastLoggedIn()).Verifiable();
            _mockUserRepository.Setup(x => x.Load(It.IsAny<string>())).Returns(_mockUser.Object);

            _userUpdateLastLoginCommandHandler.Handle(TestUserUpdateLastLoginCommand());

            _mockUser.Verify(x => x.UpdateLastLoggedIn(), Times.Once());
        }

        #endregion 
    }
}