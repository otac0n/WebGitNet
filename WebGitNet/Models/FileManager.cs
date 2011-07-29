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
        private readonly string fullPath;
        private readonly DirectoryInfo dirInfo;

        public FileManager(string path)
        {
            this.dirInfo = new DirectoryInfo(path);
            this.fullPath = this.dirInfo.FullName;
        }
    }
}
