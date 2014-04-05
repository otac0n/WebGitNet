//-----------------------------------------------------------------------
// <copyright file="WindsorControllerFactory.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>Dustin R. Welden</author>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.SessionState;
    using Castle.MicroKernel;

    public class WindsorControllerFactory : IControllerFactory
    {
        private readonly IKernel kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            controllerName =
                this.kernel.HasComponent(controllerName) ? controllerName :
                this.kernel.HasComponent(controllerName + "Controller") ? controllerName + "Controller" :
                "";

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return this.kernel.Resolve<IController>(controllerName);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        public void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }
    }
}
