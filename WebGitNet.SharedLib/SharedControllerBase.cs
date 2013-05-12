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
        private bool areWeLimitedReader;
        private bool areWeRepoCreator;

        public SharedControllerBase()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];

            this.fileManager = new FileManager(reposPath);

            this.breadCrumbs = new BreadCrumbTrail();
            ViewBag.BreadCrumbs = this.breadCrumbs;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            // Set our user access rights flags
            areWeLimitedReader = areWeInReadOnlyLimitedGroup();
            areWeRepoCreator = areWeInRepoCreatorGroup();
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

        protected bool AreWeLimitedReader
        {
            get
            {
                return this.areWeLimitedReader;
            }
        }

        public bool AreWeRepoCreator
        {
            get
            {
                return this.areWeRepoCreator;
            }
        }

        private bool areWeInConfigGroup(string groupKeyName)
        {
            var clientPrincipal = (System.Security.Principal.WindowsPrincipal)User;
            var groupName = WebConfigurationManager.AppSettings[groupKeyName];
            return clientPrincipal.IsInRole(groupName);
        }

        private bool areWeInReadOnlyLimitedGroup()
        {
            return areWeInConfigGroup("ReadOnlyLimitedGroupName");
        }

        private bool areWeInRepoCreatorGroup()
        {
            return areWeInConfigGroup("RepoCreatorsGroupName");
        }
    }
}
