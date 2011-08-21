//-----------------------------------------------------------------------
// <copyright file="SharedControllerBase.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Controllers
{
    using System.Web.Configuration;
    using System.Web.Mvc;
    using WebGitNet.Models;

    public abstract class SharedControllerBase : Controller
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
