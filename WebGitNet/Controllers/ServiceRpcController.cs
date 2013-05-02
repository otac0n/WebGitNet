//-----------------------------------------------------------------------
// <copyright file="ServiceRpcController.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Web.Mvc;
    using WebGitNet.ActionResults;

    public class ServiceRpcController : SharedControllerBase
    {
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
            var resourceInfo = this.FileManager.GetResourceInfo(url);
            if (resourceInfo.FileSystemInfo == null)
            {
                return HttpNotFound();
            }

            var repoPath = ((FileInfo)resourceInfo.FileSystemInfo).Directory.FullName;

            return new GitStreamResult("{0} --stateless-rpc .", action, repoPath);
        }
    }
}
