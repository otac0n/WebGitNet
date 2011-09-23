//-----------------------------------------------------------------------
// <copyright file="ImpactController.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;
    using WebGitNet.Models;

    public class ImpactController : SharedControllerBase
    {
        public ActionResult ViewRepoImpact(string repo)
        {
            var resourceInfo = this.FileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            AddRepoBreadCrumb(repo);
            this.BreadCrumbs.Append("Impact", "ViewRepoImpact", "Impact", new { repo });

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

            ViewBag.AllTime = allTimeImpacts;
            ViewBag.Weekly = weeklyImpacts;
            return View();
        }

        public class RouteRegisterer : IRouteRegisterer
        {
            public void RegisterRoutes(RouteCollection routes)
            {
                routes.MapRoute(
                    "View Repo Impact",
                    "browse/{repo}/impact",
                    new { controller = "Impact", action = "ViewRepoImpact", routeName = "View Repo Impact" });
            }
        }
    }
}
