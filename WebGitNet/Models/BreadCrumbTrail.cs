//-----------------------------------------------------------------------
// <copyright file="BreadCrumbTrail.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BreadCrumbTrail : IEnumerable<BreadCrumbTrail.BreadCrumb>
    {
        private readonly LinkedList<BreadCrumb> breadCrumbs = new LinkedList<BreadCrumb>();

        public void Append(string name, object routeValues)
        {
            this.breadCrumbs.AddLast(new BreadCrumb(name, routeValues));
        }

        public void Append<TSource>(IEnumerable<TSource> crumbSource, Func<TSource, string> nameSelector, Func<TSource, object> routeValuesSelector)
        {
            this.Append(crumbSource, nameSelector, (item, name) => routeValuesSelector(item));
        }

        public void Append<TSource>(IEnumerable<TSource> crumbSource, Func<TSource, string> nameSelector, Func<TSource, string, object> routeValuesSelector)
        {
            foreach (var crumb in crumbSource)
            {
                var name = nameSelector(crumb);
                var routeValues = routeValuesSelector(crumb, name);

                this.Append(name, routeValues);
            }
        }

        public IEnumerator<BreadCrumbTrail.BreadCrumb> GetEnumerator()
        {
            return this.breadCrumbs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public sealed class BreadCrumb
        {
            private readonly string name;
            private readonly object routeValues;

            public BreadCrumb(string name, object routeValues)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }

                this.name = name;

                if (routeValues == null)
                {
                    throw new ArgumentNullException("routeValues");
                }

                this.routeValues = routeValues;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public object RouteValues
            {
                get
                {
                    return this.routeValues;
                }
            }
        }
    }
}
