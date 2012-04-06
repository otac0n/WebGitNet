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

            Action<string, int> add = (h, n) =>
            {
                if (!hashes.Contains(h))
                {
                    foreach (var e in GitUtilities.GetLogEntries(path, n, @object: h))
                    {
                        if (hashes.Add(e.CommitHash))
                        {
                            set.Add(e);
                        }
                    }
                }
            };

            var refs = GitUtilities.GetAllRefs(path);
            refs.ForEach(r => add(r.ShaId, 5));

            List<string> incoming = new List<string>();

            while (set.Count > 0)
            {
                var i = set.LastOrDefault(e => !set.Any(o => o.Parents.Contains(e.CommitHash))) ?? set.Max;
                set.Remove(i);

                i.Parents.ToList().ForEach(p => add(p, 50));

                yield return new GraphEntry
                {
                    LogEntry = i,
                    Refs = refs.Where(r => r.ShaId == i.CommitHash).ToList(),
                    IncomingHashes = incoming,
                };

                incoming = BuildOutgoing(incoming, i);
            }
        }

        private List<string> BuildOutgoing(List<string> incoming, LogEntry entry)
        {
            var outgoing = incoming.ToList();
            var col = outgoing.IndexOf(entry.CommitHash);
            if (col == -1)
            {
                col = outgoing.Count;
                outgoing.Add(entry.CommitHash);
            }

            var replaced = false;
            for (var p = 0; p < entry.Parents.Count; p++)
            {
                if (outgoing.IndexOf(entry.Parents[p]) == -1)
                {
                    if (!replaced)
                    {
                        outgoing[col] = entry.Parents[p];
                        replaced = true;
                    }
                    else
                    {
                        outgoing.Add(entry.Parents[p]);
                    }
                }
            }

            if (!replaced)
            {
                outgoing.RemoveAt(col);
            }

            return outgoing;
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
                var c = x.CommitterDate.CompareTo(y.CommitterDate);

                if (c == 0)
                {
                    c = x.CommitHash.CompareTo(y.CommitHash);
                }

                return c;
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
                routes.MapResource("Scripts/graph.js", "text/javascript");
            }
        }
    }
}