//-----------------------------------------------------------------------
// <copyright file="SharedControllerBase.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Web.Configuration;
    using System.Web.Mvc;

    public abstract partial class SharedControllerBase : Controller
    {
        private readonly FileManager fileManager;
        private readonly BreadCrumbTrail breadCrumbs;

        public SharedControllerBase()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];

            this.fileManager = new FileManager(reposPath);

            this.breadCrumbs = new BreadCrumbTrail();
            ViewBag.BreadCrumbs = this.breadCrumbs;
        }

        protected void AddRepoBreadCrumb(string repo)
        {
            this.BreadCrumbs.Append("Browse", "ViewRepo", repo, new { repo });
        }

        public FileManager FileManager
        {
            get
            {
                return this.fileManager;
            }
        }

        public BreadCrumbTrail BreadCrumbs
        {
            get
            {
                return this.breadCrumbs;
            }
        }
    }
}
