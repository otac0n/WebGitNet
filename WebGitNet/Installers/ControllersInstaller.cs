namespace WebGitNet.Installers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.MicroKernel.SubSystems.Configuration;
    using System.Web.Mvc;
    using System.Reflection;
    using System.IO;

    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore configurationStore)
        {
            container.Register(AllTypes.FromAssemblyInDirectory(new AssemblyFilter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
                                       .BasedOn<IController>()
                                       .If(t => t.Name.EndsWith("Controller"))
                                       .Configure(c => c.LifeStyle.Transient));
        }
    }
}