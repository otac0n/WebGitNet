//-----------------------------------------------------------------------
// <copyright file="FileController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.Web.Configuration;
    using System.Web.Mvc;
    using WebGitNet.Models;

    public class FileController : Controller
    {
        private readonly FileManager fileManager;

        public FileController()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
            this.fileManager = new FileManager(reposPath);
        }

        public ActionResult Fetch(string url)
        {
            ViewBag.Location = url ?? string.Empty;
            return View("List");
        }
    }
}
