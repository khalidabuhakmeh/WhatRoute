using System.Web.Mvc;
using System.Web.Routing;
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
            Url = new TestUrlHelper()
                .SetRouteData(new { action = "index", controller = "home", id = 1 })
                .SetApplyAppPathModifier(x => x);

            RouteTable.Routes.Clear();
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

        [Fact]
        public void Can_generate_path_to_simple_route()
        {
            RouteTable.Routes.MapRoute(
                name: "root",
                url: "",
                defaults: new { controller = "home", action = "index" }
            );

            Url.PathTo("root").Should().Be("/");
        }

        [Fact]
        public void Can_generate_path_to_basic_route()
        {
            RouteTable.Routes.MapRoute(
                name: "home",
                url: "home/index",
                defaults: new { controller = "home", action = "index" }
            );

            Url.PathTo("home").Should().Be("/home/index");
        }

        [Fact]
        public void Can_generate_path_to_complex_route_with_parameters()
        {
            RouteTable.Routes.MapRoute(
               name: "product",
               url: "{controller}/{company}/{sku}",
               defaults: new { controller = "product", action = "show" }
           );

            Url.PathTo("product", new { company = "marvel", sku = "captain-america-shield" })
               .Should().Be("/product/marvel/captain-america-shield");
        }

        [Fact]
        public void Path_to_returns_route_definition_when_not_all_parameters_are_provided()
        {
            RouteTable.Routes.MapRoute(
              name: "product",
              url: "{controller}/{company}/{sku}",
              defaults: new { controller = "product", action = "show" }
            );

            var expected = string.Format(WhatRouteExtensions.RouteMissingParameter, "{controller}/{company}/{sku}");

            // if we are missing any of the parameters the route will not 
            // be generated and you will be left with a null
            Url.PathTo("product", new { /* company = "marvel", */ sku = "captain-america-shield" }).Should().Be(expected);
        }

        [Fact]
        public void Passing_in_null_routename_returns_routeempty()
        {
            Url.PathTo(null).Should().Be(WhatRouteExtensions.RouteNameEmpty);
        }

        [Fact]
        public void Passing_in_bad_name_routename_returns_routeempty()
        {
            const string name = "no_existy";
            var expected = string.Format(WhatRouteExtensions.RouteNotFound, name);
            Url.PathTo(name).Should().Be(expected);
        }

        [Fact]
        public void Can_determine_is_active_by_route_name()
        {
            RouteTable.Routes.MapRoute(
               name: "home",
               url: "home/index",
               defaults: new { controller = "home", action = "index" }
           );

            Url.IsActiveRoute("home").Should().BeTrue();
            Url.IsActiveRoute("poop").Should().BeFalse();
        }

        [Fact]
        public void Can_determine_is_active_by_route_name_returns_css_class()
        {
            RouteTable.Routes.MapRoute(
               name: "home",
               url: "home/index",
               defaults: new { controller = "home", action = "index" }
           );

            Html.IsActiveRouteCss("home").Should().Be(WhatRouteExtensions.DefaultActiveClass);
            Html.IsActiveRouteCss("poop").Should().BeNull();
        }

        [Fact]
        public void Can_determine_complex_route_is_active_by_route_name()
        {
            RouteTable.Routes.MapRoute(
               name: "home",
               url: "home/index/{id}",
               defaults: new { controller = "home", action = "index" }
           );

            Url.IsActiveRoute("home", new { id = 1 }).Should().BeTrue();
            Url.IsActiveRoute("home", new { id = 2 }).Should().BeFalse();
        }
    }
}
