//-----------------------------------------------------------------------
// <copyright file="TreeView.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class TreeView
    {
        private readonly ReadOnlyCollection<ObjectInfo> objects;
        private readonly string path;
        private readonly IDictionary<string, SubmoduleInfo> submodules;
        private readonly string tree;

        public TreeView(string tree, string path, IEnumerable<ObjectInfo> objects, IDictionary<string, SubmoduleInfo> submodules)
        {
            this.tree = tree;

            path = path ?? string.Empty;
            if (!path.EndsWith("/") && !string.IsNullOrEmpty(path))
            {
                path += "/";
            }

            this.path = path;

            this.objects = objects.ToList().AsReadOnly();
            this.submodules = submodules;
        }

        public IList<ObjectInfo> Objects
        {
            get { return this.objects; }
        }

        public string Path
        {
            get { return this.path; }
        }

        public IDictionary<string, SubmoduleInfo> Submodules
        {
            get { return this.submodules; }
        }

        public string Tree
        {
            get { return this.tree; }
        }
    }
}
