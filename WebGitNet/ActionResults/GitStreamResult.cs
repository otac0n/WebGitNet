//-----------------------------------------------------------------------
// <copyright file="GitStreamResult.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.ActionResults
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;

    public class GitStreamResult : ActionResult
    {
        private readonly string commandFormat;
        private readonly string action;
        private readonly string repoPath;

        public GitStreamResult(string commandFormat, string action, string repoPath)
        {
            if (string.IsNullOrEmpty(commandFormat))
            {
                throw new ArgumentNullException("commandFormat");
            }

            this.commandFormat = commandFormat;

            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;

            if (string.IsNullOrEmpty(repoPath))
            {
                throw new ArgumentNullException("repoPath");
            }

            this.repoPath = repoPath;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;

            response.ContentType = "application/git-" + this.action + "-result";
            response.Buffer = false;
            response.BufferOutput = false;
            response.ContentEncoding = Encoding.GetEncoding(1252);

            using (var git = GitUtilities.Start(string.Format(this.commandFormat, this.action), this.repoPath))
            {
                var buffer = new char[8192];
                int read;

                using (var input = new StreamReader(request.InputStream, Encoding.GetEncoding(1252)))
                {
                    while ((read = input.ReadBlock(buffer, 0, buffer.Length)) > 0)
                    {
                        git.StandardInput.Write(buffer, 0, read);
                    }
                }

                while ((read = git.StandardOutput.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    response.Write(buffer, 0, read);
                }
            }
        }
    }
}