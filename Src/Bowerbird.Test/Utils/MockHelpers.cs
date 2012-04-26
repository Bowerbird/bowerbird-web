/* Bowerbird V1  - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Security.Principal;
using System.Web;
using Moq;
using Bowerbird.Core.Config;
using Bowerbird.Core.DomainModels;

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
            mockIdentity.Setup(id => id.Name).Returns(FakeValues.Email);

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

        /// <summary>
        /// Returns IUserContext with all permissions and FakeObjects.User properties
        /// </summary>
        public static Mock<IUserContext> MockUserContext()
        {
            var mockUserContext = new Mock<IUserContext>();

            mockUserContext.Setup(ctx => ctx.IsUserAuthenticated()).Returns(true);
            mockUserContext.Setup(ctx => ctx.GetAuthenticatedUserId()).Returns(FakeObjects.TestUserWithId().Id);
            mockUserContext.Setup(ctx => ctx.HasGroupPermission(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mockUserContext.Setup(ctx => ctx.HasAppRootPermission(It.IsAny<string>())).Returns(true);
            mockUserContext.Setup(ctx => ctx.HasUserProjectPermission(It.IsAny<string>())).Returns(true);
            mockUserContext.Setup(ctx => ctx.HasGroupPermission<DomainModel>(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            return mockUserContext;
        }
    }
}