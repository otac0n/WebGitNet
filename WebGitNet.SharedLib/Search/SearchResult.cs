//-----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System.Collections.Generic;

    public class SearchResult
    {
        public string LinkText { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public object RouteValues { get; set; }

        public IList<SearchLine> Lines { get; set; }
    }
}
