using System;
using System.Collections.Specialized;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace WhatRoute.Tests
{
    public class TestUrlHelper : UrlHelper
    {
        private Uri _url = new Uri("http://localhost");
        private Mock<RequestContext> MockRequestContext { get; set; }
        protected Mock<HttpResponseBase> MockResponse { get; set; }

        public TestUrlHelper()
            : this(new Mock<RequestContext>(), new Mock<HttpResponseBase>())
        { }

        private TestUrlHelper(Mock<RequestContext> mockRequestContext, Mock<HttpResponseBase> mockResponse)
            : base(mockRequestContext.Object)
        {
            MockRequestContext = mockRequestContext;
            MockResponse = mockResponse;
            SetQueryString(null);
        }

        public TestUrlHelper SetRouteData(object data)
        {
            var routeData = new RouteData();
            foreach (var item in new RouteValueDictionary(data))
                routeData.Values.Add(item.Key, item.Value);

            return SetRouteData(routeData);
        }

        public TestUrlHelper SetRouteData(RouteData data)
        {
            MockRequestContext.SetupProperty(x => x.RouteData, data);
            return this;
        }

        public TestUrlHelper SetUrl(string url)
        {
            _url = new Uri(url);
            return this;
        }

        /// <summary>
        /// Sets up mock behavior for HttpResponseBase.ApplyAppPathModifier method.
        /// </summary>
        /// <param name="modifierFunc">Modification function.</param>
        public TestUrlHelper SetApplyAppPathModifier(Func<string, string> modifierFunc)
        {
            MockResponse.Setup(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns(modifierFunc);
            return this;
        }

        public TestUrlHelper SetQueryString(object data)
        {
            var parameters = new RouteValueDictionary(data ?? new ExpandoObject());
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var nameValueCollection = new NameValueCollection();

            foreach (var parameter in parameters)
            {
                var parameterValue = parameter.Value != null ? parameter.Value.ToString() : "";
                nameValueCollection.Add(parameter.Key, parameterValue);
            }

            mockRequest.SetupGet(x => x.QueryString).Returns(nameValueCollection);
            mockRequest.SetupGet(x => x.Url).Returns(_url);
            mockRequest.SetupGet(x => x.ApplicationPath).Returns("/");

            mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(MockResponse.Object);
            mockHttpContext.SetupGet(x => x.Server).Returns(server.Object);
            mockHttpContext.SetupGet(x => x.Session).Returns(session.Object);

            MockRequestContext.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

            return this;
        }
    }
}