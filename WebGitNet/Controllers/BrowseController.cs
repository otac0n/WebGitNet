//-----------------------------------------------------------------------
// <copyright file="BrowseController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.ActionResults;
    using WebGitNet.Models;

    public class BrowseController : SharedControllerBase
    {
        public BrowseController()
        {
            this.BreadCrumbs.Append("Browse", "Index", "Browse");
        }

        public ActionResult Index(bool archived = false)
        {
            var directory = this.FileManager.DirectoryInfo;

            ViewBag.Archived = archived;
            var repos = (from dir in directory.EnumerateDirectories()
                         select GitUtilities.GetRepoInfo(dir.FullName)).Where(ri => ri.IsArchived == archived).ToList();

            return View(repos);
        }

        public ActionResult ViewRepo(string repo)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            var repoInfo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);
            if (!repoInfo.IsGitRepo)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);

            var lastCommit = GitUtilities.GetLogEntries(resourceInfo.FullPath, 1).FirstOrDefault();

            ViewBag.RepoInfo = GitUtilities.GetRepoInfo(resourceInfo.FullPath);
            ViewBag.LastCommit = lastCommit;
            ViewBag.CurrentTree = lastCommit != null ? GitUtilities.GetTreeInfo(resourceInfo.FullPath, "HEAD") : null;
            ViewBag.Refs = GitUtilities.GetAllRefs(resourceInfo.FullPath);

            return View();
        }

        public ActionResult ViewCommit(string repo, string @object)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewCommit", @object, new { repo, @object });

            var commit = GitUtilities.GetLogEntries(resourceInfo.FullPath, 1, 0, @object).FirstOrDefault();
            if (commit == null)
            {
                return HttpNotFound();
            }

            var diffs = GitUtilities.GetDiffInfo(resourceInfo.FullPath, commit.Parents.FirstOrDefault() ?? GitUtilities.EmptyTreeHash, commit.CommitHash);

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.CommitLogEntry = commit;

            return View(diffs);
        }

        public ActionResult ViewCommits(string repo, string @object = null, int page = 1)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory || page < 1)
            {
                return HttpNotFound();
            }

            const int PageSize = 20;
            int skip = PageSize * (page - 1);
            var count = GitUtilities.CountCommits(resourceInfo.FullPath);

            if (skip >= count)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewCommits", "Commit Log", new { repo, @object });

            var commits = GitUtilities.GetLogEntries(resourceInfo.FullPath, PageSize, skip, @object);
            var branches = GitUtilities.GetAllRefs(resourceInfo.FullPath).Where(r => r.RefType == RefType.Branch).ToList();

            ViewBag.PaginationInfo = new PaginationInfo(page, (count + PageSize - 1) / PageSize, "Browse", "ViewCommits", new { repo });
            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.Object = @object ?? "HEAD";
            ViewBag.Branches = branches;

            return View(commits);
        }

        public ActionResult ViewTree(string repo, string @object, string path)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            TreeView items;
            try
            {
                items = GitUtilities.GetTreeInfo(resourceInfo.FullPath, @object, path);
            }
            catch (GitErrorException)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewTree", @object, new { repo, @object, path = string.Empty });
            this.BreadCrumbs.Append("Browse", "ViewTree", BreadCrumbTrail.EnumeratePath(path), p => p.Key, p => new { repo, @object, path = p.Value });

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.Tree = @object;
            ViewBag.Path = path ?? string.Empty;

            return View(items);
        }

        public ActionResult ViewBlob(string repo, string @object, string path, bool raw = false)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory || string.IsNullOrEmpty(path))
            {
                return HttpNotFound();
            }

            var fileName = Path.GetFileName(path);
            var containingPath = path.Substring(0, path.Length - fileName.Length);

            TreeView items;
            try
            {
                items = GitUtilities.GetTreeInfo(resourceInfo.FullPath, @object, containingPath);
            }
            catch (GitErrorException)
            {
                return HttpNotFound();
            }

            if (!items.Objects.Any(o => o.Name == fileName))
            {
                return HttpNotFound();
            }

            var contentType = MimeUtilities.GetMimeType(fileName);

            if (raw)
            {
                return new GitFileResult(resourceInfo.FullPath, @object, path, contentType);
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewTree", @object, new { repo, @object, path = string.Empty });
            var paths = BreadCrumbTrail.EnumeratePath(path, TrailingSlashBehavior.LeaveOffLastTrailingSlash).ToList();
            this.BreadCrumbs.Append("Browse", "ViewTree", paths.Take(paths.Count() - 1), p => p.Key, p => new { repo, @object, path = p.Value });
            this.BreadCrumbs.Append("Browse", "ViewBlob", paths.Last().Key, new { repo, @object, path = paths.Last().Value });

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.Tree = @object;
            ViewBag.Path = path;
            ViewBag.FileName = fileName;
            ViewBag.ContentType = contentType;
            string model = null;

            if (contentType.StartsWith("text/") || contentType == "application/xml" || Regex.IsMatch(contentType, @"^application/.*\+xml$"))
            {
                using (var blob = GitUtilities.GetBlob(resourceInfo.FullPath, @object, path))
                {
                    using (var reader = new StreamReader(blob, detectEncodingFromByteOrderMarks: true))
                    {
                        model = reader.ReadToEnd();
                    }
                }
            }

            return View((object)model);
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "Browse Index",
                    "browse",
                    new { controller = "Browse", action = "Index" });

                routes.MapRoute(
                    "Browse Archived Index",
                    "browse/archivedindex",
                    new { controller = "Browse", action = "ArchivedIndex" });

                routes.MapRoute(
                    "View Repo",
                    "browse/{repo}",
                    new { controller = "Browse", action = "ViewRepo" });

                routes.MapRoute(
                    "View Tree",
                    "browse/{repo}/tree/{object}/{*path}",
                    new { controller = "Browse", action = "ViewTree", path = UrlParameter.Optional });

                routes.MapRoute(
                    "View Blob",
                    "browse/{repo}/blob/{object}/{*path}",
                    new { controller = "Browse", action = "ViewBlob", path = UrlParameter.Optional });

                routes.MapRoute(
                    "View Commit",
                    "browse/{repo}/commit/{object}",
                    new { controller = "Browse", action = "ViewCommit" });

                routes.MapRoute(
                    "View Commit Log",
                    "browse/{repo}/commits/{object}",
                    new { controller = "Browse", action = "ViewCommits", routeName = "View Commit Log", routeIcon = "list", @object = UrlParameter.Optional });
            }
        }
    }
}