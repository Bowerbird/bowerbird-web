#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Moq;
//using Rhino.Mocks;

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Web.Config;
using Bowerbird.Web.EventHandlers;
using Bowerbird.Test.Utils;

#endregion

namespace Bowerbird.Web.Test.EventHandlers
{
    [TestFixture] public class NotifyActivityEventHandlerBaseTest
    {

        #region Test Infrastructure

        private Mock<IUserContext> _mockUserContext;
        private ProxyNotifyActivityEventHandler _proxyNotifyActivityEventHandler;

        [SetUp] public void TestInitialize() { 
            _mockUserContext = new Mock<IUserContext>();
            _proxyNotifyActivityEventHandler = new ProxyNotifyActivityEventHandler(_mockUserContext.Object);
        }

        [TearDown] public void TestCleanup() { }

        #endregion

        #region Test Helpers

        internal sealed class ProxyNotifyActivityEventHandler : NotifyActivityEventHandlerBase 
        { 
            public ProxyNotifyActivityEventHandler(IUserContext userContext):base(userContext){}

            public void ProxyNotify(string type, User user, object data)
            {
                base.Notify(type, user, data);
            }
        }

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

        [Test] public void NotifyActivityEventHandler_Constructor_Passing_Null_UserContext_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => new ProxyNotifyActivityEventHandler(null)));
        }

        #endregion

        #region Property Tests

        #endregion

        #region Method Tests

        [Test] public void NotifyActivityEventHandler_Notify_Passing_Empty_Type_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => _proxyNotifyActivityEventHandler
                            .ProxyNotify(
                                string.Empty,
                                TestUser(),
                                new object()
                                )
                            ));
        }

        [Test] public void NotifyActivityEventHandler_Notify_Passing_Null_User_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => _proxyNotifyActivityEventHandler
                            .ProxyNotify(
                                FakeValues.ActivityType,
                                null,
                                new object()
                                )
                            ));
        }

        [Test] public void NotifyActivityEventHandler_Notify_Passing_Null_Data_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => _proxyNotifyActivityEventHandler
                            .ProxyNotify(
                                FakeValues.ActivityType,
                                TestUser(),
                                null
                                )
                            ));
        }

        [Test] public void NotifiyActivity_Calls_UserContext_GetChannel()
        { 
            var clients = new
            {
                activityOccurred = "string"
            };

            _mockUserContext.Setup(x => x.GetChannel()).Returns(clients).Verifiable();

            _proxyNotifyActivityEventHandler.ProxyNotify(FakeValues.ActivityType, TestUser(), new object());

            _mockUserContext.Verify();
        }

        #endregion
        
    }
}