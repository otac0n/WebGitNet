namespace WebGitNet.SearchProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WebGitNet.Search;

    public class PathSearchProvider : ISearchProvider
    {
        public Task<IList<SearchResult>> Search(SearchQuery query, FileManager fileManager, RepoInfo repository, int skip, int count)
        {
            if (repository != null)
            {
                return Task.Factory.StartNew(() => (IList<SearchResult>)Search(query, repository).Skip(skip).Take(count).ToList());
            }
            else
            {
                return Task.Factory.StartNew(() => (IList<SearchResult>)Search(query, fileManager).Skip(skip).Take(count).ToList());
            }
        }

        private IEnumerable<SearchResult> Search(SearchQuery query, RepoInfo repo, bool includeRepoName = false)
        {
            TreeView tree;

            try
            {
                tree = GitUtilities.GetTreeInfo(repo.RepoPath, "HEAD", recurse: true);
            }
            catch (GitErrorException)
            {
                yield break;
            }

            foreach (var item in tree.Objects)
            {
                var name = item.Name.Substring(item.Name.LastIndexOf('/') + 1);

                if (query.Terms.All(t => name.IndexOf(t, StringComparison.OrdinalIgnoreCase) != -1))
                {
                    var linkText = (includeRepoName ? repo.Name + " " : string.Empty) + "/" + item.Name;

                    switch (item.ObjectType)
                    {
                        case ObjectType.Tree:
                            yield return new SearchResult
                            {
                                LinkText = linkText + "/",
                                ActionName = "ViewTree",
                                ControllerName = "Browse",
                                RouteValues = new { repo = repo.Name, @object = tree.Tree, path = tree.Path + item.Name + "/" },
                                Lines = new List<SearchLine>(),
                            };
                            break;

                        case ObjectType.Blob:
                            yield return new SearchResult
                            {
                                LinkText = linkText,
                                ActionName = "ViewBlob",
                                ControllerName = "Browse",
                                RouteValues = new { repo = repo.Name, @object = tree.Tree, path = tree.Path + item.Name },
                                Lines = new List<SearchLine>(),
                            };
                            break;

                        case ObjectType.Commit:
                            yield return new SearchResult
                            {
                                LinkText = linkText + "/",
                                ActionName = "ViewTree",
                                ControllerName = "Browse",
                                RouteValues = new { repo = repo.Name, @object = tree.Tree, path = tree.Path + item.Name + "/" },
                                Lines = new List<SearchLine>(),
                            };
                            break;
                    }
                }
            }
        }

        public IEnumerable<SearchResult> Search(SearchQuery query, FileManager fileManager)
        {
            var repos = from dir in fileManager.DirectoryInfo.EnumerateDirectories()
                        let repoInfo = GitUtilities.GetRepoInfo(dir.FullName)
                        where repoInfo.IsGitRepo
                        select repoInfo;

            return from repo in repos
                   from searchResult in Search(query, repo, includeRepoName: true)
                   select searchResult;
        }
    }
}
