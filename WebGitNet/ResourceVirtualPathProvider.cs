namespace WebGitNet
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Hosting;

    public class ResourceVirtualPathProvider : VirtualPathProvider
    {
        private const string Prefix = "~/Views/Plugins/";

        public override bool FileExists(string virtualPath)
        {
            var isPlugin = IsPluginPath(virtualPath);
            return isPlugin || base.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var isPlugin = IsPluginPath(virtualPath);
            if (isPlugin)
            {
                return null;
            }
            else
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart); ;
            }
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            string resourceName;
            var assembly = FindAssembly(virtualPath, out resourceName);

            if (assembly != null)
            {
                return new AssemblyResourceVirtualFile(virtualPath, assembly, resourceName);
            }
            else
            {
                return base.GetFile(virtualPath);
            }
        }

        private Assembly FindAssembly(string virtualPath, out string resourceName)
        {
            resourceName = null;

            virtualPath = VirtualPathUtility.ToAppRelative(virtualPath);
            if (!virtualPath.StartsWith(Prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var assemblyPath = virtualPath.Substring(Prefix.Length);
            var parts = assemblyPath.Split("/".ToArray(), 2);
            if (parts.Length != 2)
            {
                return null;
            }

            var assemblyName = parts[0];
            var filePath = VirtualPathUtility.ToAppRelative("/Views/" + parts[1]);

            var assembly = (from a in AppDomain.CurrentDomain.GetAssemblies()
                            where a.GetName().Name == assemblyName
                            select a).SingleOrDefault();

            var name = assembly.GetResourceName(filePath);
            resourceName = assembly.GetManifestResourceNames().Where(s => s == name).FirstOrDefault();
            if (resourceName != null)
            {
                return assembly;
            }
            else
            {
                return null;
            }
        }

        private bool IsPluginPath(string virtualPath)
        {
            string resourceName;
            var assembly = FindAssembly(virtualPath, out resourceName);
            return assembly != null;
        }

        private class AssemblyResourceVirtualFile : VirtualFile
        {
            private Assembly assembly;
            private string resourceName;

            public AssemblyResourceVirtualFile(string virtualPath, Assembly assembly, string resourceName)
                : base(virtualPath)
            {
                if (assembly == null)
                {
                    throw new ArgumentNullException("assembly");
                }

                this.assembly = assembly;

                if (string.IsNullOrEmpty(resourceName))
                {
                    throw new ArgumentNullException("resourceName");
                }

                this.resourceName = resourceName;
            }

            public override Stream Open()
            {
                return this.assembly.GetManifestResourceStream(this.resourceName);
            }
        }
    }
}
