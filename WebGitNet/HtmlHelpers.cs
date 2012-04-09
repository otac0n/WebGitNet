//-----------------------------------------------------------------------
// <copyright file="HtmlHelpers.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using MarkdownSharp;
    using System.Web.Configuration;

    public static class HtmlHelpers
    {
        public static MvcHtmlString Markdown(this HtmlHelper html, string markdown)
        {
            var markdownParser = new Markdown(true);

            return new MvcHtmlString(markdownParser.Transform(markdown));
        }

        public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string name, int size = 72)
        {
            var fallBack = WebConfigurationManager.AppSettings["GravatarFallBack"];
            if (string.IsNullOrEmpty(fallBack))
            {
                fallBack = "mm";
            }

            var imgUrl = string.Format(
                "https://secure.gravatar.com/avatar/{0}.png?s={1}&d={2}&r=g",
                HashString(email),
                size,
                fallBack);

            return new MvcHtmlString("<img alt=\"\" title=\"" + html.AttributeEncode(name) + "\" src=\"" + html.AttributeEncode(imgUrl) + "\" />");
        }

        public static IEnumerable<Route> FindSatisfiableRoutes(this HtmlHelper html, object routeData = null)
        {
            var routes = html.RouteCollection;
            var current = html.ViewContext.RequestContext.RouteData.Route;
            var request = html.ViewContext.RequestContext;
            var routeValues = routeData == null ? html.ViewContext.RouteData.Values : new RouteValueDictionary(routeData);

            return from route in routes.OfType<Route>()
                   where route != current
                   let name = route.GetName()
                   where name != null
                   let routed = route.GetVirtualPath(request, routeValues)
                   where routed != null
                   orderby name
                   select route;
        }

        public static string GetName(this Route route)
        {
            if (route == null || route.Defaults == null)
            {
                return null;
            }

            var routeName = route.Defaults["routeName"];

            if (routeName == null)
            {
                return null;
            }

            return routeName.ToString();
        }

        public static MvcHtmlString Pager(this HtmlHelper html, int page, int pages, string controllerName, string actionName, object routeValues, string routeKey = "page")
        {
            var result = new StringBuilder();

            Action<int, string, bool> renderPageLink = (p, text, active) =>
            {
                string link;

                if (active)
                {
                    var r = new RouteValueDictionary(routeValues);
                    if (p != 1)
                    {
                        r[routeKey] = p;
                    }

                    link = html.ActionLink(text, actionName, controllerName, r, null).ToString();
                }
                else
                {
                    link = string.Format("<a>{0}</a>", html.Encode(text));
                }

                result.Append(link);
            };

            renderPageLink(1, "Newest", page > 1);
            renderPageLink(page - 1, "Newer", page > 1);

            int left = Math.Min(page - 1, 2);
            int right = Math.Min(pages - page, 2);
            int startPage = Math.Max(1, page - left - (2 - right));
            int endPage = Math.Min(pages, page + right + (2 - left));
            for (int p = startPage; p <= endPage; p++)
            {
                renderPageLink(p, p.ToString(), p != page);
            }

            renderPageLink(page + 1, "Older", page < pages);
            renderPageLink(pages, "Oldest", page < pages);

            return new MvcHtmlString(result.ToString());
        }

        private static string HashString(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value.Trim().ToLowerInvariant());
            using (var md5 = new MD5CryptoServiceProvider())
            {
                data = md5.ComputeHash(data);
            }

            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                ret.Append(data[i].ToString("x2").ToLowerInvariant());
            }

            return ret.ToString();
        }
    }
}