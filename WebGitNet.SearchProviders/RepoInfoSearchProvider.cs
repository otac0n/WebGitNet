namespace WebGitNet.SearchProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WebGitNet.Search;
    using System.Threading.Tasks;
    using Intervals;

    public class RepoInfoSearchProvider : ISearchProvider
    {
        public Task<IList<SearchResult>> Search(SearchQuery query, FileManager fileManager, RepoInfo repository, int skip, int count)
        {
            if (repository != null)
            {
                return Task.Factory.StartNew(() => (IList<SearchResult>)new SearchResult[0]);
            }

            return Task.Factory.StartNew(() =>
            {
                return (IList<SearchResult>)this.Search(query, fileManager).Skip(skip).Take(count).ToList();
            });
        }

        public IEnumerable<SearchResult> Search(SearchQuery query, FileManager fileManager)
        {
            var repos = from dir in fileManager.DirectoryInfo.EnumerateDirectories()
                        let repoInfo = GitUtilities.GetRepoInfo(dir.FullName)
                        where repoInfo.IsGitRepo
                        select repoInfo;

            foreach (var repo in repos)
            {
                var matches = (from t in query.Terms
                               select new
                               {
                                  NameMathces = SearchHelper.FindIntervals(t, repo.Name),
                                  DescriptionMatches = SearchHelper.FindIntervals(t, repo.Description)
                               }).ToList();

                if (matches.All(t => t.NameMathces.Count > 0 || t.DescriptionMatches.Count > 0))
                {
                    yield return new SearchResult
                    {
                        LinkText = repo.Name,
                        ActionName = "ViewRepo",
                        ControllerName = "Browse",
                        RouteValues = new { repo = repo.Name },
                        Intervals = Enumerable.Concat(
                            matches.SelectMany(m => m.NameMathces),
                            matches.SelectMany(m => m.DescriptionMatches)).ToList(),
                    };
                }
            }
        }
    }
}
