namespace WebGitNet.SearchProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Search;
    using System.Threading.Tasks;

    public class RepoGrepSearchProvider : ISearchProvider
    {
        public Task<IList<SearchResult>> Search(SearchQuery query, FileManager fileManager, RepoInfo repository, int skip, int count)
        {
            if (repository != null)
            {
                return Task.Factory.StartNew(() => (IList<SearchResult>)Search(query, repository).Skip(skip).Take(count).ToList());
            }

            return Task.Factory.StartNew(() => (IList<SearchResult>)Search(query, fileManager).Skip(skip).Take(count).ToList());
        }

        private IEnumerable<GrepResult> GrepRepo(string term, string repoPath)
        {
            var commandResult = GitUtilities.Execute(string.Format("grep --line-number --fixed-strings -e {0} HEAD", GitUtilities.Q(term)), repoPath);
            var repoResults = commandResult.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return from m in repoResults
                   where !m.StartsWith("Binary file")
                   let parts = m.Split(':')
                   let filePath = parts[1]
                   let searchLine = new SearchLine
                   {
                       Line = parts[3],
                       LineNumber = int.Parse(parts[2]),
                   }
                   group searchLine by filePath into g
                   select new GrepResult
                   {
                       Term = term,
                       FilePath = g.Key,
                       Matches= g.ToList(),
                   };
        }

        public IEnumerable<SearchResult> Search(SearchQuery query, RepoInfo repository)
        {
            var results = new List<SearchResult>();
            foreach (var matches in query.Terms.Select(t => GrepRepo(t, repository.RepoPath)))
            {
                results.AddRange(matches.Select(match => new SearchResult
                {
                    LinkText = match.FilePath,
                    ActionName = "ViewBlob",
                    ControllerName = "Browse",
                    RouteValues = new { repo = repository.Name, @object = "HEAD", path = match.FilePath },
                    Lines = match.Matches,
                }));
            }

            return results;
        }

        public IEnumerable<SearchResult> Search(SearchQuery query, FileManager fileManager)
        {
            var repos = from dir in fileManager.DirectoryInfo.EnumerateDirectories()
                        let repoInfo = GitUtilities.GetRepoInfo(dir.FullName)
                        where repoInfo.IsGitRepo
                        select repoInfo;

            var results = new List<SearchResult>();
            foreach (var repo in repos)
            {
                results.AddRange(Search(query, repo));
            }

            return results;
        }

        private class GrepResult
        {
            public string Term;
            public string FilePath;
            public List<SearchLine> Matches;
        }
    }
}
