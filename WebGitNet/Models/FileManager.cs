//-----------------------------------------------------------------------
// <copyright file="FileManager.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System.IO;

    public class FileManager
    {
        private readonly string rootPath;
        private readonly DirectoryInfo dirInfo;

        public FileManager(string path)
        {
            this.dirInfo = new DirectoryInfo(path);
            this.rootPath = this.dirInfo.FullName + Path.DirectorySeparatorChar;
        }

        public ResourceInfo GetResourceInfo(string resourcePath)
        {
            var fullPath = this.FindFullPath(resourcePath);
            var info = new ResourceInfo { FullPath = fullPath };

            if (!fullPath.StartsWith(this.rootPath))
            {
                info.Type = ResourceType.NotFound;
            }
            else if (File.Exists(fullPath))
            {
                info.Type = ResourceType.File;
                info.FileSystemInfo = new FileInfo(fullPath);
            }
            else if (Directory.Exists(fullPath))
            {
                info.Type = ResourceType.Directory;
                info.FileSystemInfo = new DirectoryInfo(fullPath);
            }
            else
            {
                info.Type = ResourceType.NotFound;
            }

            if (info.Type != ResourceType.NotFound)
            {
                info.LocalPath = fullPath.Substring(this.rootPath.Length).Replace(@"\", @"/");
                info.Name = info.FileSystemInfo.Name;
            }

            return info;
        }

        private string FindFullPath(string url)
        {
            var path = Path.Combine(this.rootPath, url);
            var info = new FileInfo(path);

            return info.FullName;
        }
    }
}
