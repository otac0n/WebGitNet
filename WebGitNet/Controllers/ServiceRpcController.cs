//-----------------------------------------------------------------------
// <copyright file="ServiceRpcController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using WebGitNet.ActionResults;
    using WebGitNet.Models;

    public class ServiceRpcController : Controller
    {
        private readonly FileManager fileManager;

        public ServiceRpcController()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
            this.fileManager = new FileManager(reposPath);
        }

        [HttpPost]
        public ActionResult UploadPack(string url)
        {
            return this.ServiceRpc(url, "upload-pack");
        }

        [HttpPost]
        public ActionResult ReceivePack(string url)
        {
            return this.ServiceRpc(url, "receive-pack");
        }

        private ActionResult ServiceRpc(string url, string action)
        {
            var resourceInfo = this.fileManager.GetResourceInfo(url);
            if (resourceInfo.FileSystemInfo == null)
            {
                return HttpNotFound();
            }

            var repoPath = ((FileInfo)resourceInfo.FileSystemInfo).Directory.FullName;

            return new GitStreamResult("{0} --stateless-rpc .", action, repoPath);
        }
    }
}
