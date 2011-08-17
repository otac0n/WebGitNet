//-----------------------------------------------------------------------
// <copyright file="HtmlHelpers.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;

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