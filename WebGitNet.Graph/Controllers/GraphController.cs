//-----------------------------------------------------------------------
// <copyright file="GraphController.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class GraphController : SharedControllerBase
    {
        public ActionResult ViewGraph(string repo)
        {
            return View();
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "View Graph",
                    "browse/{repo}/graph",
                    new { controller = "Graph", action = "ViewGraph", routeName = "View Graph" });

                routes.MapResource("Scripts/repo-impact.js", "text/javascript");
                routes.MapResource("Scripts/g.raphael/g.impact.js", "text/javascript");
            }
        }
    }
}
