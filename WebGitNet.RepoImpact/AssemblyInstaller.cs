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
                                       .BasedOn<IRouteRegisterer>());
            container.Register(AllTypes.FromThisAssembly()
                                       .BasedOn<IController>()
                                       .Configure(c => c.LifeStyle.Transient));
        }
    }
}
