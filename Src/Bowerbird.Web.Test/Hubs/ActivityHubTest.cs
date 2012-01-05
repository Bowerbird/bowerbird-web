/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Web.Test.Hubs
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;
    using Moq;
    using Raven.Client;
    using SignalR.Hubs;

    using Bowerbird.Core;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Entities;
    using Bowerbird.Web.Hubs;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class ActivityHubTest
    {
        #region Test Infrastructure

        private Mock<IDocumentSession> _mockDocumentSession;

        [SetUp] 
        public void TestInitialize() 
        {
            _mockDocumentSession = new Mock<IDocumentSession>();
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
                FakeValues.KeyString,
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

        #endregion

        #region Constructor Tests

        [Test, Category(TestCategory.Unit)] 
        public void ActivityHub_Constructor_With_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ActivityHub(null)));
        }

        #endregion

        #region Property Tests

        #endregion

        #region Method Tests

        [Test, Category(TestCategory.Unit)] 
        public void ActivityHub_StartActivityStream_Throws_NotImplementedException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<NotImplementedException>(() => new ActivityHub(_mockDocumentSession.Object).StartActivityStream()));
        }

        #endregion
    }
}