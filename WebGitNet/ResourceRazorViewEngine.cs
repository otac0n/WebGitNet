using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace WebGitNet
{
    public class ResourceRazorViewEngine : RazorViewEngine
    {
        private Dictionary<string, string> resourceHashChache = new Dictionary<string, string>();
        private ReaderWriterLockSlim writeLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var controllerType = controllerContext.Controller.GetType();
            var assembly = controllerType.Assembly;
            var partialName = GetResourceName(assembly, partialPath);

            var newPartialPath = ExtractViewToFile(assembly, partialName, controllerContext.HttpContext.Server);

            return base.CreatePartialView(controllerContext, newPartialPath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var controllerType = controllerContext.Controller.GetType();
            var assembly = controllerType.Assembly;
            var viewName = GetResourceName(assembly, viewPath);
            var masterName = GetResourceName(assembly, viewPath);

            var newViewPath = ExtractViewToFile(assembly, viewName, controllerContext.HttpContext.Server);
            var newMasterPath = ExtractViewToFile(assembly, masterName, controllerContext.HttpContext.Server);

            return base.CreateView(controllerContext, newViewPath, newMasterPath);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var controllerType = controllerContext.Controller.GetType();
            var assembly = controllerType.Assembly;
            var resourceName = GetResourceName(assembly, virtualPath);

            resourceName = NormalizeResourceName(assembly, resourceName);

            return resourceName != null;
        }

        private static string GetResourceName(Assembly assembly, string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                return "";
            }

            var assemblyName = assembly.GetName().Name;

            if (virtualPath.StartsWith("~/"))
            {
                virtualPath = virtualPath.Substring(2);
            }
            else if (virtualPath.StartsWith("/"))
            {
                virtualPath = virtualPath.Substring(1);
            }

            return (assemblyName + "/" + virtualPath)
                .Replace(@"/", ".")
                .Replace(@"\", ".");
        }

        private static string NormalizeResourceName(Assembly assembly, string resourceName)
        {
            return (from n in assembly.GetManifestResourceNames()
                    where n.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)
                    select n).SingleOrDefault();
        }

        private string ExtractViewToFile(Assembly assembly, string resourceName, HttpServerUtilityBase server)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return "";
            }

            resourceName = NormalizeResourceName(assembly, resourceName);
            if (resourceName == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            string hash;

            this.writeLock.EnterReadLock();
            try
            {
                if (resourceHashChache.TryGetValue(resourceName, out hash))
                {
                    var virtualPath = BuildVirtualPath(hash);
                    if (File.Exists(server.MapPath(virtualPath)))
                    {
                        return virtualPath;
                    }
                }
            }
            finally
            {
                this.writeLock.ExitReadLock();
            }

            this.writeLock.EnterWriteLock();
            try
            {
                if (!resourceHashChache.TryGetValue(resourceName, out hash))
                {
                    hash = HashResource(assembly, resourceName);
                    resourceHashChache.Add(resourceName, hash);
                }

                var virtualPath = BuildVirtualPath(hash);

                ExtractResource(assembly, resourceName, server.MapPath(virtualPath));

                return virtualPath;
            }
            finally
            {
                this.writeLock.ExitWriteLock();
            }
        }

        private static string BuildVirtualPath(string hash)
        {
            return string.Format("~/Views/PluginCache/{0}/{1}.cshtml", hash.Substring(0, 2), hash.Substring(2));
        }

        private static string HashResource(Assembly assembly, string resourceName)
        {
            using (var sha = new SHA1Managed())
            {
                using (var resource = assembly.GetManifestResourceStream(resourceName))
                {
                    var hash = sha.ComputeHash(resource);
                    var hashText = new StringBuilder(hash.Length * 2);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        hashText.Append(hash[i].ToString("x2"));
                    }

                    return hashText.ToString();
                }
            }
        }

        private static void ExtractResource(Assembly assembly, string resourceName, string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var @out = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var @in = assembly.GetManifestResourceStream(resourceName))
                {
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = @in.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        @out.Write(buffer, 0, read);
                    }
                }
            }
        }
    }
}