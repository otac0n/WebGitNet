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
    using System.Threading;
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
                var readThread = new Thread(() =>
                {
                    var readBuffer = new char[1048576];
                    int readCount;

                    using (var input = new StreamReader(request.InputStream, Encoding.GetEncoding(1252)))
                    {
                        while ((readCount = input.ReadBlock(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            git.StandardInput.Write(readBuffer, 0, readCount);
                        }
                    }
                });
                readThread.Start();

                var writeBuffer = new char[4194304];
                int writeCount;
                while ((writeCount = git.StandardOutput.ReadBlock(writeBuffer, 0, writeBuffer.Length)) > 0)
                {
                    response.Write(writeBuffer, 0, writeCount);
                }

                readThread.Join();
            }
        }
    }
}