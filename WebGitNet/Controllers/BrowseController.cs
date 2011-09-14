//-----------------------------------------------------------------------
// <copyright file="BrowseController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using WebGitNet.ActionResults;
    using WebGitNet.Models;

    public class BrowseController : SharedControllerBase
    {
        public BrowseController()
        {
            this.BreadCrumbs.Append("Browse", "Index", "Browse");
        }

        private void AddRepoBreadCrumb(string repo)
        {
            this.BreadCrumbs.Append("Browse", "ViewRepo", repo, new { repo });
        }

        public ActionResult Index()
        {
            var directory = this.FileManager.DirectoryInfo;

            var repos = (from dir in directory.EnumerateDirectories()
                         select dir.Name).ToList();

            return View(repos);
        }

        public ActionResult ViewRepo(string repo)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);

            var lastCommit = GitUtilities.GetLogEntries(resourceInfo.FullPath, 1).FirstOrDefault();

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.LastCommit = lastCommit;
            ViewBag.CurrentTree = lastCommit != null ? GitUtilities.GetTreeInfo(resourceInfo.FullPath, "HEAD") : null;
            ViewBag.Refs = GitUtilities.GetAllRefs(resourceInfo.FullPath);

            return View();
        }

        public ActionResult ViewRepoImpact(string repo)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewRepoImpact", "Impact", new { repo });

            var userImpacts = GitUtilities.GetUserImpacts(resourceInfo.FullPath);

            var allTimeImpacts = (from g in userImpacts.GroupBy(u => u.Author, StringComparer.InvariantCultureIgnoreCase)
                                  select new UserImpact
                                  {
                                      Author = g.Key,
                                      Commits = g.Sum(ui => ui.Commits),
                                      Insertions = g.Sum(ui => ui.Insertions),
                                      Deletions = g.Sum(ui => ui.Deletions),
                                      Impact = g.Sum(ui => ui.Impact),
                                  }).OrderByDescending(i => i.Commits);

            var weeklyImpacts = (from u in userImpacts
                                 let dayOffset = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - u.Date.DayOfWeek
                                 let commitWeek = u.Date.Date.AddDays(dayOffset + (dayOffset > 0 ? -7 : 0))
                                 group u by commitWeek into wk
                                 select new ImpactWeek
                                 {
                                     Week = wk.Key,
                                     Impacts = (from g in wk.GroupBy(u => u.Author, StringComparer.InvariantCultureIgnoreCase)
                                                select new UserImpact
                                                {
                                                    Author = g.Key,
                                                    Commits = g.Sum(ui => ui.Commits),
                                                    Insertions = g.Sum(ui => ui.Insertions),
                                                    Deletions = g.Sum(ui => ui.Deletions),
                                                    Impact = g.Sum(ui => ui.Impact),
                                                }).OrderByDescending(i => i.Commits).ToList()
                                 }).OrderBy(wk => wk.Week);

            return View(new RepoImpact { AllTime = allTimeImpacts, Weekly = weeklyImpacts });
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

            var diffs = GitUtilities.GetDiffInfo(resourceInfo.FullPath, commit.CommitHash);

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.CommitLogEntry = commit;

            return View(diffs);
        }

        public ActionResult ViewCommits(string repo, int page = 1)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory || page < 1)
            {
                return HttpNotFound();
            }

            const int pageSize = 20;
            int skip = pageSize * (page - 1);
            var count = GitUtilities.CountCommits(resourceInfo.FullPath);

            if (skip >= count)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Browse", "ViewCommits", "Commit Log", new { repo });

            var commits = GitUtilities.GetLogEntries(resourceInfo.FullPath, pageSize, skip);

            ViewBag.Page = page;
            ViewBag.PageCount = (count / pageSize) + (count % pageSize > 0 ? 1 : 0);
            ViewBag.RepoName = resourceInfo.Name;

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
    }
}
