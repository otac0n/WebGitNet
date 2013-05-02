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
    using System;

    public class TreeView
    {
        private readonly string tree;
        private readonly string path;
        private readonly ReadOnlyCollection<ObjectInfo> objects;
        private readonly IDictionary<string, SubmoduleInfo> submodules;

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

        public string Tree
        {
            get { return this.tree; }
        }

        public string Path
        {
            get { return this.path; }
        }

        public IList<ObjectInfo> Objects
        {
            get { return this.objects; }
        }

        public IDictionary<string, SubmoduleInfo> Submodules
        {
          get { return this.submodules; }
        }
    }
}
