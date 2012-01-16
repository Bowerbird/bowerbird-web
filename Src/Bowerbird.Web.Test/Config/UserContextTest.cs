/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.Tasks;

namespace Bowerbird.Web.Test.Config
{
    #region Namespaces

    using System.Security.Principal;
    using System.Web;

    using Moq;
    using NUnit.Framework;

    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Config;
    
    #endregion

    [TestFixture]
    public class UserContextTest
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

        private class AuthenticatedUserContext : UserContext
        {
            private HttpContextBase HttpContext { get; set; }

            public AuthenticatedUserContext(IUserTasks userTasks)
                : base(userTasks)
            {
                HttpContext = MockAuthenticatedHttpContext().Object;
            }
        }

        private class AnonymousUserContext : UserContext
        {
            private HttpContextBase HttpContext { get; set; }

            public AnonymousUserContext(IUserTasks userTasks)
                : base(userTasks)
            {
                HttpContext = MockAnonymousHttpContext().Object;
            }
        }

        private static Mock<HttpContextBase> MockAnonymousHttpContext()
        {
            var mockAnonymousIdentity = new Mock<IIdentity>();

            mockAnonymousIdentity.Setup(id => id.IsAuthenticated).Returns(false);
            mockAnonymousIdentity.Setup(id => id.Name).Returns(string.Empty);

            var mockPrincipal = new Mock<IPrincipal>();

            mockPrincipal.Setup(ctx => ctx.Identity).Returns(mockAnonymousIdentity.Object);

            var context = new Mock<HttpContextBase>();

            context.Setup(ctx => ctx.Request).Returns(new Mock<HttpRequestBase>().Object);
            context.Setup(ctx => ctx.Response).Returns(new Mock<HttpResponseBase>().Object);
            context.Setup(ctx => ctx.Session).Returns(new Mock<HttpSessionStateBase>().Object);
            context.Setup(ctx => ctx.Server).Returns(new Mock<HttpServerUtilityBase>().Object);

            context.Setup(ctx => ctx.User).Returns(mockPrincipal.Object);

            return context;
        }

        private static Mock<HttpContextBase> MockAuthenticatedHttpContext()
        {
            var mockIdentity = new Mock<IIdentity>();

            mockIdentity.Setup(id => id.IsAuthenticated).Returns(true);
            mockIdentity.Setup(id => id.Name).Returns(FakeValues.UserId);

            var mockPrincipal = new Mock<IPrincipal>();

            mockPrincipal.Setup(ctx => ctx.Identity).Returns(mockIdentity.Object);

            var context = new Mock<HttpContextBase>();

            context.Setup(ctx => ctx.Request).Returns(new Mock<HttpRequestBase>().Object);
            context.Setup(ctx => ctx.Response).Returns(new Mock<HttpResponseBase>().Object);
            context.Setup(ctx => ctx.Session).Returns(new Mock<HttpSessionStateBase>().Object);
            context.Setup(ctx => ctx.Server).Returns(new Mock<HttpServerUtilityBase>().Object);

            context.Setup(ctx => ctx.User).Returns(mockPrincipal.Object);

            return context;
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void UserContext_IsUserAuthenticated_Having_Authenticated_Context_Returns_True()
        {
            Assert.IsTrue(new AuthenticatedUserContext(new Mock<IUserTasks>().Object).IsUserAuthenticated());
        }

        [Test, Ignore]
        [Category(TestCategory.Unit)]
        public void UserContext_IsUserAuthenticated_Having_UnAuthenticated_Context_Returns_False()
        {
            Assert.IsTrue(new AnonymousUserContext(new Mock<IUserTasks>().Object).IsUserAuthenticated());
        }

        #endregion
    }
}							