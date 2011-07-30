//-----------------------------------------------------------------------
// <copyright file="TreeView.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class TreeView
    {
        private readonly string tree;
        private readonly string path;
        private readonly ReadOnlyCollection<ObjectInfo> objects;

        public TreeView(string tree, string path, IEnumerable<ObjectInfo> objects)
        {
            this.tree = tree;

            path = path ?? string.Empty;
            if (!path.EndsWith("/") && !string.IsNullOrEmpty(path))
            {
                path += "/";
            }

            this.path = path;

            this.objects = objects.ToList().AsReadOnly();
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
    }
}