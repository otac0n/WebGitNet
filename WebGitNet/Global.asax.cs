//-----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Castle.MicroKernel.SubSystems.Configuration;

    public partial class WebGitNetApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var routeRegisterers = container.ResolveAll<IRouteRegisterer>();
            foreach (var registerer in routeRegisterers)
            {
                registerer.RegisterRoutes(routes);
            }

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Browse", action = "Index" });
        }

        protected void Application_Start()
        {
            Bootstrap();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        private static void Bootstrap()
        {
            var directoryFilter = new AssemblyFilter(HostingEnvironment.MapPath("~/Plugins"));

            container = new WindsorContainer()
                        .Install(new AssemblyInstaller())
                        .Install(FromAssembly.InDirectory(directoryFilter));

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private class AssemblyInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore configurationStore)
            {
                container.Register(AllTypes.FromThisAssembly()
                                           .BasedOn<IRouteRegisterer>());
                container.Register(AllTypes.FromThisAssembly()
                                           .BasedOn<IController>()
                                           .Configure(c => c.LifeStyle.Transient));
            }
        }
    }
}
