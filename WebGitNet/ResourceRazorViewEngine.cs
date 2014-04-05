//-----------------------------------------------------------------------
// <copyright file="ResourceRazorViewEngine.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Linq;
    using System.Web.Mvc;

    public class ResourceRazorViewEngine : RazorViewEngine
    {
        private static readonly string[] areaViewLocationFormats = new[]
        {
            "~/Views/Plugins/{0}/Areas/{{2}}/{{1}}/{{0}}.cshtml",
            "~/Views/Plugins/{0}/Areas/{{2}}/{{1}}/{{0}}.vbhtml",
            "~/Views/Plugins/{0}/Areas/{{2}}/Shared/{{0}}.cshtml",
            "~/Views/Plugins/{0}/Areas/{{2}}/Shared/{{0}}.vbhtml",
        };

        private static readonly string[] empty = new string[0];

        private static readonly string[] viewLocationFormats = new[]
        {
            "~/Views/Plugins/{0}/{{1}}/{{0}}.cshtml",
            "~/Views/Plugins/{0}/{{1}}/{{0}}.vbhtml",
            "~/Views/Plugins/{0}/Shared/{{0}}.cshtml",
            "~/Views/Plugins/{0}/Shared/{{0}}.vbhtml",
        };

        private readonly object syncRoot = new object();

        public ResourceRazorViewEngine()
        {
            ClearFormats();
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            lock (this.syncRoot)
            {
                this.SetFormats(controllerContext);
                var result = base.FindPartialView(controllerContext, partialViewName, useCache);
                this.ClearFormats();
                return result;
            }
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            lock (this.syncRoot)
            {
                this.SetFormats(controllerContext);
                var result = base.FindView(controllerContext, viewName, masterName, useCache);
                this.ClearFormats();
                return result;
            }
        }

        private void ClearFormats()
        {
            this.ViewLocationFormats =
            this.MasterLocationFormats =
            this.PartialViewLocationFormats = empty;

            this.AreaViewLocationFormats =
            this.AreaMasterLocationFormats =
            this.AreaPartialViewLocationFormats = empty;
        }

        private void SetFormats(ControllerContext controllerContext)
        {
            var controllerType = controllerContext.Controller.GetType();
            var assemblyName = controllerType.Assembly.GetName().Name;

            this.ViewLocationFormats =
            this.MasterLocationFormats =
            this.PartialViewLocationFormats = viewLocationFormats.Select(f => string.Format(f, assemblyName)).ToArray();

            this.AreaViewLocationFormats =
            this.AreaMasterLocationFormats =
            this.AreaPartialViewLocationFormats = areaViewLocationFormats.Select(f => string.Format(f, assemblyName)).ToArray();
        }
    }
}