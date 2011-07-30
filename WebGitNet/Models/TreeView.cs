namespace WebGitNet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Collections.ObjectModel;

    public class TreeView
    {
        private readonly string tree;
        private readonly string path;
        private readonly ReadOnlyCollection<ObjectInfo> objects;

        public TreeView(string tree, string path, IEnumerable<ObjectInfo> objects)
        {
            this.tree = tree;

            path = path ?? "";
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