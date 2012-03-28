//-----------------------------------------------------------------------
// <copyright file="GraphController.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.Models;

    public class GraphController : SharedControllerBase
    {
        public ActionResult ViewGraph(string repo, int page = 1)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory || page < 1)
            {
                return HttpNotFound();
            }

            const int PageSize = 50;
            int skip = PageSize * (page - 1);
            var count = GitUtilities.CountCommits(resourceInfo.FullPath);

            if (skip >= count)
            {
                return HttpNotFound();
            }

            this.BreadCrumbs.Append("Browse", "Index", "Browse");
            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Graph", "ViewGraph", "View Graph", new { repo });

            var commits = GetLogEntries(resourceInfo.FullPath).Skip(skip).Take(PageSize).ToList();

            ViewBag.Page = page;
            ViewBag.PageCount = (count / PageSize) + (count % PageSize > 0 ? 1 : 0);
            ViewBag.RepoName = resourceInfo.Name;

            return View(commits);
        }

        public IEnumerable<GraphEntry> GetLogEntries(string path)
        {
            var hashes = new HashSet<string>();
            var set = new SortedSet<LogEntry>(LogEntryComparer.Instance);

            Action<string> add = h =>
            {
                if (hashes.Add(h))
                {
                    var e = GitUtilities.GetLogEntries(path, 1, @object: h).Single();
                    set.Add(e);
                }
            };

            var refs = GitUtilities.GetAllRefs(path);
            refs.ForEach(r => add(r.ShaId));

            while (set.Count > 0)
            {
                var i = set.Max;
                set.Remove(i);

                i.Parents.ToList().ForEach(p => add(p));

                yield return new GraphEntry
                {
                    LogEntry = i,
                    Refs = refs.Where(r => r.ShaId == i.CommitHash).ToList(),
                };
            }
        }

        private class LogEntryComparer : IComparer<LogEntry>
        {
            private static readonly LogEntryComparer instance = new LogEntryComparer();

            private LogEntryComparer()
            {
            }

            public static LogEntryComparer Instance
            {
                get { return instance; }
            }

            public int Compare(LogEntry x, LogEntry y)
            {
                return x.CommitterDate.CompareTo(y.CommitterDate);
            }
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "View Graph",
                    "browse/{repo}/graph",
                    new { controller = "Graph", action = "ViewGraph", routeName = "View Graph" });

                routes.MapResource("Content/graph.css", "text/css");
            }
        }
    }
}
