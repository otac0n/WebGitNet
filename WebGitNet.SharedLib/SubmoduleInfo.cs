// -----------------------------------------------------------------------
// <copyright file="SubmoduleInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmoduleInfo
    {        
        public string Path
        {
          get;
          private set;
        }

        public string Url
        {
          get;
          private set;
        }

        public SubmoduleInfo(string path, string url)
        {
            this.Path = path;
            this.Url = url;
        }
    }
}
