//-----------------------------------------------------------------------
// <copyright file="RouteCollectionHelpers.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Reflection;

    public static class RouteCollectionHelpers
    {
        public static void MapResource(this RouteCollection routes, string resourcePath, string contentType)
        {
            var assembly = Assembly.GetCallingAssembly();
            var assemblyName = assembly.GetName().Name;
            var resourceName = assembly.GetResourceName(resourcePath);

            routes.MapRoute(
                assemblyName + " " + resourcePath,
                resourcePath,
                new
                {
                    assemblyName,
                    resourceName,
                    contentType,
                    controller = "PluginContent",
                    action = "Resource"
                });
        }

        public static string GetResourceName(this Assembly assembly, string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                return "";
            }

            var assemblyName = assembly.GetName().Name;

            if (virtualPath.StartsWith("~/"))
            {
                virtualPath = virtualPath.Substring(2);
            }
            else if (virtualPath.StartsWith("/"))
            {
                virtualPath = virtualPath.Substring(1);
            }

            return (assemblyName + "." + virtualPath)
                .Replace(@"/", ".")
                .Replace(@"\", ".");
        }
    }
}
