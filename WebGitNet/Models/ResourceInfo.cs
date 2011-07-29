//-----------------------------------------------------------------------
// <copyright file="ResourceInfo.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    public class ResourceInfo
    {
        public ResourceType Type { get; set; }

        public string LocalPath { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }
    }
}
