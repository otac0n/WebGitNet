//-----------------------------------------------------------------------
// <copyright file="ISearchProvider.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISearchProvider
    {
        Task<IList<SearchResult>> Search(SearchQuery query, FileManager fileManager, RepoInfo repository, int skip, int count);
    }
}
