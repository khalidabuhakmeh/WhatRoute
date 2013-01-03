using System.Web.Mvc;
using FluentAssertions;
using WhatRoute.Core;
using Xunit;

namespace WhatRoute.Tests
{
    public class WhatRouteExtensionsTests
    {
        private TestUrlHelper Url { get; set; }
        private HtmlHelper Html { get { return new HtmlHelper(new ViewContext { RequestContext = Url.RequestContext }, new ViewPage()); } }

        public WhatRouteExtensionsTests()
        {
            Url = new TestUrlHelper();
            Url.SetRouteData(new { action = "index", controller = "home", id = 1 });
        }

        [Fact]
        public void Can_match_on_action()
        {
            Url.IsActive(new { action = "index" }).Should().BeTrue();
        }

        [Fact]
        public void Does_not_match_when_action_are_different()
        {
            Url.IsActive(new { action = "show" }).Should().BeFalse();
        }

        [Fact]
        public void Can_match_on_controller()
        {
            Url.IsActive(new { controller = "home" }).Should().BeTrue();
        }

        [Fact]
        public void Does_not_match_when_controller_is_different()
        {
            Url.IsActive(new { controller = "products" }).Should().BeFalse();
        }

        [Fact]
        public void Can_match_on_area()
        {
            Url.IsActive(new { area = "" }).Should().BeTrue();
        }

        [Fact]
        public void Does_not_match_when_area_is_different()
        {
            Url.IsActive(new { area = "api" }).Should().BeFalse();
        }

        [Fact]
        public void Can_match_on_other_route_parameter()
        {
            Url.IsActive(new { id = 1 }).Should().BeTrue();
        }

        [Fact]
        public void Does_not_match_when_route_parameter_is_different()
        {
            Url.IsActive(new { id = 2 }).Should().BeFalse();
        }

        [Fact]
        public void Can_get_partial_match()
        {
            Url.IsActive(new { action = "index", id = 1 }).Should().BeTrue();
        }

        [Fact]
        public void Can_match_on_querystring_parameters()
        {
            Url.SetQueryString(new { q = "butter" });
            Url.IsActive(null, new { q = "butter" }).Should().BeTrue();
        }

        [Fact]
        public void Can_match_based_on_route_and_querystring_parameters()
        {
            Url.SetQueryString(new { q = "chicken" });
            Url.IsActive(new { action = "index", controller = "home" }, new { q = "chicken" }).Should().BeTrue();
        }

        [Fact]
        public void Does_not_match_when_querystring_parameter_is_incorrect()
        {
            Url.SetQueryString(new { q = "chicken" });
            Url.IsActive(new { action = "index", controller = "home" }, new { q = "beef" }).Should().BeFalse();
        }

        [Fact]
        public void Can_return_css_class_when_active()
        {
            Html.IsActiveCss(new { action = "index", controller = "home" }).Should().Be(WhatRouteExtensions.DefaultActiveClass);
        }

        [Fact]
        public void Can_return_css_class_when_active_with_action_and_controller()
        {
            Html.IsActiveCss("index", "home").Should().Be(WhatRouteExtensions.DefaultActiveClass);
        }

        [Fact]
        public void Can_retun_null_when_not_active()
        {
            Html.IsActiveCss(new { action = "index", controller = "products" }).Should().Be(null);
        }

        [Fact]
        public void Can_return_a_different_css_class_when_active()
        {
            Html.IsActiveCss(new { action = "index", controller = "home" }, cssClass: "yes").Should().Be("yes");
        }

        [Fact]
        public void Can_match_using_other_active_method()
        {
            Url.IsActive("index", "home").Should().BeTrue();
        }

        [Fact]
        public void Can_not_break_by_passing_in_null()
        {
            Url.IsActive().Should().BeFalse();
        }
    }
}
