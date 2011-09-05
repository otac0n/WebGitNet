//-----------------------------------------------------------------------
// <copyright file="HtmlHelpers.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public static class HtmlHelpers
    {
        public static MvcHtmlString Gravatar(this HtmlHelper html, string email, string name, int size = 72)
        {
            var imgUrl = string.Format(
                "http://www.gravatar.com/avatar/{0}.jpg?s={1}&d=mm&r=g",
                HashString(email),
                size);

            return new MvcHtmlString("<img alt=\"" + html.AttributeEncode(name) + "\" src=\"" + html.AttributeEncode(imgUrl) + "\" />");
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