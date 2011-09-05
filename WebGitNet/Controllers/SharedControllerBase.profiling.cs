//-----------------------------------------------------------------------
// <copyright file="SharedControllerBase.profiling.cs" company="(none)">
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
    using MvcMiniProfiler;
    using System.Web.Mvc;

    public partial class SharedControllerBase
    {
        private IDisposable actionExecuting;
        private IDisposable resultExecuting;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.actionExecuting = MiniProfiler.StepStatic("Action Executing");
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (this.actionExecuting != null)
            {
                this.actionExecuting.Dispose();
                this.actionExecuting = null;
            }
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            this.resultExecuting = MiniProfiler.StepStatic("Result Executing");
            base.OnResultExecuting(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (this.resultExecuting != null)
            {
                this.resultExecuting.Dispose();
                this.resultExecuting = null;
            }
        }
    }
}