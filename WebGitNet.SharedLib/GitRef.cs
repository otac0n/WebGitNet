//-----------------------------------------------------------------------
// <copyright file="GitRef.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;

    public enum RefType
    {
        Branch,
        Tag,
        RemoteBranch,
        Stash,
        Notes,
    }

    public class GitRef
    {
        private Dictionary<RefType, string> prefixes = new Dictionary<RefType, string>
        {
            { RefType.Branch, "refs/heads" },
            { RefType.Tag,    "refs/tags" },
            { RefType.RemoteBranch, "refs/remotes" },
            { RefType.Notes, "refs/notes" },
        };

        public GitRef(string shaId, string refPath)
        {
            this.ShaId = shaId;
            this.RefPath = refPath;

            foreach (var prefix in prefixes)
            {
                if (refPath.StartsWith(prefix.Value))
                {
                    this.Name = refPath.Substring(prefix.Value.Length + 1);
                    this.RefType = prefix.Key;
                    return;
                }
            }

            if (refPath == "refs/stash")
            {
                this.Name = "stash";
                this.RefType = RefType.Stash;
                return;
            }

            throw new ArgumentException("The ref path specified is not recognized.", "refPath");
        }

        public string ShaId { get; private set; }

        public string Name { get; private set; }

        public string RefPath { get; private set; }

        public RefType RefType { get; private set; }
    }
}
