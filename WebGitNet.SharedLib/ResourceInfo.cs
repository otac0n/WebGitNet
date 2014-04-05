//-----------------------------------------------------------------------
// <copyright file="ResourceInfo.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.IO;

    public class ResourceInfo
    {
        public FileSystemInfo FileSystemInfo { get; set; }

        public string FullPath { get; set; }

        public string LocalPath { get; set; }

        public string Name { get; set; }

        public ResourceType Type { get; set; }
    }
}
