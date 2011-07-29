//-----------------------------------------------------------------------
// <copyright file="ServiceRpcController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Mvc;
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
            return ServiceRpc(url, "upload-pack");
        }

        [HttpPost]
        public ActionResult ReceivePack(string url)
        {
            return ServiceRpc(url, "receive-pack");
        }

        private ActionResult ServiceRpc(string url, string action)
        {
            var resourceInfo = this.fileManager.GetResourceInfo(url);
            if (resourceInfo.FileSystemInfo == null)
            {
                return HttpNotFound();
            }

            var repoPath = ((FileInfo)resourceInfo.FileSystemInfo).Directory.FullName;

            string input;
            using (var request = new StreamReader(Request.InputStream, Encoding.GetEncoding(1252)))
            {
                input = request.ReadToEnd();
            }

            string output;
            using (var git = GitUtilities.Start(action + " --stateless-rpc .", repoPath))
            {
                git.StandardInput.Write(input);
                output = git.StandardOutput.ReadToEnd();
            }

            return Content(output, "application/git-" + action + "-result");
        }
    }
}
