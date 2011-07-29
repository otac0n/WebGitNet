namespace WebGitNet.ActionResults
{
    using System;
    using System.Web.Mvc;

    public class GitCommandResult : ActionResult
    {
        private readonly string commandFormat;
        private readonly string service;
        private readonly string workingDir;

        public GitCommandResult(string commandFormat, string service, string workingDir)
        {
            if (string.IsNullOrEmpty(commandFormat))
            {
                throw new ArgumentNullException("commandFormat");
            }

            this.commandFormat = commandFormat;

            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentNullException("service");
            }

            this.service = service;

            if (string.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentNullException("workingDir");
            }

            this.workingDir = workingDir;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            throw new NotImplementedException();
        }
    }
}