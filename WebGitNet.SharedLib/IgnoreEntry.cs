//-----------------------------------------------------------------------
// <copyright file="IgnoreEntry.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class IgnoreEntry
    {
        private string pathGlobs;
        private ReadOnlyCollection<Regex> pathRegexes;

        public string CommitHash { get; set; }

        public string PathGlobs
        {
            get { return this.pathGlobs; }
            set { this.pathGlobs = value; this.pathRegexes = null; }
        }

        public bool Negated { get; set; }

        public bool Rooted { get; set; }

        public ReadOnlyCollection<Regex> PathRegexes
        {
            get
            {
                if (this.pathRegexes == null && this.pathGlobs != null)
                {
                    this.pathRegexes = (from glob in this.pathGlobs.Split('/')
                                        select PathUtilities.GlobToRegex(glob)).ToList().AsReadOnly();
                }

                return this.pathRegexes;
            }
        }

        public bool IsMatch(string path)
        {
            var parts = path.Split('/');
            var regexes = this.PathRegexes;

            var maxPart = parts.Length - regexes.Count;

            if (maxPart < 0)
            {
                return false;
            }

            if (this.Rooted)
            {
                maxPart = 0;
            }

            for (int part = 0; part <= maxPart; part++)
            {
                bool match = true;

                for (int i = 0; i < regexes.Count; i++)
                {
                    if (!regexes[i].IsMatch(parts[part + i]))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
