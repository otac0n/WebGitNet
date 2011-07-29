namespace WebGitNet.Controllers
{
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using WebGitNet.Models;

    public class BrowseController : Controller
    {
        private readonly FileManager fileManager;

        public BrowseController()
        {
            var reposPath = WebConfigurationManager.AppSettings["RepositoriesPath"];
            this.fileManager = new FileManager(reposPath);
        }

        public ActionResult Index()
        {
            var directory = this.fileManager.DirectoryInfo;

            var repos = (from dir in directory.EnumerateDirectories()
                         select dir.Name).ToList();

            return View(repos);
        }

        public ActionResult ViewRepo(string repo)
        {
            var resourceInfo = this.fileManager.GetResourceInfo(repo);
            if (resourceInfo.Type != ResourceType.Directory)
            {
                return HttpNotFound();
            }

            ViewBag.RepoName = resourceInfo.Name;
            ViewBag.LastCommit = GitUtilities.Execute("log -1 --shortstat", resourceInfo.FullPath);

            return View();
        }
    }
}
