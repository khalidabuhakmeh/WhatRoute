using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace WhatRoute.Core
{
    public static class WhatRouteExtensions
    {
        public const string DefaultActiveClass = "active";

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

            var yourRouteValues = new RouteValueDictionary(routeValues);
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
            var yourQuerystringValues = new RouteValueDictionary(querystringValues);

            foreach (string key in helper.RequestContext.HttpContext.Request.QueryString.Keys)
                querystring.Add(key, helper.RequestContext.HttpContext.Request.QueryString[key]);

            foreach (var key in yourQuerystringValues.Keys)
                match &= querystring.ContainsKey(key) && AreEqual(querystring[key], yourQuerystringValues[key]);

            return match;
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

        private static bool AreEqual(object one, object two)
        {
            return string.Compare((one ?? new object()).ToString(), (two ?? new object()).ToString(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
