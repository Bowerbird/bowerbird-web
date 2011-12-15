using System.Security.Principal;
using System.Web;
using Moq;

namespace Bowerbird.Test.Utils
{

    public static class MockHelpers
    {

        /// <summary>
        /// Return a Mock HttpRequestBase
        /// </summary>
        public static Mock<HttpRequestBase> MockHttpRequest()
        {
            return new Mock<HttpRequestBase>();
        }

        /// <summary>
        /// Return a Mock HttpResponseBase
        /// </summary>
        public static Mock<HttpResponseBase> MockHttpResponse()
        {
            return new Mock<HttpResponseBase>();
        }

        /// <summary>
        /// Return a Mock HttpSessionStateBase 
        /// </summary>
        public static Mock<HttpSessionStateBase> MockHttpSessionStateBase()
        {
            return new Mock<HttpSessionStateBase>();
        }

        /// <summary>
        /// Returns a Mock HttpServerUtilityBase
        /// </summary>
        public static Mock<HttpServerUtilityBase> MockHttpServerUtilityBase()
        {
            return new Mock<HttpServerUtilityBase>();
        }

        /// <summary>
        /// Mock context using pre-configured minimum mocks for 
        /// request, response, session, server
        /// </summary>
        public static Mock<HttpContextBase> MockAnonymousHttpContext()
        {
            var context = new Mock<HttpContextBase>();

            context.Setup(ctx => ctx.Request).Returns(MockHttpRequest().Object);
            context.Setup(ctx => ctx.Response).Returns(MockHttpResponse().Object);
            context.Setup(ctx => ctx.Session).Returns(MockHttpSessionStateBase().Object);
            context.Setup(ctx => ctx.Server).Returns(MockHttpServerUtilityBase().Object);

            return context;
        }

        /// <summary>
        /// Return Mock IIdentity
        /// : Returns IsAuthenticated = true
        /// : Returns Name = "username"
        /// </summary>
        public static Mock<IIdentity> MockIdentity()
        {
            var mockIdentity = new Mock<IIdentity>();

            mockIdentity.Setup(id => id.IsAuthenticated).Returns(true);
            mockIdentity.Setup(id => id.Name).Returns(FakeValues.UserName);

            return mockIdentity;
        }

        /// <summary>
        /// Returns Mock IPrincipal
        /// : Returns Identity = MockIdentity
        /// </summary>
        public static Mock<IPrincipal> MockPrincipal()
        {
            var mockPrincipal = new Mock<IPrincipal>();

            mockPrincipal.Setup(ctx => ctx.Identity).Returns(MockIdentity().Object);

            return mockPrincipal;
        }

    }
}