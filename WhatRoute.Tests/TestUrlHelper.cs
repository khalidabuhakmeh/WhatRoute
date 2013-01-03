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
        private Mock<RequestContext> MockRequestContext { get; set; }

        public TestUrlHelper()
            : this(new Mock<RequestContext>())
        { }

        private TestUrlHelper(Mock<RequestContext> mockRequestContext)
            : base(mockRequestContext.Object)
        {
            MockRequestContext = mockRequestContext;
            SetQueryString(null);
        }

        public void SetRouteData(object data)
        {
            var routeData = new RouteData();
            foreach (var item in new RouteValueDictionary(data))
                routeData.Values.Add(item.Key, item.Value);

            SetRouteData(routeData);
        }

        public TestUrlHelper SetRouteData(RouteData data)
        {
            MockRequestContext.SetupProperty(x => x.RouteData, data);
            return this;
        }

        public TestUrlHelper SetQueryString(object data)
        {
            var parameters = new RouteValueDictionary(data ?? new ExpandoObject());
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockRequest = new Mock<HttpRequestBase>();
            var nameValueCollection = new NameValueCollection();

            foreach (var parameter in parameters)
            {
                var parameterValue = parameter.Value != null ? parameter.Value.ToString() : "";
                nameValueCollection.Add(parameter.Key, parameterValue);
            }

            mockRequest.SetupGet(x => x.QueryString).Returns(nameValueCollection);
            mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);
            MockRequestContext.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

            return this;
        }
    }
}