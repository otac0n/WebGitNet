//-----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System.Collections.Generic;
    using Intervals;

    public class SearchResult
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public object RouteValues { get; set; }

        public string ResultText { get; set; }

        public IList<StringInterval> Results { get; set; }
    }
}
