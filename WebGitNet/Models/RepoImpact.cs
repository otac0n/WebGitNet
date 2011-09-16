namespace WebGitNet.Models
{
    using System.Linq;

    public class RepoImpact
    {
        public IOrderedEnumerable<UserImpact> AllTime { get; set; }

        public IOrderedEnumerable<ImpactWeek> Weekly { get; set; }
    }
}