namespace WebGitNet.SearchProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebGitNet.Search;

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
                if (query.Terms.All(t => repo.Name.IndexOf(t, StringComparison.OrdinalIgnoreCase) != -1 || repo.Description.IndexOf(t, StringComparison.OrdinalIgnoreCase) != -1))
                {
                    yield return new SearchResult
                    {
                        LinkText = repo.Name,
                        ActionName = "ViewRepo",
                        ControllerName = "Browse",
                        RouteValues = new { repo = repo.Name },
                        Lines = new List<SearchLine>
                        {
                            new SearchLine { Line = repo.Description },
                        },
                    };
                }
            }
        }
    }
}
