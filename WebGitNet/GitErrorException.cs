namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class GitErrorException : Exception
    {
        private readonly string command;
        private readonly int exitCode;
        private readonly string errorOutput;

        public GitErrorException(string command, int exitCode, string errorOutput)
            : base("Fatal error executing git command." + Environment.NewLine + errorOutput)
        {
            this.command = command;
            this.exitCode = exitCode;
            this.errorOutput = errorOutput;
        }

        public string Command { get { return this.command; } }

        public int ExitCode { get { return this.exitCode; } }

        public string ErrorOutput { get { return this.errorOutput; } }
    }
}