namespace WebGitNet
{
    public class RepoInfo
    {
        public string Description { get; set; }

        public bool IsArchived { get; set; }

        public bool IsBare { get; set; }

        public bool IsGitRepo { get; set; }

        public string Name { get; set; }

        public string RepoPath { get; set; }
    }
}
