//-----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using WebGitNet.Models;
    using io = System.IO;

    public class ManageController : Controller
    {
        private readonly FileManager fileManager;

        public ManageController()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
            this.fileManager = new FileManager(reposPath);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateRepoRequest request)
        {
            var invalid = Path.GetInvalidFileNameChars();
            if (request.RepoName.Any(c => invalid.Contains(c)))
            {
                ModelState.AddModelError("RepoName", "Repository name must be a valid folder name.");
            }

            var resourceInfo = this.fileManager.GetResourceInfo(request.RepoName);
            if (resourceInfo.FileSystemInfo == null)
            {
                ModelState.AddModelError("RepoName", "You do not have permission to create this repository.");
            }

            if (resourceInfo.Type != ResourceType.NotFound)
            {
                ModelState.AddModelError("RepoName", "There is already an object at that location.");
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var repoPath = resourceInfo.FullPath;

            try
            {
                GitUtilities.CreateRepo(repoPath);
            }
            catch (GitUtilities.CreateRepoFailedException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }

            io::File.WriteAllText(Path.Combine(repoPath, "description"), request.Description);

            GitUtilities.ExecutePostCreateHook(repoPath);

            return RedirectToAction("ViewRepo", "Browse", new { repo = request.RepoName });
        }
    }
}
