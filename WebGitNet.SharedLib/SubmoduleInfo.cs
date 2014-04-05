// -----------------------------------------------------------------------
// <copyright file="SubmoduleInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebGitNet
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SubmoduleInfo
    {
        public SubmoduleInfo(string path, string url)
        {
            this.Path = path;
            this.Url = url;
        }

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
    }
}
