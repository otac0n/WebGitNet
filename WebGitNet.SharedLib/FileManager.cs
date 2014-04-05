//-----------------------------------------------------------------------
// <copyright file="FileManager.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.IO;

    public class FileManager
    {
        private readonly DirectoryInfo dirInfo;
        private readonly string rootPath;

        public FileManager(string path)
        {
            path = Path.GetDirectoryName(Path.Combine(path, "slug"));
            this.dirInfo = new DirectoryInfo(path);
            this.rootPath = path + Path.DirectorySeparatorChar;
        }

        public DirectoryInfo DirectoryInfo
        {
            get
            {
                return this.dirInfo;
            }
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
                info.FileSystemInfo = fullPath.EndsWith(Path.DirectorySeparatorChar.ToString())
                    ? (FileSystemInfo)new DirectoryInfo(fullPath)
                    : new FileInfo(fullPath);
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
            return Path.GetFullPath(path);
        }
    }
}
