using System;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Routing;

namespace WhatRoute.Core
{
    public static class WhatRouteExtensions
    {
        public const string DefaultActiveClass = "active";
        public const string RouteNameEmpty = "/__route_name_empty";
        public const string RouteNotFound = "/__route_not_found/{0}";
        public const string RouteMissingParameter = "/__missing_parameter/{0}";

        public static bool IsActive(this UrlHelper helper, string action, string controller, string area = "")
        {
            return IsActive(helper, new { action, controller, area });
        }

        public static bool IsActive(this UrlHelper helper, object routeValues = null, object querystringValues = null)
        {
            var match = true;

            if (routeValues == null && querystringValues == null)
            {
                return false;
            }

            var yourRouteValues = (routeValues is RouteValueDictionary)
                ? (RouteValueDictionary)routeValues
                : new RouteValueDictionary(routeValues);

            var routeData = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            // default area to empty string
            const string areaKey = "area";
            if (!routeData.ContainsKey(areaKey))
            {
                var value = helper.RequestContext.RouteData.DataTokens.ContainsKey(areaKey)
                                ? helper.RequestContext.RouteData.DataTokens[areaKey]
                                : "";

                routeData.Add(areaKey, value);
            }

            foreach (var key in yourRouteValues.Keys)
                match &= routeData.ContainsKey(key) && AreEqual(routeData[key], yourRouteValues[key]);

            var querystring = new RouteValueDictionary();
            var yourQuerystringValues = (querystringValues is RouteValueDictionary)
                ? (RouteValueDictionary)querystringValues
                : new RouteValueDictionary(querystringValues);

            foreach (string key in helper.RequestContext.HttpContext.Request.QueryString.Keys)
                querystring.Add(key, helper.RequestContext.HttpContext.Request.QueryString[key]);

            foreach (var key in yourQuerystringValues.Keys)
                match &= querystring.ContainsKey(key) && AreEqual(querystring[key], yourQuerystringValues[key]);

            return match;
        }

        public static bool IsActiveRoute(this UrlHelper helper, string routeName, object routeValues = null, object querystringValues = null)
        {
            if (string.IsNullOrWhiteSpace(routeName)) return false;
            var route = RouteTable.Routes[routeName] as Route;

            if (route == null) return false;

            var defaults = new RouteValueDictionary(route.Defaults);
            var yours = new RouteValueDictionary(routeValues);

            foreach (var key in yours.Keys)
                defaults[key] = yours[key];

            return IsActive(helper, defaults, querystringValues);
        }

        public static string IsActiveRouteCss(this HtmlHelper helper, string routeName, object routeValues = null, object querystringValues = null, string cssClass = DefaultActiveClass)
        {
            return IsActiveRoute(new UrlHelper(helper.ViewContext.RequestContext), routeName, routeValues, querystringValues)
                ? DefaultActiveClass
                : null;
        }

        public static string IsActiveCss(this HtmlHelper helper, object routeValues = null, object queryStringValues = null, string cssClass = DefaultActiveClass)
        {
            return IsActive(new UrlHelper(helper.ViewContext.RequestContext), routeValues, queryStringValues)
                       ? cssClass
                       : null;
        }

        public static string IsActiveCss(this HtmlHelper helper, string action, string controller, string area = "", string cssClass = DefaultActiveClass)
        {
            return IsActive(new UrlHelper(helper.ViewContext.RequestContext), action, controller, area)
                       ? cssClass
                       : null;
        }

        public static string PathTo(this UrlHelper helper, string routeName, object routeValues = null)
        {
            if (string.IsNullOrWhiteSpace(routeName))
                return RouteNameEmpty;

            var route = RouteTable.Routes[routeName] as Route;

            if (route == null)
                return string.Format(RouteNotFound, routeName);

            var start = new RouteValueDictionary(route.Defaults);

            // then merge in existing route data
            var routedata = helper.RequestContext.RouteData;
            var area = routedata.DataTokens.ContainsKey("area")
                           ? routedata.DataTokens["area"]
                           : "";
            // add the area
            start.Add("area", area);

            // then yours
            var yours = new RouteValueDictionary(routeValues);

            foreach (var key in yours.Keys)
                start[key] = yours[key];

            var url = helper.RouteUrl(routeName, start);

            return string.IsNullOrWhiteSpace(url)
                ? string.Format(RouteMissingParameter, route.Url)
                : url;
        }

        private static bool AreEqual(object one, object two)
        {
            return string.Compare((one ?? new object()).ToString(), (two ?? new object()).ToString(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
