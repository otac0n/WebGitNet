﻿//-----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.Models;

    using io = System.IO;

    public class ManageController : SharedControllerBase
    {
        public ManageController()
        {
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateRepoRequest request)
        {
            string repoPath = null;

            if (ModelState.IsValid)
            {
                if( !AreWeRepoCreator )
                    return new HttpStatusCodeResult(403, "You do not have permission to create repositories");
                var invalid = Path.GetInvalidFileNameChars();

                if (request.RepoName.Any(c => invalid.Contains(c)))
                {
                    ModelState.AddModelError("RepoName", "Repository name must be a valid folder name.");
                }
                else
                {
                    var resourceInfo = this.FileManager.GetResourceInfo(request.RepoName);

                    if (resourceInfo.FileSystemInfo == null)
                    {
                        ModelState.AddModelError("RepoName", "You do not have permission to create this repository.");
                    }

                    if (resourceInfo.Type != ResourceType.NotFound)
                    {
                        ModelState.AddModelError("RepoName", "There is already an object at that location.");
                    }

                    repoPath = resourceInfo.FullPath;
                }
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                GitUtilities.CreateRepo(repoPath);
            }
            catch (GitErrorException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }

            io::File.WriteAllText(Path.Combine(repoPath, "description"), request.Description);

            GitUtilities.ExecutePostCreateHook(repoPath);

            return RedirectToAction("ViewRepo", "Browse", new { repo = request.RepoName });
        }

        public ActionResult ManageRepo(string repoName)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repoName);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            var repo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);
            if (!repo.IsGitRepo)
            {
                return HttpNotFound();
            }

            return View(new RepoSettings
            {
                Description = repo.Description,
                IsArchived = repo.IsArchived,
            });
        }

        [HttpPost]
        public ActionResult ManageRepo(string repoName, RepoSettings settings)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repoName);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            var repo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);
            if (!repo.IsGitRepo)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(settings);
            }

            io::File.WriteAllText(Path.Combine(resourceInfo.FullPath, "description"), settings.Description);
            if (repo.IsArchived != settings.IsArchived)
            {
                GitUtilities.ToggleArchived(resourceInfo.FullPath);
            }

            return RedirectToAction("ViewRepo", "Browse", new { repo = repoName });
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "Create Repo",
                    "create",
                    new { controller = "Manage", action = "Create" });

                routes.MapRoute(
                    "Manage Repo",
                    "manage/{repoName}",
                    new { controller = "Manage", action = "ManageRepo" });
            }
        }
    }
}