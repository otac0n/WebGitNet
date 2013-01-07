//-----------------------------------------------------------------------
// <copyright file="FileController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.ActionResults;

    public class FileController : SharedControllerBase
    {
        public FileController()
        {
            this.BreadCrumbs.Append("File", "Fetch", "Direct Access", new { url = string.Empty });
        }

        public ActionResult Fetch(string url)
        {
            url = url ?? string.Empty;
            var resourceInfo = this.FileManager.GetResourceInfo(url);

            if (resourceInfo.Type == ResourceType.NotFound)
            {
                return HttpNotFound();
            }

            if (resourceInfo.Type == ResourceType.File)
            {
                return File(resourceInfo.FullPath, "application/octet-stream");
            }

            this.BreadCrumbs.Append("File", "Fetch", BreadCrumbTrail.EnumeratePath(url), p => p.Key, p => new { url = p.Value });

            return View("List", resourceInfo);
        }

        public ActionResult GetInfoRefs(string url)
        {
            var service = this.GetService();
            var resourceInfo = this.FileManager.GetResourceInfo(url);

            if (resourceInfo.Type != ResourceType.Directory)
            {
                var repoPath = ((FileInfo)resourceInfo.FileSystemInfo).Directory.Parent.FullName;
                GitUtilities.UpdateServerInfo(repoPath);

                if (resourceInfo.Type == ResourceType.NotFound)
                {
                    resourceInfo = this.FileManager.GetResourceInfo(url);
                }
            }

            if (service == null || resourceInfo.Type == ResourceType.Directory)
            {
                return this.Fetch(url);
            }
            else
            {
                var repoPath = ((FileInfo)resourceInfo.FileSystemInfo).Directory.Parent.FullName;

                return new GitCommandResult("{0} --stateless-rpc --advertise-refs .", service, repoPath);
            }
        }

        private string GetService()
        {
            var serviceType = this.Request.QueryString["service"];
            if (string.IsNullOrEmpty(serviceType) || !serviceType.StartsWith("git-"))
            {
                return null;
            }

            return serviceType.Substring(4);
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "Get */info/refs",
                    "git/{*url}",
                    new { controller = "File", action = "GetInfoRefs" },
                    new { url = @"(.*?)/info/refs" });

                routes.MapRoute(
                    "Post */git-upload-pack",
                    "git/{*url}",
                    new { controller = "ServiceRpc", action = "UploadPack" },
                    new { url = @"(.*?)/git-upload-pack" });

                routes.MapRoute(
                    "Post */git-receive-pack",
                    "git/{*url}",
                    new { controller = "ServiceRpc", action = "ReceivePack" },
                    new { url = @"(.*?)/git-receive-pack" });

                routes.MapRoute(
                    "File Access",
                    "git/{*url}",
                    new { controller = "File", action = "Fetch" });
            }
        }
    }
}
