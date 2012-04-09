//-----------------------------------------------------------------------
// <copyright file="AssemblyInstaller.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.RepoImpact
{
    using System.Web.Mvc;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class AssemblyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromThisAssembly()
                                       .BasedOn<IRouteRegisterer>()
                                       .WithService.FromInterface());
            container.Register(AllTypes.FromThisAssembly()
                                       .BasedOn<IController>()
                                       .Configure(c => c.Named(c.Implementation.Name))
                                       .LifestyleTransient());
        }
    }
}
