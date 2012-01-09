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

    using System.Linq;
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
    public class UserCreateCommandHandlerTest
    {
        #region Test Infrastructure

        private Mock<IRepository<User>> _mockUserRepository;
        private Mock<IRepository<Role>> _mockRoleRepository;
        private UserCreateCommandHandler _userCreateCommandHandler;

        [SetUp]
        public void TestInitialize()
        {
            _mockUserRepository = new Mock<IRepository<User>>();
            _mockRoleRepository = new Mock<IRepository<Role>>();
            _userCreateCommandHandler = new UserCreateCommandHandler(_mockUserRepository.Object,_mockRoleRepository.Object);
        }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        /// <summary>
        /// Id: "abc"
        /// Password: "password"
        /// Email: "padil@padil.gov.au"
        /// FirstName: "first name"
        /// LastName: "last name"
        /// Description: "description"
        /// Roles: "Member"
        /// </summary>
        /// <returns></returns>
        private static User TestUser()
        {
            return new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeValues.Description,
                TestRoles()
            )
            .UpdateLastLoggedIn()
            .UpdateResetPasswordKey()
            .IncrementFlaggedItemsOwned()
            .IncrementFlagsRaised();
        }

        private static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        private static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };

        }

        private static UserCreateCommand TestUserCreateCommand()
        {
            return new UserCreateCommand()
                       {
                           Description = FakeValues.Description,
                           Email = FakeValues.Email,
                           FirstName = FakeValues.FirstName,
                           LastName = FakeValues.LastName,
                           Password = FakeValues.Password,
                           Roles = TestRoles().Select(x => x.Id).ToList()
                       };
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommandHandler_Constructor_Passing_Null_UserRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserCreateCommandHandler(null, _mockRoleRepository.Object)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommandHandler_Constructor_Passing_Null_RoleRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserCreateCommandHandler(_mockUserRepository.Object, null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void UserCreateCommandHandler_Handle_Passing_Null_UserCreateCommand_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserCreateCommandHandler(_mockUserRepository.Object, _mockRoleRepository.Object).Handle(null)));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void UserCreateCommandHandler_Handle_Passing_UserCreateCommand_Calls_RoleRepository_Load()
        {
            _mockRoleRepository.Setup(x => x.Load(It.IsAny<IEnumerable<string>>())).Returns(TestRoles());

            _userCreateCommandHandler.Handle(TestUserCreateCommand());

            _mockRoleRepository.Verify(x => x.Load(It.IsAny<IEnumerable<string>>()), Times.Once());
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void UserCreateCommandHandler_Handle_Passing_UserCreateCommand_Calls_UserRepository_Add()
        {
            _mockUserRepository.Setup(x => x.Add(It.IsAny<User>())).Verifiable();

            _userCreateCommandHandler.Handle(TestUserCreateCommand());

            _mockUserRepository.Verify(x => x.Add(It.IsAny<User>()), Times.Once());
        }

        #endregion 
    }
}