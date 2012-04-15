//-----------------------------------------------------------------------
// <copyright file="SearchQuery.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System.Collections.Generic;
    using System.Linq;

    public class SearchQuery
    {
        public SearchQuery(IEnumerable<string> terms)
        {
            this.Terms = terms.ToList().AsReadOnly();
        }

        public IList<string> Terms { get; private set; }
    }
}
