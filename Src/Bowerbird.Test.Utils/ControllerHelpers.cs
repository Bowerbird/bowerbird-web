using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Bowerbird.Test.Utils
{
    public static class SetupHelpers
    {

        public static HttpContextBase SetupHttpContext(string url)
        {
            var context = MockHelpers.MockAnonymousHttpContext().Object;

            context.Request.SetupRequestUrl(url);

            return context;
        }

        public static Mock<HttpContextBase> SetupAuthenticatedHttpContext()
        {
            var context = MockHelpers.MockAnonymousHttpContext();
            var user = MockHelpers.MockPrincipal();

            context.Setup(ctx => ctx.User).Returns(user.Object);

            return context;
        }

        public static void SetupControllerContext(this Controller controller)
        {
            var httpContext = MockHelpers.MockAnonymousHttpContext();

            var context = new ControllerContext(new RequestContext(httpContext.Object, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupAuthenticatedControllerContext(this Controller controller)
        {
            var httpContext = SetupAuthenticatedHttpContext().Object;

            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);

            controller.ControllerContext = context;
        }

        public static void SetupRequestUrl(this HttpRequestBase request, string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            if (!url.StartsWith("~/"))
                throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

            var mock = Mock.Get(request);

            mock.Setup(req => req.QueryString)
                .Returns(url.GetQueryStringParameters());

            mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
                .Returns(url.GetUrlFileName());

            mock.Setup(req => req.PathInfo)
                .Returns(string.Empty);
        }

        public static void SetupHttpMethodResult(this HttpRequestBase request, string httpMethod)
        {
            Mock.Get(request)
                .Setup(req => req.HttpMethod)
                .Returns(httpMethod);
        }

        public static void SetupControllerContext(this Controller controller, HttpContextBase context)
        {

        }

    }

}