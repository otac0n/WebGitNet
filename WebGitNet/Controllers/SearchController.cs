//-----------------------------------------------------------------------
// <copyright file="BrowseController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.Search;
    using Castle.Windsor;
    using System.Threading.Tasks;

    public class SearchController : SharedControllerBase
    {
        private readonly IWindsorContainer container;

        public SearchController(IWindsorContainer container)
        {
            this.container = container;
        }

        public ActionResult Search(string q)
        {
            return Search(q, null);
        }

        public ActionResult SearchRepo(string repo, string q)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            var repoInfo = GitUtilities.GetRepoInfo(repo);
            if (!repoInfo.IsGitRepo)
            {
                return HttpNotFound();
            }

            return Search(q, repoInfo);
        }

        private ActionResult Search(string q, RepoInfo repoInfo)
        {
            var searchProviders = container.ResolveAll<ISearchProvider>();

            var query = new SearchQuery(q);

            var results = (from p in searchProviders
                           select p.Search(query, this.FileManager, repoInfo, 0, 10)).ToArray();

            Task.WaitAll(results);

            return View("Search", results.Select(r => r.Result).SelectMany(r => r));
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "Search All",
                    "search",
                    new { controller = "Search", action = "Search" });

                routes.MapRoute(
                    "Search Repo",
                    "browse/{repo}/search",
                    new { controller = "Search", action = "SearchRepo" });
            }
        }
    }
}
