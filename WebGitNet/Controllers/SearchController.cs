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

    public class SearchController : Controller
    {
        public ActionResult Search(string q)
        {
            return View(new List<SearchResult>());
        }

        public ActionResult SearchRepo(string repo, string q)
        {
            return View("Search", new List<SearchResult>());
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
